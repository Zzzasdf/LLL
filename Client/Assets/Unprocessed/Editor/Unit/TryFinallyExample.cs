using System.Diagnostics;
using NUnit.Framework;
using Debug = UnityEngine.Debug;

[TestFixture]
public class TryFinallyExample
{ 
    const int Iterations = 1; // 1亿次迭代
    
    [Test]
    public void Foo()
    {
        Debug.Log("性能测试开始...");
        Debug.Log($"测试迭代次数: {Iterations:N0}");
        
        // 测试不使用 try-finally 的性能
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
        {
            WithoutTryFinally();
        }
        stopwatch.Stop();
        Debug.Log($"不使用 try-finally: {stopwatch.ElapsedMilliseconds} ms");
        
        // 测试使用 try-finally 的性能
        stopwatch.Restart();
        for (int i = 0; i < Iterations; i++)
        {
            WithTryFinally();
        }
        stopwatch.Stop();
        Debug.Log($"使用 try-finally:    {stopwatch.ElapsedMilliseconds} ms");
        
        Debug.Log("\n测试完成");
    }
    
    // 不使用 try-finally 的方法
    void WithoutTryFinally()
    {
        // List<int> l = ListPool<int>.Get();
        // l.Add(1);
        // ListPool<int>.Release(l); 
        
        // var l = PooledList<int>.Get();
        // l.Add(1);
        // ((IDisposable)l).Dispose();
    }
    
    // 使用 try-finally 的方法
    void WithTryFinally()
    {
        // using (var l = PooledList<int>.Get())
        // {
        //     l.Add(1);
        //     using (var l2 = PooledList<int>.Get())
        //     {
        //         l2.Add(1);
        //     } 
        // } 
    }
}
