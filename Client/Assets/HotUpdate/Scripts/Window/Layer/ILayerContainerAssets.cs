// using System;
// using Cysharp.Threading.Tasks;
// using UnityEngine;
//
// public interface ILayerContainerAssets
// {
//     void BindLocator(ILayerLocator layerLocator);
//     UniTask<IView> ShowViewAsync<TViewLocator>(Type type) where TViewLocator : MonoBehaviour, IViewLocator;
//     bool TryPopView(int uniqueId);
//     void HideView(int uniqueId, int? popId);
//     IView GetView(int uniqueId);
// }
