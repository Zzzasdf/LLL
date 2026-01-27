using UnityEngine;

public class LayerMaskBlackLocator : LayerRaycastBlockingLocator
{
    protected new void Awake()
    {
        base.Awake();
        imgMask.color = new Color(0, 0, 0, 0.5f);
    }
}
