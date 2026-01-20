using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class PopupContainer : ILayerContainer
{
    private ILogService logService;
    
    private ViewLayer viewLayer;
    private int warmCapacity;
    
    private Dictionary<int, IView> container;
    
    private IViewService viewService;
    private LayerLocator layerLocator;

    public PopupContainer(ILogService logService)
    {
        this.logService = logService;
    }
    
    public ILayerContainer BindParam(ViewLayer viewLayer, int warmCapacity)
    {
        this.viewLayer = viewLayer;
        this.warmCapacity = warmCapacity;
        
        container = new Dictionary<int, IView>();
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

    UniTask ILayerContainer.AddAsync<T>(T window)
    {
        if (container.Count == warmCapacity)
        {
            logService.Warning($"{viewLayer} {nameof(StackContainer)} 的容量已超过预警值：{warmCapacity}");
        }
        container.Add(window.GetUniqueId(), window);
        return UniTask.CompletedTask;
    }

    UniTask<bool> ILayerContainer.RemoveAsync(IView view)
    {
        if (container.Count == 0)
        {
            logService.Warning($"{viewLayer} {nameof(PopupContainer)} 的数量为 0， HideAsync 无效");
            return UniTask.FromResult(false);
        }
        int uniqueId = view.GetUniqueId();
        if (container.Remove(uniqueId))
        {
            logService.Warning($"{viewLayer} {nameof(QueueContainer)} 的 Peek 对象非请求关闭的 {nameof(view)}， HideAsync 无效");
            return UniTask.FromResult(false);
        }
        container.Remove(uniqueId);
        return UniTask.FromResult(true);
    }
}
