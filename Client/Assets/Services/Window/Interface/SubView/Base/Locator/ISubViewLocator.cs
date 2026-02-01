using UnityEngine;

public interface ISubViewLocator: IEntityLocator
{
    void Bind(ViewLayer viewLayer, IView view, IViewCheck viewCheck);
    void Bind(IView view, IViewCheck viewCheck);
    ViewLayer GetLayer();

    void BindUniqueId(int uniqueId);
    int GetUniqueId();
    
    GameObject GameObject();

    void Show();
    void Hide();
}
