using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SubViewLayerMultiLocatorBase<TSubViewLayerContainer, TSubViewLoader, TSubViewLocator>: SubViewLayerLocatorBase<TSubViewLayerContainer, TSubViewLoader, TSubViewLocator>
    where TSubViewLayerContainer: class, ISubViewLayerContainer
    where TSubViewLoader: class, IEntityLoader<Type, ViewEntityBase>, ISubViewLoader
    where TSubViewLocator: MonoBehaviour, ISubViewLocator
{
    [SerializeField] private SerializableDictionary<int, ViewEntityBase> uniqueIdViews;
    
    private List<ISubViewConfigure> subViewConfigures;
    private ISubViewMultiLocator subViewMultiOpener;

    public override void Init(IViewConfigure viewConfigure, SubViewShow? firstSubViewShow)
    {
        base.Init(viewConfigure, firstSubViewShow);
        this.uniqueIdViews = new SerializableDictionary<int, ViewEntityBase>();
        this.subViewConfigures = new List<ISubViewConfigure>();
        viewConfigure.TryGetSubViewConfigures(out List<ISubViewConfigure> subViewConfigures);
        for (int i = 0; i < subViewConfigures.Count; i++)
        {
            ISubViewConfigure subViewConfigure = subViewConfigures[i];
            if (!subViewConfigure.TryGetViewCheck(out IViewCheck viewCheck)
                || viewCheck.IsFuncOpen())
            {
                this.subViewConfigures.Add(subViewConfigure);
            }
        }
        subViewMultiOpener = GetComponent<ISubViewMultiLocator>();
        ShowAllViewAsync(subViewMultiOpener.GetRtParent()).Forget();
    }

    private async UniTask ShowAllViewAsync(RectTransform rtPanelParent)
    {
        for (int i = 0; i < subViewConfigures.Count; i++)
        {
            await ShowViewAsync(rtPanelParent, i);
        }
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
        
        int uniqueId = UniqueIdGenerator.Default.Create();
        uniqueIdViews.Add(uniqueId, view);
        subViewLocator = (ISubViewLocator)view.GetLocator();
        
        subViewConfigure.TryGetViewCheck(out IViewCheck viewCheck);
        subViewLocator.Bind(view, viewCheck);
        
        subViewLocator.BindUniqueId(uniqueId);
        subViewLocator.GameObject().transform.SetAsLastSibling();
        subViewLocator.Show();
        return view;
    }
   
    public override void HideViews()
    {
        foreach (var pair in uniqueIdViews)
        {
            int uniqueId = pair.Key;
            ViewEntityBase view = pair.Value;
            ViewModelGenerator.Default.Delete(uniqueId);
            UniqueIdGenerator.Default.Delete(uniqueId);

            ISubViewLocator subViewLocator = (ISubViewLocator)view.GetLocator();
            subViewLocator.Hide();
            Type type = view.GetType();
            subViewLoader.Release(type, view);
        }
        uniqueIdViews.Clear();
    }
}
