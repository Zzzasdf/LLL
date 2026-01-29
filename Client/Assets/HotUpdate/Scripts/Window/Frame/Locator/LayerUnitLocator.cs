using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LayerUnitLocator : MonoBehaviour, ILayerLocator
{
    private ILayerContainer layerContainer;
    private ViewLayer viewLayer;
    private IViewLoader viewLoader;

    private IUICanvasLocator uiCanvasLocator;
    private RectTransform thisRt;
    
    [SerializeField]
    private SerializableDictionary<int, Type> uniqueTypeDict;
    [SerializeField]
    private SerializableDictionary<int, IView> uniqueViewDict;
    
    void ILayerLocator.Build(ILayerContainer layerContainer, IUICanvasLocator uiCanvasLocator)
    {
        this.layerContainer = layerContainer;
        viewLayer = layerContainer.GetViewLayer();
        viewLoader = layerContainer.GetViewLoader();
        
        this.uiCanvasLocator = uiCanvasLocator;
        thisRt = gameObject.GetComponent<RectTransform>();
        if (thisRt == null)
        {
            thisRt = gameObject.AddComponent<RectTransform>();
        }

        uniqueTypeDict = new SerializableDictionary<int, Type>();
        uniqueViewDict = new SerializableDictionary<int, IView>();
        
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

    async UniTask<IView> ILayerLocator.ShowViewAsync(Type type)
    {
        IViewLocator viewLocator;
        if (!viewLoader.TryGetActiveView(type, out IView view))
        {
            if (!viewLoader.TryGetPoolView(type, out view))
            {
                view = await viewLoader.CreateView(type);
                GameObject goView = view.GameObject();
                viewLocator = layerContainer.AddViewLocator(goView);
                viewLocator.Bind(viewLayer, view);
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
            viewLocator = view.GetLocator();
            int oldUniqueId = viewLocator.GetUniqueId();
            uniqueViewDict.Remove(oldUniqueId);
            viewLocator.Hide();
        }
        int uniqueId = UniqueIdGenerator.Default.Create();
        ViewCheckGenerator.Default.Add(type, uniqueId);
        uniqueTypeDict.Add(uniqueId, type);
        
        uniqueViewDict.Add(uniqueId, view);
        CheckUniqueViewDictCount();
        viewLocator = view.GetLocator();
        viewLocator.BindUniqueId(uniqueId);
        viewLocator.GameObject().transform.SetAsLastSibling();
        viewLocator.Show();
        return view;
    }

    async UniTask<bool> ILayerLocator.TryPopViewAsync(List<int> uniqueIds)
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
            if (uniqueViewDict.TryGetValue(uniqueId, out IView view))
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
        if (!viewLoader.TryGetActiveView(type, out IView view))
        {
            if (!viewLoader.TryGetPoolView(type, out view))
            {
                view = await viewLoader.CreateView(type);
                GameObject goView = view.GameObject();
                viewLocator = layerContainer.AddViewLocator(goView);
                viewLocator.Bind(viewLayer, view);
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
            viewLocator = view.GetLocator();
            int oldUniqueId = viewLocator.GetUniqueId();
            uniqueViewDict.Remove(oldUniqueId);
            viewLocator.Hide();
        }
        uniqueViewDict.Add(uniqueId, view);
        CheckUniqueViewDictCount();
        viewLocator = view.GetLocator();
        viewLocator.BindUniqueId(uniqueId);
        viewLocator.GameObject().transform.SetAsLastSibling();
        viewLocator.Show();
        return true;
    }

    void ILayerLocator.HideView(int uniqueId)
    {
        if (!uniqueTypeDict.Remove(uniqueId))
        {
            LLogger.Error($"请求关闭界面的唯一id: {uniqueId} 不存在！！");
            return;
        }
        ViewModelGenerator.Default.Delete(uniqueId);
        UniqueIdGenerator.Default.Delete(uniqueId);
        ViewCheckGenerator.Default.Delete(uniqueId);

        if (uniqueViewDict.Remove(uniqueId, out IView view))
        {
            IViewLocator viewLocator = view.GetLocator();
            viewLocator.Hide();
            viewLoader.ReleaseView(view);
        }
        CheckUniqueViewDictCount();
    }

    void ILayerLocator.PushHideView(int uniqueId)
    {
        if (!uniqueViewDict.Remove(uniqueId, out IView view))
        {
            CheckUniqueViewDictCount();
            return;
        }
        CheckUniqueViewDictCount();
        IViewLocator viewLocator = view.GetLocator();
        viewLocator.Hide();
        viewLoader.ReleaseView(view);
    }

    private void CheckUniqueViewDictCount()
    {
        CheckUniqueViewDictCount(uniqueViewDict.Count);
    }
    protected virtual void CheckUniqueViewDictCount(int count)
    {
        
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
