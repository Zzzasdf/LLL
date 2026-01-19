using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IWindowContainer
{
    RectTransform GetTransform();
    UniTask AddAsync<T>(T window) where T: class, IWindow;
    UniTask<bool> RemoveAsync(IWindow window);
}
