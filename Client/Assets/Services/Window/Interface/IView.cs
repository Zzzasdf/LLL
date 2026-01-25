using UnityEngine;

public interface IView
{
    void BindLayer(ViewLayer viewLayer);
    ViewLayer GetLayer();
    
    void BindUniqueId(int uniqueId);
    int GetUniqueId();
    
    GameObject GameObject();

    void Show();
    void Hide();
}
