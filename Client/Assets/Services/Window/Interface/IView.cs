using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IView
{
    void BindLayer(ViewLayer viewLayer);
    void BindLocator(IViewLocator viewLocator);
    ViewLayer GetLayer();
    ViewState GetViewState();

    void BindUniqueId(int uniqueId);
    int GetUniqueId();
    
    GameObject GameObject();

    void Show();
    UniTask Hide();
}
