using UnityEngine;

public class ViewDriver_MaskBlack : ViewDriver_RaycastBlocking
{
    protected override void Awake()
    {
        base.Awake();
        imgMask.color = new Color(0, 0, 0, 0.5f);
    }
}
