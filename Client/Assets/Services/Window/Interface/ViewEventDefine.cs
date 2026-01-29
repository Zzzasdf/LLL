using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

public class ViewShowAsyncRequestEvent: AsyncRequestMessage<bool>
{
    public Type Type { get; }
    public ViewShowAsyncRequestEvent(Type type)
    {
        Type = type;
    }
}
public class ViewSubShowAsyncRequestEvent: AsyncRequestMessage<bool>
{
    public SubViewType SubViewType { get; }
    public ViewSubShowAsyncRequestEvent(SubViewType subViewType)
    {
        SubViewType = subViewType;
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
public class ViewAllHideAsyncRequestEvent
{
}

