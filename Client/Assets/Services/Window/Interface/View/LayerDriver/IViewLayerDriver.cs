using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IViewLayerDriver
{
    RectTransform RtChildNodeParent { get; }
    void CreateCore(IViewLayerDriver previousViewLayerDriver, IViewDriver viewDriver, ViewNode viewNode);

    UniTask<bool> ShowViewAsync(Stack<ViewNode> stack);
    bool HideView(Stack<ViewNode> stack);
    void HideView(ViewNode stack);
    void HideAllSubLayerView();

    void CheckChildCount(int count);
}
