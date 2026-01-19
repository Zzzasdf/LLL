using System;
using System.Collections.Generic;

/// <summary>
/// 定时器
/// CPU 优化点：1、记录一次当前时间，固定频率修正，2、长短间隔分离
/// </summary>
public sealed class TimerManager : ITimerManager
{
    private long CurrentTime => GameEntry.SystemTime.CurrentUnixTimeMilliseconds();

    // 自增 id
    private int incrementId;
    private SortedHeap<TimerSpan> spanSortedHeap;
    private Dictionary<int, TimerSpan> dict;

    void IInit.OnInit()
    {
        spanSortedHeap ??= new SortedHeap<TimerSpan>();
        dict ??= new Dictionary<int, TimerSpan>();
    }

    void IReset.OnReset()
    {
        foreach (var pair in dict)
        {
            pair.Value.EndHandler.Invoke(pair.Key, false);
            ((IDisposable)pair.Value).Dispose();
        }

        dict.Clear();
        spanSortedHeap.Clear();
    }

    void IDestroy.OnDestroy()
    {
        foreach (var pair in dict)
        {
            ((IDisposable)pair.Value).Dispose();
        }

        dict.Clear();
        spanSortedHeap.Clear();
    }

    void IUpdate.OnUpdate(float deltaTime)
    {
        while (!spanSortedHeap.IsEmpty)
        {
            TimerSpan span = spanSortedHeap.Peek();
            if (CurrentTime < span.EndMilliseconds) return;
            spanSortedHeap.Pop();
            dict.Remove(span.Id);
            span.EndHandler.Invoke(span.Id, true);
            ((IDisposable)span).Dispose();
        }
    }

    /// <summary>
    /// 创建定时器
    /// </summary>
    /// <param name="milliseconds">当前时间经过毫秒数后执行</param>
    /// <param name="endHandler">触发的回调 (int => 唯一id, true => 成功触发，false => 取消)</param>
    /// <returns>唯一id</returns>
    public int SetAfterMilliseconds(long milliseconds, Action<int, bool> endHandler)
    {
        long time = CurrentTime + milliseconds;
        return SetEndMilliseconds(time, endHandler);
    }

    /// <summary>
    /// 创建定时器
    /// </summary>
    /// <param name="endMilliseconds">结束的时间（毫秒）</param>
    /// <param name="endHandler">触发的回调 (int => 唯一id, true => 成功触发，false => 取消)</param>
    /// <returns>唯一id</returns>
    public int SetEndMilliseconds(long endMilliseconds, Action<int, bool> endHandler)
    {
        int id = CreateUniqueId();
        TimerSpan timerSpan = TimerSpan.Get(id, endMilliseconds, endHandler);
        spanSortedHeap.Push(timerSpan);
        dict.Add(id, timerSpan);
        return id;
    }

    /// <summary>
    /// 取消定时器
    /// </summary>
    /// <param name="id">唯一 id</param>
    public void Cancel(int id)
    {
        if (!dict.Remove(id, out TimerSpan span)) return;
        spanSortedHeap.Remove(span);
        span.EndHandler.Invoke(id, false);
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

    private class TimerSpan : IDisposable, IComparable<TimerSpan>
    {
        private static readonly MonitoredObjectPool.ObjectPool<TimerSpan, TimerSpan> s_Pool =
            new(nameof(TimerSpan), () => new TimerSpan(),
                null,
                t =>
                {
                    t.EndHandler = null;
                    t.EndMilliseconds = 0;
                    t.Id = 0;
                });

        public static TimerSpan Get(int id, long endMilliseconds, Action<int, bool> endHandler)
        {
            TimerSpan timerSpan = s_Pool.Get();
            timerSpan.Id = id;
            timerSpan.EndMilliseconds = endMilliseconds;
            timerSpan.EndHandler = endHandler;
            return timerSpan;
        }

        private TimerSpan()
        {
        }

        void IDisposable.Dispose() => s_Pool.Release(this);

#if POOLED_EXCEPTION
        ~TimerSpan() => s_Pool.FinalizeDebug();
#endif

        public int Id { get; private set; }
        public long EndMilliseconds { get; private set; }
        public Action<int, bool> EndHandler { get; private set; }

        public int CompareTo(TimerSpan other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return EndMilliseconds.CompareTo(other.EndMilliseconds);
        }
    }
}
