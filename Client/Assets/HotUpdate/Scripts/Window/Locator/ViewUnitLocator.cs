using UnityEngine;
using UnityEngine.UI;

public class ViewUnitLocator: MonoBehaviour, IViewLocator
{
    protected void Awake()
    {
        gameObject.AddComponent<Canvas>();
        gameObject.AddComponent<GraphicRaycaster>();
    }
}
