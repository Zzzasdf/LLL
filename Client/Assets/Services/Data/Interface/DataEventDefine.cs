using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

public class DataSaveAccountLevelAsyncRequestEvent: AsyncRequestMessage<bool>
{
    public Type Type { get; }
    public IAccountLevelModel Model { get; }
    public DataSaveAccountLevelAsyncRequestEvent(Type type, IAccountLevelModel model)
    {
        Type = type;
        Model = model;
    }
}
public class DataSaveRoleLevelAsyncRequestEvent: AsyncRequestMessage<bool>
{
    public Type Type { get; }
    public object Model { get; }
    public DataSaveRoleLevelAsyncRequestEvent(Type type, object model)
    {
        Type = type;
        Model = model;
    }
}
public class DataClearRoleLevelAsyncRequestEvent: AsyncRequestMessage<bool>
{
}

