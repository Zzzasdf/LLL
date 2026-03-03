using System;

public interface IReusableRecorder
{
    void ReRegisterForFinalize();
    void SuppressFinalize();
}
public class ReusableRecorder<T>: IReusableRecorder
{
    void IReusableRecorder.ReRegisterForFinalize() => GC.ReRegisterForFinalize(this);
    void IReusableRecorder.SuppressFinalize() => GC.SuppressFinalize(this);
    ~ReusableRecorder()
    {
        UnityEngine.Debug.LogError($"pool item gc eg!! {typeof(T)} => 当前对象被销毁，代码中存在未回收该类型的地方");
    }
}
public class ReusableRecorder : IReusableRecorder
{
    private Type type;
    public ReusableRecorder(Type type) => this.type = type;
    void IReusableRecorder.ReRegisterForFinalize() => GC.ReRegisterForFinalize(this);
    void IReusableRecorder.SuppressFinalize() => GC.SuppressFinalize(this);
    ~ReusableRecorder()
    {
        UnityEngine.Debug.LogError($"pool item gc eg!! {type} => 当前对象被销毁，代码中存在未回收该类型的地方");
    }
}

public interface IReusableDisposable : IReusable, IDisposable
{
    
}
public interface IReusable
{
    IReusableRecorder ReusableRecorder { get; set; }
}