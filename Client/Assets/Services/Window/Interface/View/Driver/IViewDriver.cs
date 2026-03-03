using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IViewDriver: IEntityLocator
{
    RectTransform RtThis { get; }
    RectTransform RtChildNodeParent { get; }
    int UniqueId { get; }
    UniTask<int> Show(ViewNode viewNode);
    void Hide();
}
