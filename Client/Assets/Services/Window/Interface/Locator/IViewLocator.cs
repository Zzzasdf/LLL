using UnityEngine;

public interface IViewLocator
{
    void Bind(ViewLayer viewLayer, IView view);
    ViewLayer GetLayer();

    void BindUniqueId(int uniqueId);
    int GetUniqueId();
    
    GameObject GameObject();

    void Show();
    void Hide();

    void SetFirstSubView(SubViewType subViewType);
    void SwitchSubView(SubViewType subViewType);
}
