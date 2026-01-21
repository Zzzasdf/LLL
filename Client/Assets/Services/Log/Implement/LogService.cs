using UnityEngine;

public class LogService : ILogService
{
    void ILogService.Debug(string message)
    {
        Debug.Log(message);
    }

    void ILogService.Warning(string message)
    {
       Debug.LogWarning(message);
    }

    void ILogService.Error(string message)
    {
        Debug.LogError(message);
    }
}
