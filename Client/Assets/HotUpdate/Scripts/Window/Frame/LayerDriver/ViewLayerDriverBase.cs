using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.DependencyInjection;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class ViewLayerDriverBase : EntityBase, IViewLayerDriver
{
    private ViewNode viewNode;

    private IPoolService poolService;
    private IViewLayerCore viewLayerCore;
    private RectTransform rtChildNodeParent;
    public RectTransform RtChildNodeParent
    {
        get
        {
            if (rtChildNodeParent == null)
            {
                rtChildNodeParent = GetComponent<RectTransform>();
            }
            return rtChildNodeParent;
        }
    }

    void IViewLayerDriver.CreateCore(IViewLayerDriver previousViewLayerDriver, IViewDriver viewDriver, ViewNode viewNode)
    {
        this.viewNode = viewNode;
        poolService ??= Ioc.Default.GetService<IPoolService>();
        if (viewLayerCore != null)
        {
            viewLayerCore.Dispose();
            poolService!.Release(viewLayerCore);
        }
        Type viewLayerCoreType = viewNode.Value.SubViewLayerCoreType;
        viewLayerCore = (IViewLayerCore)poolService!.Get(viewLayerCoreType);
        viewLayerCore.Init(previousViewLayerDriver, this, viewDriver, viewNode);
    }

    private void OnDestroy()
    {
        if (viewLayerCore == null) return;
        viewLayerCore.Dispose();
        poolService!.Release(viewLayerCore);
    }
    
    async UniTask<bool> IViewLayerDriver.ShowViewAsync(Stack<ViewNode> stack) => await viewLayerCore.ShowViewAsync(stack);

    bool IViewLayerDriver.HideView(Stack<ViewNode> stack) => viewLayerCore.HideView(stack);

    void IViewLayerDriver.HideView(ViewNode viewNode) => viewLayerCore.HideView(viewNode);
    void IViewLayerDriver.HideAllSubLayerView() => viewLayerCore.HideAllSubLayerView();

    public virtual void CheckChildCount(int count)
    {
        
    }
}
