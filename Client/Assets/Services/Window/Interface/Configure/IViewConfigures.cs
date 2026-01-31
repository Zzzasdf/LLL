using System.Collections.Generic;

public interface IViewConfigures: IEnumerable<KeyValuePair<ViewLayer, List<IViewConfigure>>>
{
    List<IViewConfigure> this[ViewLayer viewLayer]
    {
        get;
    }
}
