using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public interface IViewLoader
{
    IViewLoader SetCapacity(int capacity);
    bool TryGetActiveView(Type type, out IView view);
    bool TryGetPoolView(Type type, out IView view);
    List<int> BatchAddFilter(List<Type> types, List<int> uniqueIds);
    
    UniTask<IView> CreateView(Type type);
    void ReleaseView(IView view);

    string ToString();
}
