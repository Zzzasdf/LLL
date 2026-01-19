using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Pool;

public class PooledTest : MonoBehaviour
{
    private PooledList<int> list;
    private PooledHashSet<int> hashSet;
    private PooledDictionary<int, int> dict;

    private void Awake()
    {
        list = PooledList<int>.Get();
        hashSet = PooledHashSet<int>.Get();
        dict = PooledDictionary<int, int>.Get();
    }
    private void OnDestroy()
    {
        ((IDisposable)dict).Dispose();
        ((IDisposable)hashSet).Dispose();
        ((IDisposable)list).Dispose();
    }
    
    [SerializeField][Header("迭代次数")] private int Iterations = 1_000_000; // 百万次迭代
    [SerializeField][Header("性能测试 KeyCode")] private KeyCode keyCode = KeyCode.Space;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            Debug.Log("手动触发GC");
        }
        if (Input.GetKeyDown(keyCode))
        {
            PerformanceTest();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            NormalTest();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            PNormalTest();
        }
    }

    /// 性能测试
    private void PerformanceTest()
    {
        long milliseconds;
        long before = GC.GetTotalMemory(true);
        {
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            {
                for (int i = 0; i < Iterations; i++)
                {
                    using (var l = PooledList<int>.Get())
                    {
                        l.Add(1);
                    } 
                }
            }
            stopwatch.Stop();
            milliseconds = stopwatch.ElapsedMilliseconds;
        }
        long after = GC.GetTotalMemory(true);
        StringBuilder sb = new StringBuilder("Performance");
        sb.AppendLine($"耗时 => {Iterations}次，{milliseconds}毫秒");
        sb.AppendLine($"GC分配总量: {after - before} bytes");
        Debug.Log(sb.ToString());
    }

    private void NormalTest()
    {
        long milliseconds;
        long before = GC.GetTotalMemory(true);
        {
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            {
                for (int i = 0; i < Iterations; i++)
                {
                    var l = new List<int>();
                    l.Add(1);
                }
            }
            stopwatch.Stop();
            milliseconds = stopwatch.ElapsedMilliseconds;
        }
        long after = GC.GetTotalMemory(true);
        StringBuilder sb = new StringBuilder("Normal");
        sb.AppendLine($"耗时 => {Iterations}次，{milliseconds}毫秒");
        sb.AppendLine($"GC分配总量: {after - before} bytes");
        Debug.Log(sb.ToString());
    }
    private void PNormalTest()
    {
        long milliseconds;
        long before = GC.GetTotalMemory(true);
        {
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            {
                for (int i = 0; i < Iterations; i++)
                {
                    List<int> l = ListPool<int>.Get();
                    l.Add(1);
                    ListPool<int>.Release(l);
                }
            }
            stopwatch.Stop();
            milliseconds = stopwatch.ElapsedMilliseconds;
        }
        long after = GC.GetTotalMemory(true);
        StringBuilder sb = new StringBuilder("PNormal");
        sb.AppendLine($"耗时 => {Iterations}次，{milliseconds}毫秒");
        sb.AppendLine($"GC分配总量: {after - before} bytes");
        Debug.Log(sb.ToString());
    }
}