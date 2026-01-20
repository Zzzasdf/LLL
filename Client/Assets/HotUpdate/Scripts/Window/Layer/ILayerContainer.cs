using Cysharp.Threading.Tasks;

public interface ILayerContainer
{
    void BindService(IViewService viewService);
    void BindLocator(LayerLocator layerLocator);
    UniTask AddAsync<T>(T window) where T: class, IView;
    UniTask<bool> RemoveAsync(IView view);
}
