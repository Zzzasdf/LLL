using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.DependencyInjection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ViewLayerUnitLocatorBase<TViewLayerContainer, TViewLoader, TViewLocator> : MonoBehaviour, IViewLayerLocator
    where TViewLayerContainer: class, IViewLayerContainer
    where TViewLoader: class, IEntityLoader<Type, ViewEntityBase>, IViewLoader
    where TViewLocator: MonoBehaviour, IViewLocator
{
    private RectTransform thisRt;
    private Canvas canvas;
    private GraphicRaycaster graphicRaycaster;
    
    private ViewLayer viewLayer;
    private List<IViewConfigure> viewConfigures;
    private TViewLayerContainer layerContainer;
    private IEntityLoader<Type, ViewEntityBase> viewLoader;
    
    [SerializeField]
    private SerializableDictionary<int, Type> uniqueTypeDict;
    [SerializeField]
    private SerializableDictionary<int, ViewEntityBase> uniqueViewDict;

    protected virtual void Awake()
    { 
        thisRt = gameObject.AddComponent<RectTransform>();
        
        // 设置全屏拉伸
        thisRt.anchorMin = Vector2.zero;
        thisRt.anchorMax = Vector2.one;
        thisRt.offsetMin = Vector2.zero;
        thisRt.offsetMax = Vector2.zero;
        
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
    }

    void IViewLayerLocator.Init(ViewLayer viewLayer, List<IViewConfigure> viewConfigures)
    {
        this.viewLayer = viewLayer;
        this.viewConfigures = viewConfigures;
        this.layerContainer = Ioc.Default.GetRequiredService<TViewLayerContainer>();
        this.layerContainer.Bind(this);
        this.viewLoader = Ioc.Default.GetRequiredService<TViewLoader>();
        
        uniqueTypeDict = new SerializableDictionary<int, Type>();
        uniqueViewDict = new SerializableDictionary<int, ViewEntityBase>();
    }

    List<IViewConfigure> IViewLayerLocator.GetViewConfigures() => viewConfigures;
    IViewLayerContainer IViewLayerLocator.GetContainer() => layerContainer;

    async UniTask<IView> IViewLayerLocator.ShowViewAsync(Type type)
    {
        IViewLocator viewLocator;
        RectTransform windowRt;
        if (!viewLoader.TryGetFromActive(type, out ViewEntityBase view))
        {
            if (!viewLoader.TryGetFromPool(type, out view))
            {
                view = await viewLoader.Create(type);
                viewLocator = view.gameObject.AddComponent<TViewLocator>();
                IViewCheck viewCheck = GetViewCheck(type);
                viewLocator.Bind(viewLayer, view, viewCheck);
                view.BindLocator(viewLocator);
            }
        }
        else
        {
            viewLocator = (IViewLocator)view.GetLocator();
            int oldUniqueId = viewLocator.GetUniqueId();
            uniqueViewDict.Remove(oldUniqueId);
            HideSubViews(view);
            viewLocator.Hide();
        }
        windowRt = view.GetComponent<RectTransform>();
        windowRt.SetParent(thisRt);
        windowRt.localPosition = Vector3.zero;
        windowRt.localScale = Vector3.one;
        windowRt.anchoredPosition = Vector2.zero;
        windowRt.anchorMin = Vector2.zero;
        windowRt.anchorMax = Vector2.one;
        windowRt.offsetMin = Vector2.zero;
        windowRt.offsetMax = Vector2.zero;
        
        int uniqueId = UniqueIdGenerator.Default.Create();
        uniqueTypeDict.Add(uniqueId, type);
        
        uniqueViewDict.Add(uniqueId, view);
        CheckUniqueViewDictCount();
        viewLocator = (IViewLocator)view.GetLocator();
        viewLocator.BindUniqueId(uniqueId);
        viewLocator.GameObject().transform.SetAsLastSibling();
        viewLocator.Show();
        return view;
    }

    async UniTask<bool> IViewLayerLocator.TryPopViewAsync(List<int> uniqueIds)
    {
        List<Type> types = new List<Type>();
        foreach (var uniqueId in uniqueIds)
        {
            types.Add(uniqueTypeDict[uniqueId]);
        }
        uniqueIds = viewLoader.BatchAddFilter(types, uniqueIds);

        for (int i = 0; i < uniqueIds.Count; i++)
        {
            int uniqueId = uniqueIds[i];
            if (uniqueViewDict.TryGetValue(uniqueId, out ViewEntityBase view))
            {
                view.GameObject().transform.SetAsLastSibling();
            }
            else
            {
                await TryPopViewAsync(uniqueId);
            }
        }
        return true;
    }
    private async UniTask<bool> TryPopViewAsync(int uniqueId)
    {
        if (!uniqueTypeDict.TryGetValue(uniqueId, out Type type))
        {
            return false;
        }
        IViewLocator viewLocator;
        if (!viewLoader.TryGetFromActive(type, out ViewEntityBase view))
        {
            if (!viewLoader.TryGetFromPool(type, out view))
            {
                view = await viewLoader.Create(type);
                GameObject goView = view.GameObject();
                viewLocator = goView.AddComponent<TViewLocator>();
                IViewCheck viewCheck = GetViewCheck(type);
                viewLocator.Bind(viewLayer, view, viewCheck);
                view.BindLocator(viewLocator);
                
                RectTransform windowRt = goView.GetComponent<RectTransform>();
                windowRt.SetParent(thisRt);
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
            viewLocator = (IViewLocator)view.GetLocator();
            int oldUniqueId = viewLocator.GetUniqueId();
            uniqueViewDict.Remove(oldUniqueId);
            HideSubViews(view);
            viewLocator.Hide();
        }
        uniqueViewDict.Add(uniqueId, view);
        CheckUniqueViewDictCount();
        viewLocator = (IViewLocator)view.GetLocator();
        viewLocator.BindUniqueId(uniqueId);
        viewLocator.GameObject().transform.SetAsLastSibling();
        viewLocator.Show();
        return true;
    }

    void IViewLayerLocator.HideView(int uniqueId)
    {
        if (!uniqueTypeDict.Remove(uniqueId))
        {
            LLogger.Error($"请求关闭界面的唯一id: {uniqueId} 不存在！！");
            return;
        }
        ViewModelGenerator.Default.Delete(uniqueId);
        UniqueIdGenerator.Default.Delete(uniqueId);

        if (uniqueViewDict.Remove(uniqueId, out ViewEntityBase view))
        {
            IViewLocator viewLocator = (IViewLocator)view.GetLocator();
            HideSubViews(view);
            viewLocator.Hide();
            Type type = view.GetType();
            viewLoader.Release(type, view);
        }
        CheckUniqueViewDictCount();
    }

    void IViewLayerLocator.PushHideView(int uniqueId)
    {
        if (!uniqueViewDict.Remove(uniqueId, out ViewEntityBase view))
        {
            CheckUniqueViewDictCount();
            return;
        }
        CheckUniqueViewDictCount();
        IViewLocator viewLocator = (IViewLocator)view.GetLocator();
        HideSubViews(view);
        viewLocator.Hide();
        Type type = view.GetType();
        viewLoader.Release(type, view);
    }

    private void CheckUniqueViewDictCount()
    {
        CheckUniqueViewDictCount(uniqueViewDict.Count);
    }
    protected virtual void CheckUniqueViewDictCount(int count)
    {
        
    }

    private IViewCheck GetViewCheck(Type type)
    {
        IViewCheck viewCheck = null;
        for (int i = 0; i < viewConfigures.Count; i++)
        {
            IViewConfigure viewConfigure = viewConfigures[i];
            if (viewConfigure.GetViewType() != type) continue;
            viewConfigure.TryGetViewCheck(out viewCheck);
            return viewCheck;
        }
        return viewCheck;
    }
    private void HideSubViews(ViewEntityBase view)
    {
        ISubViewLayerLocator subViewLayerLocator = view.GetComponent<ISubViewLayerLocator>();
        if (subViewLayerLocator == null) return;
        subViewLayerLocator.HideViews();
    }

    [ContextMenu("LayerContainer Content")]
    void LayerContainerDebug()
    {
        LLogger.Log(layerContainer.ToString());
    }

    [ContextMenu("ViewLoader Content")]
    void ViewLoaderDebug()
    {
        LLogger.Log(viewLoader.ToString());
    }
}