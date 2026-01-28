using System;
using UnityEngine;

public interface IView
{
    void BindLayer(ViewLayer viewLayer);
    void BindLocator(IViewHelper viewHelper);
    ViewLayer GetLayer();
    ViewState GetViewState();

    void BindUniqueId(int uniqueId);
    int GetUniqueId();
    
    GameObject GameObject();

    void Show();
    void Hide();

    void SetFirstSubView(Type subViewType);
    void SetFirstSubView(SubViewAKA subViewAka);
}
