using System;
using Cysharp.Threading.Tasks;

public interface IViewLoader
{
    IViewLoader SetCapacity(int capacity);
    bool TryGetActiveView(Type type, out IView view);
    bool TryGetPoolView(Type type, out IView view);
    UniTask<IView> CreateView(Type type);
    void ReleaseView(IView view);

    string ToString();
}
