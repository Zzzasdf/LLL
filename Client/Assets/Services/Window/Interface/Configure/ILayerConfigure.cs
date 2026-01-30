using System;
using UnityEngine;

public interface ILayerConfigure
{
    ILayerLocator AddLayerLocator(GameObject goLayer);
    ILayerContainer CreateLayerContainer();
    IViewLoader CreateViewLoader();
    Type GetViewLocatorType();
}
