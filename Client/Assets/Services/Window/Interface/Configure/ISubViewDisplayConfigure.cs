using System;
using UnityEngine;

public interface ISubViewDisplayConfigure
{
    ISubViewCollectLocator AddSubViewDisplayLocator(GameObject goSubViewDisplay);
    ISubViewCollectContainer CreateSubViewDisplayContainer();
    IViewLoader CreateViewLoader();
    Type GetSubViewsLocatorType();
    Type GetSubViewLocatorType();
}
