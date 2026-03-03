using System;
using CommunityToolkit.Mvvm.Messaging;
using UnityEngine;

public partial class ViewService: MonoBehaviour, IViewService
{
    private ViewTree viewTree;
    private IViewDriver viewDriver;
    private IViewLayerDriver subViewLayerDriver;
    
    [SerializeField] private ViewModelGenerator viewModelGenerator = ViewModelGenerator.Default;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        WeakReferenceMessenger.Default.Register<ViewShowAsyncRequestEvent>(this);
        WeakReferenceMessenger.Default.Register<ViewHideAsyncRequestEvent>(this);
        WeakReferenceMessenger.Default.Register<ViewAllHideAsyncRequestEvent>(this);
    }
    
    void IViewService.Bind(IViewConfigure viewConfigure)
    {
        viewTree = new ViewTree(viewConfigure);
        Type viewDriverType = viewConfigure.ViewDriverType;
        viewDriver = (IViewDriver)gameObject.AddComponent(viewDriverType);
        
        Type subViewLayerDriverType = viewConfigure.SubViewLayerDriverType;
        subViewLayerDriver = (IViewLayerDriver)gameObject.AddComponent(subViewLayerDriverType);
        ViewNode viewNode = viewTree[ViewType.Service];
        subViewLayerDriver.CreateCore(null, viewDriver, viewNode);
    }
}