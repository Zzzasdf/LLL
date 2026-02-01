using UnityEngine;

public class SubViewMultiLocator : MonoBehaviour, ISubViewMultiLocator
{
    [SerializeField] private RectTransform rtParent;
    
    RectTransform ISubViewMultiLocator.GetRtParent() => rtParent;
}
