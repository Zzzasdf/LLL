using UnityEngine;

public interface IViewLocator
{
    void Bind(ViewLayer viewLayer, IView view);
    void Bind(IView view, IViewCheck viewCheck);
    ViewLayer GetLayer();

    void BindUniqueId(int uniqueId);
    int GetUniqueId();
    
    GameObject GameObject();

    void Show();
    void Hide();
}
