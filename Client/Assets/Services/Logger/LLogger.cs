using System;
using System.Diagnostics;

public static class LLogger
{
    private const string ColorFrameLog = "#DD2090";
    private const string ColorNonMainThread = "#FFC107"; // 非主线程
    private static readonly string FramePrefix = $" [<color={ColorFrameLog}>Frame</color>] ";

    private static string FrameFormat() => FramePrefix + ThreadIdFormat();
    private static string ThreadIdFormat()
    {
        int currentManagedThreadId = Environment.CurrentManagedThreadId;
        string threadIdStr = $"ThreadId: {currentManagedThreadId}";
        if (currentManagedThreadId != 1)
        {
            threadIdStr = $"<color={ColorNonMainThread}>{threadIdStr}</color>";
        }
        return string.Join(' ', threadIdStr, "\n{0}");
    }

#region Frame
    [Conditional("FRAME_LOG")]
    public static void FrameLog(object message) => UnityEngine.Debug.LogFormat(FrameFormat(), message);
    [Conditional("FRAME_WARNING")]
    public static void FrameWarning(object message) => UnityEngine.Debug.LogWarningFormat(FrameFormat(), message);
    [Conditional("FRAME_ERROR")]
    public static void FrameError(object message) => UnityEngine.Debug.LogErrorFormat(FrameFormat(), message);
#endregion

#region Game
    [Conditional("LOG")]
    public static void Log(object message) => UnityEngine.Debug.LogFormat(ThreadIdFormat(), message);
    [Conditional("WARNING")]
    public static void Warning(object message) => UnityEngine.Debug.LogWarningFormat(ThreadIdFormat(), message);
    [Conditional("ERROR")]
    public static void Error(object message) => UnityEngine.Debug.LogErrorFormat(ThreadIdFormat(), message);
#endregion
}