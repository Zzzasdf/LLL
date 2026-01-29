using UnityEngine;

public class SubViewCollectUnitLocator : MonoBehaviour, ISubViewCollectLocator
{
    private ISubViewCollectContainer subViewCollectContainer;
    private SubViewDisplay subViewDisplay;
    private IViewLoader viewLoader;
    
    private IUICanvasLocator uiCanvasLocator;
    private RectTransform thisRt;
    
    public void Build(ISubViewCollectContainer subViewCollectContainer, IUICanvasLocator uiCanvasLocator)
    {
        this.subViewCollectContainer = subViewCollectContainer;
        subViewDisplay = subViewCollectContainer.GetSubViewDisplay();
        viewLoader = subViewCollectContainer.GetViewLoader();
        
        this.uiCanvasLocator = uiCanvasLocator;
        thisRt = gameObject.GetComponent<RectTransform>();
        if (thisRt == null)
        {
            thisRt = gameObject.AddComponent<RectTransform>();
        }
    }
}
