using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PopupContainer : MonoBehaviour, IWindowContainer
{
    private RectTransform traThis;
    
    private IWindowService windowService;
    private WindowLayer windowLayer;
    private int warmCapacity;
    private ILogService logService;
    
    private Dictionary<int, IWindow> container;
    
    public IWindowContainer Init(IWindowService windowService, WindowLayer windowLayer, int warmCapacity, ILogService logService = null)
    {
        traThis = GetComponent<RectTransform>();
        this.windowService = windowService;
        this.windowLayer = windowLayer;
        this.warmCapacity = warmCapacity;
        this.logService = logService;
        
        container = new Dictionary<int, IWindow>(warmCapacity);
        return this;
    }
    
    RectTransform IWindowContainer.GetTransform() => traThis;

    UniTask IWindowContainer.AddAsync<T>(T window)
    {
        if (container.Count == warmCapacity)
        {
            logService.Warning($"{windowLayer} {nameof(StackContainer)} 的容量已超过预警值：{warmCapacity}");
        }
        container.Add(window.GetUniqueId(), window);
        return UniTask.CompletedTask;
    }

    UniTask<bool> IWindowContainer.RemoveAsync(IWindow window)
    {
        if (container.Count == 0)
        {
            logService.Warning($"{windowLayer} {nameof(PopupContainer)} 的数量为 0， HideAsync 无效");
            return UniTask.FromResult(false);
        }
        int uniqueId = window.GetUniqueId();
        if (container.Remove(uniqueId))
        {
            logService.Warning($"{windowLayer} {nameof(QueueContainer)} 的 Peek 对象非请求关闭的 {nameof(window)}， HideAsync 无效");
            return UniTask.FromResult(false);
        }
        container.Remove(uniqueId);
        return UniTask.FromResult(true);
    }
}
