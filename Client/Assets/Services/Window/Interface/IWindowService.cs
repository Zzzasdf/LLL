using Cysharp.Threading.Tasks;

public interface IWindowService
{
    UniTask<T> ShowAsync<T>(WindowLayer windowLayer) where T: class, IWindow;
    UniTask<bool> HideAsync(IWindow window);
}
