using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SubViewLayerSelectLocatorBase<TSubViewLayerContainer, TSubViewLoader, TSubViewLocator>: SubViewLayerLocatorBase<TSubViewLayerContainer, TSubViewLoader, TSubViewLocator>
    where TSubViewLayerContainer: class, ISubViewLayerContainer
    where TSubViewLoader: EntityUniqueLoader<Type, ViewEntityBase>, ISubViewLoader
    where TSubViewLocator: MonoBehaviour, ISubViewLocator
{
    [SerializeField] private SerializableDictionary<int, int> indexUniqueIds;
    [SerializeField] private SerializableDictionary<int, Type> uniqueIdTypes;
    [SerializeField] private SerializableDictionary<int, ViewEntityBase> uniqueIdViews;

    private List<ISubViewConfigure> subViewConfigures;
    private int showIndex = -1;
    private ISubViewSelectLocator subViewSelector;

    public override void Init(IViewConfigure viewConfigure, SubViewShow? firstSubViewShow)
    {
        base.Init(viewConfigure, firstSubViewShow);
        this.indexUniqueIds = new SerializableDictionary<int, int>();
        this.uniqueIdTypes = new SerializableDictionary<int, Type>();
        this.uniqueIdViews = new SerializableDictionary<int, ViewEntityBase>();
        this.subViewConfigures = new List<ISubViewConfigure>();
        viewConfigure.TryGetSubViewConfigures(out List<ISubViewConfigure> subViewConfigures);
        int firstShowIndex = 0;
        for (int i = 0; i < subViewConfigures.Count; i++)
        {
            ISubViewConfigure subViewConfigure = subViewConfigures[i];
            if (!subViewConfigure.TryGetViewCheck(out IViewCheck viewCheck)
                || viewCheck.IsFuncOpen())
            {
                if (firstSubViewShow.HasValue && subViewConfigure.EqualsSubViewShow(firstSubViewShow.Value))
                {
                    firstShowIndex = subViewConfigures.Count;
                }
                this.subViewConfigures.Add(subViewConfigure);
            }
        }
        subViewSelector = GetComponent<ISubViewSelectLocator>();
        subViewSelector.Init(subViewConfigures, OnSubViewSelector, firstShowIndex);
    }

    private async UniTask OnSubViewSelector(RectTransform rtPanelParent, int index)
    {
        if (index == showIndex) return;
        PushHideView(showIndex);
        showIndex = index;
        await ShowViewAsync(rtPanelParent, index);
    }
    
    private async UniTask<IView> ShowViewAsync(RectTransform rtPanelParent, int index)
    {
        ISubViewConfigure subViewConfigure = subViewConfigures[index];
        Type type = subViewConfigure.GetSubViewType();
        ISubViewLocator subViewLocator;
        RectTransform windowRt;
        if (!subViewLoader.TryGetFromActive(type, out ViewEntityBase view))
        {
            if (!subViewLoader.TryGetFromPool(type, out view))
            {
                view = await subViewLoader.Create(type);
                subViewLocator = view.gameObject.AddComponent<TSubViewLocator>();
                view.BindLocator(subViewLocator);
            }
        }
        else
        {
            subViewLocator = (ISubViewLocator)view.GetLocator();
            int oldUniqueId = subViewLocator.GetUniqueId();
            uniqueIdViews.Remove(oldUniqueId);
            subViewLocator.Hide();
        }
        windowRt = view.GetComponent<RectTransform>();
        windowRt.SetParent(rtPanelParent);
        windowRt.localPosition = Vector3.zero;
        windowRt.localScale = Vector3.one;
        windowRt.anchoredPosition = Vector2.zero;
        windowRt.anchorMin = Vector2.zero;
        windowRt.anchorMax = Vector2.one;
        windowRt.offsetMin = Vector2.zero;
        windowRt.offsetMax = Vector2.zero;
        
        if (!indexUniqueIds.TryGetValue(index, out int uniqueId))
        {
            indexUniqueIds.Add(index, uniqueId = UniqueIdGenerator.Default.Create());
            uniqueIdTypes.Add(uniqueId, type);
        }
        uniqueIdViews.Add(uniqueId, view);
        subViewLocator = (ISubViewLocator)view.GetLocator();
        
        subViewConfigure.TryGetViewCheck(out IViewCheck viewCheck);
        subViewLocator.Bind(view, viewCheck);
        
        subViewLocator.BindUniqueId(uniqueId);
        subViewLocator.GameObject().transform.SetAsLastSibling();
        subViewLocator.Show();
        return view;
    }
    
    private void PushHideView(int index)
    {
        if (!indexUniqueIds.TryGetValue(index, out int uniqueId)) return;
        if (!uniqueIdViews.Remove(uniqueId, out ViewEntityBase view)) return;
        ISubViewLocator subViewLocator = (ISubViewLocator)view.GetLocator();
        subViewLocator.Hide();
        Type type = view.GetType();
        subViewLoader.Release(type, view);
    }

    public override void HideViews()
    {
        foreach (var pair in indexUniqueIds)
        {
            int uniqueId = pair.Value;
            if (!uniqueIdTypes.Remove(uniqueId))
            {
                LLogger.Error($"请求关闭界面的唯一id: {uniqueId} 不存在！！");
                return;
            }
            ViewModelGenerator.Default.Delete(uniqueId);
            UniqueIdGenerator.Default.Delete(uniqueId);

            if (uniqueIdViews.Remove(uniqueId, out ViewEntityBase view))
            {
                ISubViewLocator subViewLocator = (ISubViewLocator)view.GetLocator();
                subViewLocator.Hide();
                Type type = view.GetType();
                subViewLoader.Release(type, view);
            }
        }
        indexUniqueIds.Clear();
        showIndex = -1;
    }
}
