using System;
using System.Collections.Generic;

public interface IViewService
{
    void Bind(Dictionary<ViewLayer, Type> layerLocators, Dictionary<ViewLayer, List<IViewConfigure>> configures);
}
