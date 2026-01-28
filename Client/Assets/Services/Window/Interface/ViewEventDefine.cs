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
public class ViewAKAShowAsyncRequestEvent: AsyncRequestMessage<bool>
{
    public SubViewAKA SubViewAKA { get; }
    public ViewAKAShowAsyncRequestEvent(SubViewAKA subViewAka)
    {
        SubViewAKA = subViewAka;
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

