using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

public class ViewShowAsyncRequestEvent: AsyncRequestMessage<IView>
{
    public Type Type { get; }
    public ViewShowAsyncRequestEvent(Type type)
    {
        Type = type;
    }
}
public class ViewHideAsyncRequestEvent: AsyncRequestMessage<bool>
{
    public IView View { get; }
    public ViewHideAsyncRequestEvent(IView view)
    {
        View = view;
    }
}
public class ViewAllHideAsyncRequestEvent: AsyncRequestMessage<bool>
{
}

