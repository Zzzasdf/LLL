using System;
using System.Collections.Generic;

public interface ILayerContainer
{
    void BindLocator(ILayerLocator layerLocator);
    ILayerLocator GetLocator();

    void BindGetView(Func<int, IView> getViewFunc);
    
    bool AddAndTryOutRemoveId(int uniqueId, out int removeId);
    bool RemoveAndTryPopId(int uniqueId, out int popId);
    
    void PushStorage(int uniqueId);
    bool TryPopStorage(int uniqueId, out Queue<int> storage);
}
