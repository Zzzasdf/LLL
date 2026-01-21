using Cysharp.Threading.Tasks;

public interface ILayerContainer
{
    void BindService(IViewService viewService);
    void BindLocator(ILayerLocator layerLocator);
    UniTask AddAsync<T>(T view) where T: class, IView;
    UniTask<bool> RemoveAsync<T>(T view) where T: class, IView;
    UniTask<bool> TryPop(out IView view);
}
