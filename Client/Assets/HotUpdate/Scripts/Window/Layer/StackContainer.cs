using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class StackContainer<TViewHelper> : ILayerContainer
    where TViewHelper: ViewHelperBase
{
    private ViewLayer viewLayer;
    private int warmCapacity;

    private Stack<IView> container;

    private IViewService viewService;
    private ILayerLocator layerLocator;

    public ILayerContainer BindParam(ViewLayer viewLayer, int warmCapacity)
    {
        this.viewLayer = viewLayer;
        this.warmCapacity = warmCapacity;
        
        container = new Stack<IView>();
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
            LLogger.FrameWarning($"{viewLayer} {nameof(StackContainer<TViewHelper>)} 的容量已超过预警值：{warmCapacity}");
        }
        container.Push(view);
        view.GameObject().AddComponent<TViewHelper>();
        return UniTask.CompletedTask;
    }

    UniTask<bool> ILayerContainer.RemoveAsync<T>(T view) => RemoveAsync_Internal(view);
    private UniTask<bool> RemoveAsync_Internal<T>(T view) where T: class, IView
    {
        if (container.Count == 0)
        {
            LLogger.FrameError($"{viewLayer} {nameof(StackContainer<TViewHelper>)} 的数量为 0， HideAsync 无效");
            return UniTask.FromResult(false);
        }
        if (view != container.Peek())
        {
            LLogger.FrameError($"{viewLayer} {nameof(StackContainer<TViewHelper>)} 的 Peek 对象非请求关闭的 {nameof(view)}， HideAsync 无效");
            return UniTask.FromResult(false);
        }
        container.Pop();
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
        view = container.Peek();
        return UniTask.FromResult(true);
    }
}
