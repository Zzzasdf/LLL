using UnityEngine;

public class ChildNodeParent : MonoBehaviour, IChildNodeParent
{
    [SerializeField] private RectTransform childNodeParent;
    RectTransform IChildNodeParent.GetChildNodeParent() => childNodeParent;
}
