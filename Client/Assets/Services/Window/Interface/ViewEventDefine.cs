using CommunityToolkit.Mvvm.Messaging.Messages;

public class ViewShowAsyncRequestEvent: AsyncRequestMessage<bool>
{
    public ViewType ViewType { get; }
    public ViewShowAsyncRequestEvent(ViewType viewType)
    {
        ViewType = viewType;
    }
}

public class ViewHideAsyncRequestEvent: AsyncRequestMessage<bool>
{
    public ViewEntityBase View { get; }
    public ViewHideAsyncRequestEvent(ViewEntityBase view)
    {
        View = view;
    }
}

public class ViewAllHideAsyncRequestEvent
{
}

