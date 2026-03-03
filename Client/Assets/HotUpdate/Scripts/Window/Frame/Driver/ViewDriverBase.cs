using System;
using CommunityToolkit.Mvvm.DependencyInjection;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract partial class ViewDriverBase : EntityBase, IViewDriver
{
    public RectTransform RtThis { get; private set; }
    public RectTransform RtChildNodeParent
    {
        get
        {
            if (view != null)
            {
                return view.RtChildNodeParent;
            }
            return GetComponent<RectTransform>();
        }
    }

    public int UniqueId => view.UniqueId;

    
    private ViewNode viewNode;
    private IEntityLoader<Type, ViewEntityBase> viewLoader;
    protected ViewEntityBase view;

    protected virtual void Awake()
    {
        RtThis = GetComponent<RectTransform>();
    }
    protected virtual void OnDestroy()
    {
        OnDestroy_Internal();
        ReleaseAsync().Forget();
    }

    async UniTask<int> IViewDriver.Show(ViewNode viewNode)
    {
        if (viewNode == this.viewNode)
        {
            return view.UniqueId;
        }
        this.viewNode = viewNode;
        IViewConfigure viewConfigure = viewNode.Value;
        Type viewLoaderType = viewConfigure.ViewLoaderType;
        viewLoader = (IEntityLoader<Type, ViewEntityBase>)Ioc.Default.GetRequiredService(viewLoaderType);
        
        Type type = viewConfigure.Type;
        view = await viewLoader.Get(type);
        int uniqueId = ViewUniqueIdGenerator.Default.Create(viewConfigure.ViewType);
        view.UniqueId = uniqueId;
        
        RectTransform windowRt = view.GetComponent<RectTransform>();
        windowRt.SetParent(RtThis);
        windowRt.localPosition = Vector3.zero;
        windowRt.localScale = Vector3.one;
        windowRt.anchoredPosition = Vector2.zero;
        windowRt.anchorMin = Vector2.zero;
        windowRt.anchorMax = Vector2.one;
        windowRt.offsetMin = Vector2.zero;
        windowRt.offsetMax = Vector2.zero;
        windowRt.SetAsLastSibling();
        InitAnimations();
        Show_Internal().Forget();
        return view.UniqueId;
    }

    void IViewDriver.Hide()
    {
        ReleaseAsync().Forget();
    }
    private async UniTask ReleaseAsync()
    {
       if (viewLoader == null) return;
       ViewUniqueIdGenerator.Default.Delete(view.UniqueId);
       IViewConfigure viewConfigure = viewNode.Value;
       Type type = viewConfigure.Type;
       viewLoader.Release(type, view);
       viewLoader = null;
       viewNode = null;
       await Hide_Internal();
       gameObject.SetActive(false);
    }
}