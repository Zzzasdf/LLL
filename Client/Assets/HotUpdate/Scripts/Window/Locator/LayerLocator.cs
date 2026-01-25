using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.DependencyInjection;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LayerLocator : MonoBehaviour, ILayerLocator
{
    private ILayerContainer layerContainer;
    private ViewLayer viewLayer;
    private IViewLoader viewLoader;

    private IUICanvasLocator uiCanvasLocator;
    private RectTransform thisRt;
    
    private Dictionary<int, Type> uniqueTypeDict;
    private Dictionary<int, IView> uniqueViewDict;
    
    public void Build(ILayerContainer layerContainer, IUICanvasLocator uiCanvasLocator)
    {
        this.layerContainer = layerContainer;
        viewLayer = layerContainer.GetViewLayer();
        viewLoader = layerContainer.GetViewLoader();
        
        this.uiCanvasLocator = uiCanvasLocator;
        thisRt = gameObject.AddComponent<RectTransform>();

        uniqueTypeDict = new Dictionary<int, Type>();
        uniqueViewDict = new Dictionary<int, IView>();
        
        // 设置全屏拉伸
        thisRt.anchorMin = Vector2.zero;
        thisRt.anchorMax = Vector2.one;
        thisRt.offsetMin = Vector2.zero;
        thisRt.offsetMax = Vector2.zero;
        
        // 添加CanvasGroup用于控制整组UI
        CanvasGroup canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    ILayerContainer ILayerLocator.GetContainer() => layerContainer;
    IUICanvasLocator ILayerLocator.GetCanvasLocator() => uiCanvasLocator;
    Type ILayerLocator.GetViewType(int uniqueId) => uniqueTypeDict[uniqueId];
    public bool ExistInstantiate(int uniqueId) => uniqueViewDict.ContainsKey(uniqueId);

    async UniTask<IView> ILayerLocator.ShowViewAsync(Type type)
    {
        if (!viewLoader.TryGetActiveView(type, out IView view))
        {
            if (!viewLoader.TryGetPoolView(type, out view))
            {
                view = await viewLoader.CreateView(type);
                view.BindLayer(viewLayer);
                GameObject goView = view.GameObject();
                RectTransform windowRt = goView.GetComponent<RectTransform>();
                windowRt.SetParent(thisRt);
                windowRt.localPosition = Vector3.zero;
                windowRt.localScale = Vector3.one;
                windowRt.anchoredPosition = Vector2.zero;
                windowRt.anchorMin = Vector2.zero;
                windowRt.anchorMax = Vector2.one;
                windowRt.offsetMin = Vector2.zero;
                windowRt.offsetMax = Vector2.zero;
                layerContainer.AddViewLocator(goView);
            }
        }
        else
        {
            int oldUniqueId = view.GetUniqueId();
            uniqueViewDict.Remove(oldUniqueId);
            view.Hide();
        }
        int uniqueId = UniqueIdGenerator.Default.Create();
        uniqueTypeDict.Add(uniqueId, type);
        
        uniqueViewDict.Add(uniqueId, view);
        view.BindUniqueId(uniqueId);
        view.GameObject().transform.SetAsLastSibling();
        view.Show();
        return view;
    }
    
    async UniTask<bool> ILayerLocator.TryPopViewAsync(int uniqueId)
    {
        if (!uniqueTypeDict.TryGetValue(uniqueId, out Type type))
        {
            return false;
        }
        if (!viewLoader.TryGetActiveView(type, out IView view))
        {
            if (!viewLoader.TryGetPoolView(type, out view))
            {
                view = await viewLoader.CreateView(type);
                view.BindLayer(viewLayer);
                GameObject goView = view.GameObject();
                RectTransform windowRt = goView.GetComponent<RectTransform>();
                windowRt.SetParent(thisRt);
                windowRt.localPosition = Vector3.zero;
                windowRt.localScale = Vector3.one;
                windowRt.anchoredPosition = Vector2.zero;
                windowRt.anchorMin = Vector2.zero;
                windowRt.anchorMax = Vector2.one;
                windowRt.offsetMin = Vector2.zero;
                windowRt.offsetMax = Vector2.zero;
                layerContainer.AddViewLocator(goView);
            }
        }
        else
        {
            int oldUniqueId = view.GetUniqueId();
            uniqueViewDict.Remove(oldUniqueId);
        }
        uniqueViewDict.Add(uniqueId, view);
        view.BindUniqueId(uniqueId);
        view.GameObject().transform.SetAsLastSibling();
        view.Show();
        return true;
    }
    
    void ILayerLocator.HideView(int uniqueId)
    {
        if (!uniqueTypeDict.Remove(uniqueId, out Type type))
        {
            LLogger.Error($"请求关闭界面的唯一id: {uniqueId} 不存在！！");
            return;
        }
        UniqueIdGenerator.Default.Delete(uniqueId);

        if (uniqueViewDict.Remove(uniqueId, out IView view))
        {
            view.Hide();
            ViewModelGenerator.Default.Remove(uniqueId);
            viewLoader.ReleaseView(view);
        }
    }

    void ILayerLocator.PushHideView(int uniqueId)
    {
        if (!uniqueViewDict.Remove(uniqueId, out IView view))
        {
            return;
        }
        view.Hide();
        viewLoader.ReleaseView(view);
    }
}
