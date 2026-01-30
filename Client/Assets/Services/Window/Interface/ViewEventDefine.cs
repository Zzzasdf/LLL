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
    public SubViewShow subViewShow { get; }
    public ViewSubShowAsyncRequestEvent(SubViewShow subViewShow)
    {
        this.subViewShow = subViewShow;
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

