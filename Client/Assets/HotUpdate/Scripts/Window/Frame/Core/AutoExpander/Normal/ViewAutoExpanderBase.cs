using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.DependencyInjection;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ViewAutoExpanderBase: IViewLayerCore
{
    public IReusableRecorder ReusableRecorder { get; set; }

    private IViewLayerDriver previousViewLayerViewDriver;
    private IViewLayerDriver viewLayerDriver;
    private IViewDriver viewDriver;
    private ViewNode viewNode;
    
    private IEntityLoader<Type, ViewDriverBase> subViewDriverLoader;
    private Dictionary<int, ViewNode> subViewNodes;
    private Dictionary<ViewNode, IViewDriver> subViewDrivers;
    private Dictionary<ViewNode, IViewLayerDriver> subViewLayerDrivers;

    void IViewLayerCore.Init(IViewLayerDriver previousViewLayerViewDriver, IViewLayerDriver viewLayerDriver, IViewDriver viewDriver, ViewNode viewNode)
    {
        this.previousViewLayerViewDriver = previousViewLayerViewDriver;
        this.viewLayerDriver = viewLayerDriver;
        this.viewDriver = viewDriver;
        this.viewNode = viewNode;

        subViewDriverLoader = Ioc.Default.GetRequiredService<ViewDriverLoader>();
        subViewNodes ??= new Dictionary<int, ViewNode>();
        subViewDrivers ??= new Dictionary<ViewNode, IViewDriver>();
        subViewLayerDrivers ??= new Dictionary<ViewNode, IViewLayerDriver>();
    }
    void IDisposable.Dispose()
    {
        previousViewLayerViewDriver = null;
        viewDriver = null;
        viewNode = null;

        subViewDriverLoader = null;
        subViewNodes.Clear();
        subViewDrivers.Clear();
        subViewLayerDrivers.Clear();
    }

    async UniTask<bool> IViewLayerCore.ShowViewAsync(Stack<ViewNode> stack)
    {
        ViewNode viewNode = stack.Pop();
        List<ViewNode> subViewNodes = viewNode.Nexts;
        if (subViewNodes == null || subViewNodes.Count == 0) return false;
        bool isExistingPeek = true;
        for (int i = 0; i < subViewNodes.Count; i++)
        {
            ViewNode subViewNode = subViewNodes[i];
            if (subViewDrivers.ContainsKey(subViewNode)) continue;
            if (stack.Count > 0 && subViewNode == stack.Peek())
            {
                await ShowViewAsync_Internal(subViewNode, stack);
                isExistingPeek = false;
            }
            else
            {
                IViewConfigure viewConfigure = subViewNode.Value;
                IViewCheck viewCheck = viewConfigure.ViewCheck;
                if (viewCheck == null || viewCheck.IsFuncOpen())
                {
                    Stack<ViewNode> nextStack = new Stack<ViewNode>();
                    nextStack.Push(subViewNode);
                    await ShowViewAsync_Internal(subViewNode, nextStack);
                }
            }
        }
        if (isExistingPeek && stack.Count > 0 && subViewLayerDrivers.TryGetValue(stack.Peek(), out IViewLayerDriver subViewLayerDriver))
        {
            viewLayerDriver.CheckChildCount(subViewNodes.Count);
            return await subViewLayerDriver.ShowViewAsync(stack);
        }
        viewLayerDriver.CheckChildCount(subViewNodes.Count);
        return true;
    }


    private async UniTask<bool> ShowViewAsync_Internal(ViewNode subViewNode, Stack<ViewNode> stack)
    {
        Type subViewDriverType = subViewNode.Value.ViewDriverType;
        IViewDriver subViewDriver = await subViewDriverLoader.Get(subViewDriverType);
        ((ViewDriverBase)subViewDriver).transform.name = $"------Driver------";
        subViewDrivers.Add(subViewNode, subViewDriver);
        
        RectTransform rtDriver = subViewDriver.RtThis;
        rtDriver.SetParent(viewLayerDriver.RtChildNodeParent);
        rtDriver.localPosition = Vector3.zero;
        rtDriver.localScale = Vector3.one;
        rtDriver.anchoredPosition = Vector2.zero;
        rtDriver.anchorMin = Vector2.zero;
        rtDriver.anchorMax = Vector2.one;
        rtDriver.offsetMin = Vector2.zero;
        rtDriver.offsetMax = Vector2.zero;
        rtDriver.SetAsLastSibling();
        
        int uniqueId = await subViewDriver.Show(subViewNode);
        subViewNodes.Add(uniqueId, subViewNode);

        Type sub2ViewLayerCoreType = subViewNode.Value.SubViewLayerCoreType;
        if (sub2ViewLayerCoreType != null)
        {
            Type sub2ViewLayerDriverType = subViewNode.Value.SubViewLayerDriverType;
            IEntityLoader<Type, ViewLayerDriverBase> sub2ViewLayerDriverLoader = Ioc.Default.GetRequiredService<ViewLayerDriverLoader>();
            IViewLayerDriver sub2ViewLayerDriver = await sub2ViewLayerDriverLoader.Get(sub2ViewLayerDriverType);
            ((ViewLayerDriverBase)sub2ViewLayerDriver).transform.name = $"------LayerDriver------";
            ((ViewLayerDriverBase)sub2ViewLayerDriver).transform.SetParent(subViewDriver.RtChildNodeParent);
            RectTransform rtLayerDriver = sub2ViewLayerDriver.RtChildNodeParent;
            rtLayerDriver.localPosition = Vector3.zero;
            rtLayerDriver.localScale = Vector3.one;
            rtLayerDriver.anchoredPosition = Vector2.zero;
            rtLayerDriver.anchorMin = Vector2.zero;
            rtLayerDriver.anchorMax = Vector2.one;
            rtLayerDriver.offsetMin = Vector2.zero;
            rtLayerDriver.offsetMax = Vector2.zero;
            rtLayerDriver.SetAsLastSibling();
            
            sub2ViewLayerDriver.CreateCore(viewLayerDriver, subViewDriver, subViewNode);
            subViewLayerDrivers.Add(subViewNode, sub2ViewLayerDriver);
            await sub2ViewLayerDriver.ShowViewAsync(stack);
        }
        return true;
    }
    
    bool IViewLayerCore.HideView(Stack<ViewNode> stack)
    {
        ViewNode viewNode = stack.Pop();
        if (stack.Count > 0)
        {
            ViewNode subViewNode = stack.Peek();
            if (subViewLayerDrivers.TryGetValue(subViewNode, out IViewLayerDriver subViewLayerDriver))
            {
                return subViewLayerDriver.HideView(stack);
            }
            if (subViewDrivers.ContainsKey(subViewNode))
            {
                viewLayerDriver.HideView(subViewNode);
                viewLayerDriver.CheckChildCount(subViewNodes.Count);
                return true;
            }
        }
        else if(viewNode == this.viewNode)
        {
            if (previousViewLayerViewDriver != null)
            {
                previousViewLayerViewDriver.HideView(viewNode);
                previousViewLayerViewDriver.CheckChildCount(subViewNodes.Count);
            }
            return true;
        }
        viewLayerDriver.CheckChildCount(subViewNodes.Count);
        return false;
    }
    
    void IViewLayerCore.HideView(ViewNode subViewNode)
    {
        if (subViewLayerDrivers.TryGetValue(subViewNode, out IViewLayerDriver subViewLayerDriver))
        {
            subViewLayerDriver.HideAllSubLayerView();
            IEntityLoader<Type, ViewLayerDriverBase> sub2ViewLayerDriverLoader = Ioc.Default.GetRequiredService<ViewLayerDriverLoader>();
            sub2ViewLayerDriverLoader.Release(subViewLayerDriver.GetType(), (ViewLayerDriverBase)subViewLayerDriver);
            subViewLayerDrivers.Remove(subViewNode);
        }
        
        IViewDriver subViewDriver = subViewDrivers[subViewNode];
        subViewDriver.Hide();
        
        subViewNodes.Remove(subViewDriver.UniqueId);
        
        subViewDriverLoader.Release(subViewDriver.GetType(), (ViewDriverBase)subViewDriver);
        subViewDrivers.Remove(subViewNode);
    }

    void IViewLayerCore.HideAllSubLayerView()
    {
        List<ViewNode> subViewNodes = this.subViewNodes.Values.ToList();
        for (int i = 0; i < subViewNodes.Count; i++)
        {
            ViewNode subViewNode = subViewNodes[i];
            viewLayerDriver.HideView(subViewNode);
        }
    }

    public void CheckSubViewCount(int count)
    {
    }


    void IViewLayerCore.PushHide()
    {
    }

    void IViewLayerCore.Push()
    {
    }

    void IViewLayerCore.Pop()
    {
    }

    // void IViewLayerContainer.Bind(IViewLayerDriver viewLayerDriver)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // UniTask<(IView view, int? removeId)> IViewLayerContainer.ShowViewAndTryRemoveAsync(Type type)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // UniTask<List<int>> IViewLayerContainer.PopViewAndTryRemove(List<int> popIds)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // List<int> IViewLayerContainer.HideViewTryPop(int uniqueId)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // void IViewLayerContainer.HideAllView()
    // {
    //     throw new NotImplementedException();
    // }
    //
    // void IViewLayerContainer.HideAllActivateView()
    // {
    //     throw new NotImplementedException();
    // }
    //
    // void IViewLayerContainer.HideAllStashView()
    // {
    //     throw new NotImplementedException();
    // }
    //
    // void IViewLayerContainer.Stash(int uniqueId)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // bool IViewLayerContainer.TryStashPop(int uniqueId, out List<int> popIds)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // void IViewLayerContainer.StashClear(int uniqueId)
    // {
    //     throw new NotImplementedException();
    // }

}
