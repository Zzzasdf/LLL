using UnityEngine;

public interface ISubViewCollectContainer
{
    void AddSubViewContainerType(SubViewDisplay subViewDisplay);
    ISubViewCollectLocator AddLocator(GameObject goLocator);
    
    
    SubViewDisplay GetSubViewDisplay();
    IViewLoader GetViewLoader();
}
