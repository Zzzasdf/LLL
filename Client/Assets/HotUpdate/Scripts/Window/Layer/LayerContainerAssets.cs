// using System;
// using System.Collections.Generic;
// using Cysharp.Threading.Tasks;
// using UnityEngine;
// using YooAsset;
//
// public class LayerContainerAssets: ILayerContainerAssets
// {
//     private ViewLayer viewLayer;
//     private bool isMultiple;
//     private ILayerLocator layerLocator;
//
//     private Dictionary<Type, IView> viewDict;
//     private Dictionary<int, IView> uniqueViewDict;
//     
//     public LayerContainerAssets(ViewLayer viewLayer, bool isMultiple)
//     {
//         this.viewLayer = viewLayer;
//         this.isMultiple = isMultiple;
//         viewDict = new Dictionary<Type, IView>();
//         uniqueViewDict = new Dictionary<int, IView>();
//     }
//     
//     void ILayerContainerAssets.BindLocator(ILayerLocator layerLocator) => this.layerLocator = layerLocator;
//
//     async UniTask<IView> ILayerContainerAssets.ShowViewAsync<TViewLocator>(Type type)
//     {
//         // 资源获取
//         if (isMultiple || !viewDict.TryGetValue(type, out IView view))
//         {
//             string name = type.Name;
//             var handle = YooAssets.LoadAssetAsync<GameObject>(name);
//             await handle.Task;
//             
//             if (handle.Status != EOperationStatus.Succeed)
//             {
//                 LLogger.FrameError($"未找到该资源 {name}");
//                 return await new UniTask<IView>(default);
//             }
//             GameObject assetObject = handle.AssetObject as GameObject;
//             if (assetObject == null)
//             {
//                 LLogger.FrameError($"加载的资源不是 GameObject: {name}");
//                 return await new UniTask<IView>(default);
//             }
//             GameObject instantiatedObject = UnityEngine.Object.Instantiate(assetObject);
//             if (instantiatedObject == null)
//             {
//                 LLogger.FrameError($"实例化失败: {name}");
//                 return await new UniTask<IView>(default);
//             }
//             view = instantiatedObject.GetComponent<IView>();
//             view.BindLayer(viewLayer);
//             RectTransform windowRt = instantiatedObject.GetComponent<RectTransform>();
//             windowRt.SetParent(layerLocator.ShowView());
//             windowRt.localPosition = Vector3.zero;
//             windowRt.localScale = Vector3.one;
//             windowRt.anchoredPosition = Vector2.zero;
//             windowRt.anchorMin = Vector2.zero;
//             windowRt.anchorMax = Vector2.one;
//             windowRt.offsetMin = Vector2.zero;
//             windowRt.offsetMax = Vector2.zero;
//             instantiatedObject.AddComponent<TViewLocator>();
//             viewDict.TryAdd(type, view);
//         }
//         // 装配
//         int uniqueId = UniqueIdGenerator.Default.Create();
//         uniqueViewDict.Add(uniqueId, view);
//         view.BindUniqueId(uniqueId);
//         view.RefIncrement();
//         view.Show();
//         return view;
//     }
//
//     bool ILayerContainerAssets.TryPopView(int uniqueId)
//     {
//         if (!uniqueViewDict.TryGetValue(uniqueId, out IView view))
//         {
//             return false;
//         }
//         view.BindUniqueId(uniqueId);
//         view.Show();
//         return true;
//     }
//
//     void ILayerContainerAssets.HideView(int uniqueId, int? popId)
//     {
//         if (uniqueViewDict.Count == 0) return;
//         IView view = uniqueViewDict[uniqueId];
//         view.RefReduction();
//         if (view.GetRefCount() == 0)
//         {
//             Type type = view.GetType();
//             viewDict.Remove(type);
//             view.Hide();
//         }
//         else if(popId.HasValue)
//         {
//             view.BindUniqueId(popId.Value);
//             view.Show();
//         }
//         uniqueViewDict.Remove(uniqueId);
//         UniqueIdGenerator.Default.Delete(uniqueId);
//     }
//
//     IView ILayerContainerAssets.GetView(int uniqueId) => uniqueViewDict[uniqueId];
// }
