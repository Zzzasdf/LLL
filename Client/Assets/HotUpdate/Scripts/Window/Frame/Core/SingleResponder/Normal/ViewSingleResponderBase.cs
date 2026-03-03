// using System;
// using System.Collections.Generic;
// using CommunityToolkit.Mvvm.DependencyInjection;
// using Cysharp.Threading.Tasks;
// using UnityEngine;
//
// public class ViewSingleResponderBase<TViewLoader>: IViewLayerCore
//     where TViewLoader: class, IEntityLoader<Type, ViewEntityBase>, IViewLoader
// {
//     public IReusableRecorder ReusableRecorder { get; set; }
//
//     private IViewDriver previousViewDriver { get; set; }
//     
//     private IViewDriver viewDriver;
//     private IEntityLoader<Type, ViewEntityBase> viewLoader;
//
//     private Dictionary<int, ViewNode> nextViewNodes;
//     private Dictionary<ViewNode, IViewDriver> nextViewDrivers;
//     
//     void IViewLayerCore.Init(IViewDriver previousViewDriver, IViewDriver viewDriver)
//     {
//         this.previousViewDriver = previousViewDriver;
//         this.viewDriver = viewDriver;
//         viewLoader = Ioc.Default.GetRequiredService<TViewLoader>();
//         nextViewNodes ??= new Dictionary<int, ViewNode>();
//         nextViewDrivers ??= new Dictionary<ViewNode, IViewDriver>();
//     }
//     void IDisposable.Dispose()
//     {
//         viewDriver = null;
//         viewLoader = null;
//         nextViewNodes.Clear();
//         nextViewDrivers.Clear();
//     }
//
//     async UniTask<bool> IViewLayerCore.ShowViewAsync(Stack<ViewNode> stack)
//     {
//         ViewNode viewNode = stack.Pop();
//         List<ViewNode> nextViewNodes = viewNode.Nexts;
//         if (nextViewNodes == null || nextViewNodes.Count == 0) return true;
//         for (int i = 0; i < nextViewNodes.Count; i++)
//         {
//             ViewNode nextNode = nextViewNodes[i];
//             if (nextViewDrivers.ContainsKey(nextNode)) continue;
//             if (stack.Count > 0 && nextNode == stack.Peek())
//             {
//                 await ShowViewAsync(nextNode, stack);
//             }
//             else
//             {
//              
//                 await ShowViewWithCheckAsync(nextNode);
//             }
//         }
//         return true;
//     }
//     private async UniTask<bool> ShowViewWithCheckAsync(ViewNode nextViewNode)
//     {
//         IViewConfigure viewConfigure = nextViewNode.Value;
//         IViewCheck viewCheck = viewConfigure.ViewCheck;
//         if (viewCheck != null && !viewCheck.IsFuncOpen())
//         {
//             return false;
//         }
//         Stack<ViewNode> nextStack = new Stack<ViewNode>();
//         nextStack.Push(nextViewNode);
//         return await ShowViewAsync(nextViewNode, nextStack);
//     }
//     private async UniTask<bool> ShowViewAsync(ViewNode nextViewNode, Stack<ViewNode> stack)
//     {
//         IViewConfigure subViewConfigure = nextViewNode.Value;
//         Type type = subViewConfigure.Type;
//         
//         IViewDriver subViewDriver;
//         if (!viewLoader.TryGetFromActive(type, out ViewEntityBase view))
//         {
//             if (!viewLoader.TryGetFromPool(type, out view))
//             {
//                 view = await viewLoader.Get(type);
//                 subViewDriver = view.gameObject.AddComponent<ViewDriver>();
//             }
//             else
//             {
//                 subViewDriver = view.gameObject.GetComponent<ViewDriver>();
//             }
//             nextViewDrivers.Add(nextViewNode, subViewDriver);
//         }
//         else
//         {
//             int oldUniqueId = view.UniqueId;
//             nextViewNodes.Remove(oldUniqueId);
//             viewDriver.HideView(nextViewNode);
//             subViewDriver = view.gameObject.GetComponent<IViewDriver>();
//         }
//         int uniqueId = UniqueIdGenerator.Default.Create();
//         view.UniqueId = uniqueId;
//         subViewDriver.BindView(viewDriver, view, nextViewNode);
//         nextViewNodes.Add(uniqueId, nextViewNode);
//         
//         RectTransform windowRt = view.GetComponent<RectTransform>();
//         windowRt.SetParent(this.viewDriver.RtChildNodeParent);
//         windowRt.localPosition = Vector3.zero;
//         windowRt.localScale = Vector3.one;
//         windowRt.anchoredPosition = Vector2.zero;
//         windowRt.anchorMin = Vector2.zero;
//         windowRt.anchorMax = Vector2.one;
//         windowRt.offsetMin = Vector2.zero;
//         windowRt.offsetMax = Vector2.zero;
//         windowRt.SetAsLastSibling();
//         
//         return await subViewDriver.ShowViewAsync(stack);
//     }
//     
//     void IViewLayerCore.HideView(Stack<ViewNode> stack)
//     {
//         Stack<IViewDriver> viewDrivers = new Stack<IViewDriver>();
//         stack.Pop();
//         viewDrivers.Push(viewDriver);
//         viewDriver.SetSubViewDriver(viewDrivers, stack);
//         while (stack.Count > 0)
//         {
//             // 从父节点删除自己
//             IViewDriver viewParentDriver = viewDrivers.Pop().PreviousViewDriver;
//             if (viewParentDriver == null) return;
//             if (!viewParentDriver.HideViewAfterSelfHide(viewDriver.ViewNode))
//             {
//                 return;
//             }
//         }
//     }
//     void IViewLayerCore.SetSubViewDriver(in Stack<IViewDriver> viewDrivers, Stack<ViewNode> stack)
//     {
//         if (stack.Count == 0) return;
//         ViewNode subViewNode = stack.Pop();
//         if (!nextViewDrivers.TryGetValue(subViewNode, out IViewDriver subViewDriver))
//         {
//             LLogger.FrameError($"该节点不存在 {subViewNode}");
//             return;
//         }
//         viewDrivers.Push(subViewDriver);
//         subViewDriver.SetSubViewDriver(viewDrivers, stack);
//     }
//     void IViewLayerCore.HideView(ViewNode viewNode)
//     {
//         IViewDriver viewDriver = nextViewDrivers[viewNode];
//         nextViewNodes.Remove(viewDriver.View.UniqueId);
//         nextViewDrivers.Remove(viewNode);
//         viewLoader.Release(viewNode.Value.Type, viewDriver.View);
//     }
//     bool IViewLayerCore.HideViewAfterSelfHide(ViewNode viewNode)
//     {
//         IViewDriver viewDriver = nextViewDrivers[viewNode];
//         nextViewNodes.Remove(viewDriver.View.UniqueId);
//         nextViewDrivers.Remove(viewNode);
//         viewLoader.Release(viewNode.Value.Type, viewDriver.View);
//         return nextViewNodes.Count > 0;
//     }
//
//     public void CheckSubViewCount(int count)
//     {
//         throw new NotImplementedException();
//     }
//
//
//     void IViewLayerCore.PushHide()
//     {
//     }
//
//     void IViewLayerCore.Push()
//     {
//     }
//
//     void IViewLayerCore.Pop()
//     {
//     }
// }
