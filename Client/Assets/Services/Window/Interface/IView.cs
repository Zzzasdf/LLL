using UnityEngine;

public interface IView
{
    void BindLocator(IViewLocator viewLocator);
    IViewLocator GetLocator();
    GameObject GameObject();
    
    void AddViewModel(int uniqueId);
    void RemoveViewModel();

    void InitUI(IViewCheck viewCheck);
    void DestroyUI();
    void BindUI();
    void UnBindUI();
}
