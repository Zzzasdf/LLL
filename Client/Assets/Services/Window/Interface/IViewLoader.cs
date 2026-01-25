using System;
using Cysharp.Threading.Tasks;

public interface IViewLoader
{
    bool TryGetActiveView(Type type, out IView view);
    bool TryGetPoolView(Type type, out IView view);
    UniTask<IView> CreateView(Type type);
    void ReleaseView(IView view);
}
