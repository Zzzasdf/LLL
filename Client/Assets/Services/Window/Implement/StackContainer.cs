using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StackContainer : MonoBehaviour, IWindowContainer
{
    private RectTransform traThis;
    
    private IWindowService windowService;
    private WindowLayer windowLayer;
    private int warmCapacity;

    private ILogService logService;
    
    private Stack<IWindow> container;
    
    public IWindowContainer Init(IWindowService windowService, WindowLayer windowLayer, int warmCapacity, ILogService logService = null)
    {
        traThis = GetComponent<RectTransform>();
        this.windowService = windowService;
        this.windowLayer = windowLayer;
        this.warmCapacity = warmCapacity;
        this.logService = logService;
        
        container = new Stack<IWindow>();
        return this;
    }

    RectTransform IWindowContainer.GetTransform() => traThis;

    UniTask IWindowContainer.AddAsync<T>(T window)
    {
        if (container.Count == warmCapacity)
        {
            logService.Warning($"{windowLayer} {nameof(StackContainer)} 的容量已超过预警值：{warmCapacity}");
        }
        container.Push(window);
        return UniTask.CompletedTask;
    }

    UniTask<bool> IWindowContainer.RemoveAsync(IWindow window)
    {
        if (container.Count == 0)
        {
            logService.Warning($"{windowLayer} {nameof(StackContainer)} 的数量为 0， HideAsync 无效");
            return UniTask.FromResult(false);
        }
        if (window != container.Peek())
        {
            logService.Warning($"{windowLayer} {nameof(StackContainer)} 的 Peek 对象非请求关闭的 {nameof(window)}， HideAsync 无效");
            return UniTask.FromResult(false);
        }
        container.Pop();
        return UniTask.FromResult(true);
    }
}
