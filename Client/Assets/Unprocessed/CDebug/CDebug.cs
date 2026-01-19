using System;
using System.Diagnostics;

public static class CDebug
{
    private const string ColorFrameLog = "#DD2090";
    private const string ColorNonMainThread = "#FFC107"; // 非主线程
    private static readonly string FramePrefix = $" [<color={ColorFrameLog}>Frame</color>] ";

    private static string FrameFormat()
    {
        return FramePrefix + TimeFormat();
    }
    private static string TimeFormat()
    {
        int currentManagedThreadId = Environment.CurrentManagedThreadId;
        string threadIdStr = $"ThreadId: {currentManagedThreadId}";
        if (currentManagedThreadId != 1)
        {
            threadIdStr = $"<color={ColorNonMainThread}>{threadIdStr}</color>";
        }
        return string.Join(' ', DateTime.Now.ToString(), threadIdStr, "\n{0}");
    }

#region Frame
    [Conditional("FRAME_LOG")]
    public static void FrameLog(object message)
    {
        UnityEngine.Debug.Log(FrameFormat() + message);
    }
    [Conditional("FRAME_WARNING")]
    public static void FrameWarming(object message)
    {
        UnityEngine.Debug.LogWarning(FrameFormat() + message);
    }
    [Conditional("FRAME_ERROR")]
    public static void FrameError(object message)
    {
        UnityEngine.Debug.LogError(FrameFormat() + message);
    }
#endregion

#region Game
    [Conditional("LOG")]
    public static void Log(object message)
    {
        UnityEngine.Debug.Log(TimeFormat() + message);
    }
    [Conditional("WARNING")]
    public static void Warming(object message)
    {
        UnityEngine.Debug.LogWarning(TimeFormat() + message);
    }
    [Conditional("ERROR")]
    public static void Error(object message)
    {
        UnityEngine.Debug.LogError(TimeFormat() + message);
    }
#endregion
}
