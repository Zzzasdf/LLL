// using CommunityToolkit.Mvvm.Messaging;
// using Cysharp.Threading.Tasks;
//
// public partial class ViewService:
//     IRecipient<ViewSubShowAsyncRequestEvent>
// {
//     private async UniTask<bool> ShowAsync_Internal(ViewType viewType)
//     {
//         if (!subViewShows.TryGetValue(viewType, out IViewConfigure viewConfigure))
//         {
//             return false;
//         }
//         if (viewConfigure.TryGetSubViewCheck(viewType, out IViewCheck subViewCheck))
//         {
//             if (!subViewCheck.IsFuncOpenWithTip())
//             {
//                 return false;
//             }
//         }
//         if (viewConfigure.TryGetViewCheck(out IViewCheck viewCheck))
//         {
//             if (!viewCheck.IsFuncOpenWithTip())
//             {
//                 return false;
//             }
//         }
//         IView view = await ShowMainAsync_Internal(viewConfigure);
//         AddSubViewLocator(view, viewConfigure, viewType);
//         return true;
//     }
//
//     private void AddSubViewLocator(IView view, IViewConfigure viewConfigure, ViewType? firstSubViewShow = null)
//     {
//         ISubViewLayerLocator subViewLayerLocator = viewConfigure.GetOrAddSubViewsLocator(view.GameObject());
//         subViewLayerLocator.Init(viewConfigure, firstSubViewShow);
//     }
//     
//     void IRecipient<ViewSubShowAsyncRequestEvent>.Receive(ViewSubShowAsyncRequestEvent message)
//     {
//         message.Reply(ShowAsync_Internal(message.viewType).AsTask());
//     }
// }
