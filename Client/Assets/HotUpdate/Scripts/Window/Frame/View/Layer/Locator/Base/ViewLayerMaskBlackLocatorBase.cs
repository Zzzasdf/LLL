using System;
using UnityEngine;

public class ViewLayerMaskBlackLocatorBase<TViewLayerContainer, TViewLoader, TViewLocator> : ViewLayerRaycastBlockingLocatorBase<TViewLayerContainer, TViewLoader, TViewLocator>
    where TViewLayerContainer: class, IViewLayerContainer
    where TViewLoader: class, IEntityLoader<Type, ViewEntityBase>, IViewLoader
    where TViewLocator: MonoBehaviour, IViewLocator
{
    protected override void Awake()
    {
        base.Awake();
        imgMask.color = new Color(0, 0, 0, 0.5f);
    }
}
