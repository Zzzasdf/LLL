using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class QueueContainer : MonoBehaviour, IWindowContainer
{
    private RectTransform traThis;
    
    private IWindowService windowService;
    private WindowLayer windowLayer;
    private int capacity;
    private ILogService logService;
    
    private Queue<IWindow> container;
    
    public IWindowContainer Init(IWindowService windowService, WindowLayer windowLayer, int capacity, ILogService logService = null)
    {
        traThis = GetComponent<RectTransform>();
        this.windowService = windowService;
        this.windowLayer = windowLayer;
        this.capacity = capacity;
        this.logService = logService;

        container = capacity > 0 ? new Queue<IWindow>(capacity) : new Queue<IWindow>();
        return this;
    }
    
    RectTransform IWindowContainer.GetTransform() => traThis;
    
    async UniTask IWindowContainer.AddAsync<T>(T window)
    {
        if (container.Count == capacity)
        {
            await windowService.HideAsync(container.Peek());
        }
        container.Enqueue(window);
    }

    UniTask<bool> IWindowContainer.RemoveAsync(IWindow window)
    {
        if (container.Count == 0)
        {
            logService.Warning($"{windowLayer} {nameof(QueueContainer)} 的数量为 0， HideAsync 无效");
            return UniTask.FromResult(false);
        }
        if (window != container.Peek())
        {
            logService.Warning($"{windowLayer} {nameof(QueueContainer)} 的 Peek 对象非请求关闭的 {nameof(window)}， HideAsync 无效");
            return UniTask.FromResult(false);
        }
        container.Dequeue();
        return UniTask.FromResult(true);
    }
}
