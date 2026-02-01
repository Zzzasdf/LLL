using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public partial class ViewService:
    IRecipient<ViewSubShowAsyncRequestEvent>
{
    private async UniTask<bool> ShowAsync_Internal(SubViewShow subViewShow)
    {
        if (subViewShows.TryGetValue(subViewShow, out IViewConfigure viewConfigure))
        {
            if (viewConfigure.TryGetSubViewCheck(subViewShow, out IViewCheck subViewCheck))
            {
                if (!subViewCheck.IsFuncOpenWithTip())
                {
                    return false;
                }
            }
            if (viewConfigure.TryGetViewCheck(out IViewCheck viewCheck))
            {
                if (!viewCheck.IsFuncOpenWithTip())
                {
                    return false;
                }
            }
            IView view = await ShowMainAsync_Internal(viewConfigure);
            AddSubViewLocator(view, viewConfigure, subViewShow);
            return true;
        }
        return false;
    }

    private void AddSubViewLocator(IView view, IViewConfigure viewConfigure, SubViewShow? firstSubViewShow = null)
    {
        ISubViewLayerLocator subViewLayerLocator = viewConfigure.AddSubViewsLocator(view.GameObject());
        subViewLayerLocator.Init(viewConfigure, firstSubViewShow);
    }
    
    void IRecipient<ViewSubShowAsyncRequestEvent>.Receive(ViewSubShowAsyncRequestEvent message)
    {
        message.Reply(ShowAsync_Internal(message.subViewShow).AsTask());
    }
}
