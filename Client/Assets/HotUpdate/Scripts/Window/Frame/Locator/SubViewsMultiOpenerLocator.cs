using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SubViewsMultiOpenerLocator: MonoBehaviour, ISubViewsLocator
{
    private ISubViewCollectLocator subViewCollectLocator;
    private ISubViewCollectContainer subViewCollectContainer;
    private ISubViewCollectContainer subViewCollectContainer;
    private IViewLoader viewLoader;
    private Dictionary<SubViewShow, IViewCheck> subViewChecks;

    private List<SubViewShow> subViewTypes;
    private List<SubViewShow> displaySubViewTypes;

    private Dictionary<SubViewShow, IView> views; 
    
    void ISubViewsLocator.Init(ISubViewCollectLocator subViewCollectLocator, ISubViewCollectContainer subViewCollectContainer, Dictionary<SubViewShow, IViewCheck> subViewChecks)
    {
        this.subViewCollectLocator = subViewCollectLocator;
        this.subViewCollectContainer = subViewCollectLocator.GetSubViewCollectContainer();
        this.subViewDisplayContainer = subViewCollectContainer;
        this.viewLoader = subViewCollectContainer.GetViewLoader();
        this.subViewChecks = subViewChecks;
    }
    void ISubViewsLocator.InitSubViews(List<SubViewShow> subViewTypes, List<SubViewShow> displaySubViewTypes, SubViewShow firstDisplaySubViewShow)
    {
        this.subViewTypes = subViewTypes;
        this.displaySubViewTypes = displaySubViewTypes;
        views = new Dictionary<SubViewShow, IView>();
        for (int i = 0; i < displaySubViewTypes.Count; i++)
        {
            viewLoader.
        }
    }
    void ISubViewsLocator.SwitchSubView(SubViewShow subViewShow) { }
    
    private async UniTask<IView> ShowViewAsync(Type type)
    {
        IViewLocator viewLocator;
        if (!viewLoader.TryGetActiveView(type, out IView view))
        {
            if (!viewLoader.TryGetPoolView(type, out view))
            {
                view = await viewLoader.CreateView(type);
                GameObject goView = view.GameObject();
                viewLocator = subViewDisplayContainer.AddSubViewLocator(goView);
                viewLocator.Bind(view);
                view.BindLocator(viewLocator);
                
                RectTransform windowRt = goView.GetComponent<RectTransform>();
                windowRt.SetParent(this.transform);
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
            uniqueViewDict.Remove(oldUniqueId);
            viewLocator.Hide();
        }
        int uniqueId = UniqueIdGenerator.Default.Create();
        MainViewCheckGenerator.Default.Add(type, uniqueId);
        uniqueTypeDict.Add(uniqueId, type);
        
        uniqueViewDict.Add(uniqueId, view);
        viewLocator = view.GetLocator();
        viewLocator.BindUniqueId(uniqueId);
        viewLocator.GameObject().transform.SetAsLastSibling();
        viewLocator.Show();
        return view;
    }
}
