using System.Collections.Generic;
using CommunityToolkit.Mvvm.DependencyInjection;
using Cysharp.Threading.Tasks;

public class QueueContainer : ILayerContainer
{
    private ILogService logService;
    
    private ViewLayer viewLayer;
    private int capacity;
    
    private Queue<IView> container;

    private IViewService viewService;
    private LayerLocator layerLocator;
    
    public QueueContainer(ILogService logService)
    {
        this.logService = logService;
    }
    
    public ILayerContainer BindParam(ViewLayer viewLayer, int capacity)
    {
        this.viewLayer = viewLayer;
        this.capacity = capacity;

        container = capacity > 0 ? new Queue<IView>(capacity) : new Queue<IView>();
        return this;
    }
    
    void ILayerContainer.BindService(IViewService viewService)
    {
        this.viewService = viewService;
    }
    
    void ILayerContainer.BindLocator(LayerLocator layerLocator)
    {
        this.layerLocator = layerLocator;
    }

    async UniTask ILayerContainer.AddAsync<T>(T window)
    {
        if (container.Count == capacity)
        {
            await viewService.HideAsync(container.Peek());
        }
        container.Enqueue(window);
    }

    UniTask<bool> ILayerContainer.RemoveAsync(IView view)
    {
        if (container.Count == 0)
        {
            logService.Warning($"{viewLayer} {nameof(QueueContainer)} 的数量为 0， HideAsync 无效");
            return UniTask.FromResult(false);
        }
        if (view != container.Peek())
        {
            logService.Warning($"{viewLayer} {nameof(QueueContainer)} 的 Peek 对象非请求关闭的 {nameof(view)}， HideAsync 无效");
            return UniTask.FromResult(false);
        }
        container.Dequeue();
        return UniTask.FromResult(true);
    }
}
