using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SubViewsMultiOpenerLocator: MonoBehaviour, ISubViewsLocator
{
    private ISubViewCollectLocator subViewCollectLocator;
    private IViewConfigure viewConfigure;
    private IViewLoader viewLoader;
    
    private List<ISubViewConfigure> subViewConfigures;

    [SerializeField] private SerializableDictionary<int, int> indexUniqueIds;
    [SerializeField] private SerializableDictionary<int, Type> uniqueIdTypes;
    [SerializeField] private SerializableDictionary<int, IView> uniqueIdViews;
    
    private SubViewMultiOpenerBase subViewMultiOpener;
    
    void ISubViewsLocator.Init(ISubViewCollectLocator subViewCollectLocator, IViewConfigure viewConfigure, SubViewShow? firstSubViewShow)
    {
        this.subViewCollectLocator = subViewCollectLocator;
        this.viewConfigure = viewConfigure;
        this.viewLoader = subViewCollectLocator.GetViewLoader();
        this.subViewConfigures = new List<ISubViewConfigure>();
        this.indexUniqueIds = new SerializableDictionary<int, int>();
        this.uniqueIdTypes = new SerializableDictionary<int, Type>();
        this.uniqueIdViews = new SerializableDictionary<int, IView>();
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
        subViewMultiOpener = GetComponent<SubViewMultiOpenerBase>();
        ShowAllViewAsync(subViewMultiOpener.GetRtPanelParent()).Forget();
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
        IViewLocator viewLocator;
        if (!viewLoader.TryGetActiveView(type, out IView view))
        {
            if (!viewLoader.TryGetPoolView(type, out view))
            {
                view = await viewLoader.CreateView(type);
                GameObject goView = view.GameObject();
                viewLocator = subViewCollectLocator.AddSubViewLocator(goView);
                subViewConfigure.TryGetViewCheck(out IViewCheck viewCheck);
                viewLocator.Bind(view, viewCheck);
                view.BindLocator(viewLocator);
                
                RectTransform windowRt = goView.GetComponent<RectTransform>();
                windowRt.SetParent(rtPanelParent);
                windowRt.localPosition = Vector3.zero;
                windowRt.localScale = Vector3.one;
                windowRt.anchoredPosition = Vector2.zero;
                windowRt.anchorMin = Vector2.zero;
                windowRt.anchorMax = Vector2.one;
                windowRt.offsetMin = Vector2.zero;
                windowRt.offsetMax = Vector2.zero;
            }
        }
        else
        {
            viewLocator = view.GetLocator();
            int oldUniqueId = viewLocator.GetUniqueId();
            uniqueIdViews.Remove(oldUniqueId);
            viewLocator.Hide();
        }
        if (!indexUniqueIds.TryGetValue(index, out int uniqueId))
        {
            indexUniqueIds.Add(index, uniqueId = UniqueIdGenerator.Default.Create());
            uniqueIdTypes.Add(uniqueId, type);
        }
        uniqueIdViews.Add(index, view);
        viewLocator = view.GetLocator();
        viewLocator.BindUniqueId(uniqueId);
        viewLocator.GameObject().transform.SetAsLastSibling();
        viewLocator.Show();
        return view;
    }

    void ISubViewsLocator.HideViews()
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

            if (uniqueIdViews.Remove(uniqueId, out IView view))
            {
                IViewLocator viewLocator = view.GetLocator();
                viewLocator.Hide();
                viewLoader.ReleaseView(view);
            }
        }
    }
}
