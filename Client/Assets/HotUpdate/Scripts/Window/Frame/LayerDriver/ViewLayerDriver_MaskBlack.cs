using UnityEngine;

public class ViewLayerDriver_MaskBlack : ViewLayerDriver_RaycastBlocking
{
    protected override void Awake()
    {
        base.Awake();
        imgMask.color = new Color(0, 0, 0, 0.5f);
    }
}
