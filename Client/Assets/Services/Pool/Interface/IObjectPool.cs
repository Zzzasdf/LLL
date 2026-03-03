using System;

public interface IObjectPool
{
    T Get<T>() where T : class, IReusable, new();
    IReusable Get(Type reusableType);

    void Release<T>(T toRelease) where T : class, IReusable;
    void Release(IReusable toRelease);
}
