using UnityEngine;

public class WindowBase : MonoBehaviour, IWindow
{
    private WindowLayer windowLayer;
    private int uniqueId;
    private GameObject go;
    
    void IWindow.Init(WindowLayer windowLayer, int uniqueId)
    {
        this.windowLayer = windowLayer;
        this.uniqueId = uniqueId;
    }

    WindowLayer IWindow.GetLayer() => windowLayer;
    int IWindow.GetUniqueId() => uniqueId;
    GameObject IWindow.GameObject() => go;
}
