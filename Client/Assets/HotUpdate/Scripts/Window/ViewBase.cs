using UnityEngine;

public abstract class ViewBase : MonoBehaviour, IView
{
    private ViewLayer viewLayer;
    private int uniqueId;
    
    void IView.Init(ViewLayer viewLayer, int uniqueId)
    {
        this.viewLayer = viewLayer;
        this.uniqueId = uniqueId;
    }

    ViewLayer IView.GetLayer() => viewLayer;
    int IView.GetUniqueId() => uniqueId;
    GameObject IView.GameObject() => gameObject;
}
