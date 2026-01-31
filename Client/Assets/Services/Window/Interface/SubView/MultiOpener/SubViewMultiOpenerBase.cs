using UnityEngine;

public abstract class SubViewMultiOpenerBase: MonoBehaviour
{
    [SerializeField] private RectTransform rtPanelParent;

    public RectTransform GetRtPanelParent() => rtPanelParent;
}
