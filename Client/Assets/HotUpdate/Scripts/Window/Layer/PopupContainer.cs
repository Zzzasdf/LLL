using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class PopupContainer : ILayerContainer
{
    private ILogService logService;
    
    private ViewLayer viewLayer;
    private int warmCapacity;
    
    private Dictionary<int, IView> container;
    
    private IViewService viewService;
    private ILayerLocator layerLocator;

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
    
    void ILayerContainer.BindLocator(ILayerLocator layerLocator)
    {
        this.layerLocator = layerLocator;
    }

    UniTask ILayerContainer.AddAsync<T>(T view) => AddAsync_Internal(view);
    private UniTask AddAsync_Internal<T>(T view) where T: class, IView
    {
        if (container.Count == warmCapacity)
        {
            logService.Warning($"{viewLayer} {nameof(StackContainer)} 的容量已超过预警值：{warmCapacity}");
        }
        container.Add(view.GetUniqueId(), view);
        return UniTask.CompletedTask;
    }

    UniTask<bool> ILayerContainer.RemoveAsync<T>(T view) => RemoveAsync_Internal(view);
    private UniTask<bool> RemoveAsync_Internal<T>(T view) where T: class, IView
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
    
    UniTask<bool> ILayerContainer.TryPop(out IView view) => TryPop_Internal(out view);
    private UniTask<bool> TryPop_Internal(out IView view)
    {
        view = null;
        if (container.Count == 0)
        {
            return UniTask.FromResult(false);
        }
        using (Dictionary<int, IView>.Enumerator enumerator = container.GetEnumerator())
        {
            view = enumerator.Current.Value;
        }
        return UniTask.FromResult(true);
    }
}
