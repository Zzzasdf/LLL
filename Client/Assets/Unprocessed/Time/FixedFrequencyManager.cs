using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定频更新
/// CPU 优化点：1、记录一次当前时间，固定频率修正 2、定频分类
/// GC 优化点：记录类型的显示的数字字符串
/// </summary>
public sealed partial class FixedFrequencyManager : IFixedFrequencyManager
{
    private long CurrentTime => GameEntry.SystemTime.CurrentUnixTimeMilliseconds();

    private const int IntervalSecond = 1;
    private float lastUpdateTime;

    // 自增 id
    private int incrementId;
    private Dictionary<int, FixedFrequencySpan> dict;

    void IInit.OnInit()
    {
        dict ??= new Dictionary<int, FixedFrequencySpan>();
    }

    void IReset.OnReset()
    {
        foreach (var pair in dict)
        {
            pair.Value.FixedHandler.Invoke(pair.Key, CurrentTime);
            ((IDisposable)pair.Value).Dispose();
        }

        dict.Clear();
    }

    void IDestroy.OnDestroy()
    {
        foreach (var pair in dict)
        {
            ((IDisposable)pair.Value).Dispose();
        }

        dict.Clear();
    }

    void IUpdate.OnUpdate(float deltaTime)
    {
        if (lastUpdateTime >= CurrentTime) return;
        lastUpdateTime = CurrentTime + IntervalSecond;
        foreach (var pair in dict)
        {
            FixedFrequencySpan span = pair.Value;
            span.FixedHandler.Invoke(pair.Key, CurrentTime);
        }
    }

    /// <summary>
    /// 创建定频更新
    /// </summary>
    /// <param name="endMilliseconds">结束的时间（毫秒）</param>
    /// <param name="fixedHandler">触发的定频更新回调 (int => 唯一 id, long => 当前时间（毫秒）)</param>
    /// <returns></returns>
    public int SetEndMilliseconds(long endMilliseconds, Action<int, long> fixedHandler)
    {
        int id = CreateUniqueId();
        FixedFrequencySpan span = FixedFrequencySpan.Get(id, endMilliseconds, fixedHandler);
        dict.Add(id, span);
        return id;
    }

    /// <summary>
    /// 取消定频更新
    /// </summary>
    /// <param name="id">唯一 id</param>
    public void Cancel(int id)
    {
        if (!dict.Remove(id, out FixedFrequencySpan span)) return;
        span.FixedHandler.Invoke(id, CurrentTime);
        ((IDisposable)span).Dispose();
    }

    /// 生成唯一 id
    private int CreateUniqueId()
    {
        do
        {
            incrementId++;
        } while (dict.ContainsKey(incrementId));

        return incrementId;
    }

    private class FixedFrequencySpan : IDisposable
    {
        private static readonly MonitoredObjectPool.ObjectPool<FixedFrequencySpan, FixedFrequencySpan> s_Pool =
            new(nameof(FixedFrequencySpan), () => new FixedFrequencySpan(),
                null,
                t =>
                {
                    t.FixedHandler = null;
                    t.EndMilliseconds = 0;
                    t.Id = 0;
                });

        public static FixedFrequencySpan Get(int id, long endMilliseconds, Action<int, long> fixedHandler)
        {
            FixedFrequencySpan span = s_Pool.Get();
            span.Id = id;
            span.EndMilliseconds = endMilliseconds;
            span.FixedHandler = fixedHandler;
            return span;
        }

        private FixedFrequencySpan()
        {
        }

        void IDisposable.Dispose()
        {
            s_Pool.Release(this);
        }

#if POOLED_EXCEPTION
        ~FixedFrequencySpan() => s_Pool.FinalizeDebug();
#endif

        public int Id { get; private set; }
        public long EndMilliseconds { get; private set; }
        public Action<int, long> FixedHandler { get; private set; }
    }
}
