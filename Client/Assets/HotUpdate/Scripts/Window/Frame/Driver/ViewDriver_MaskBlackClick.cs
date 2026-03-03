using UnityEngine;

public class ViewDriver_MaskBlackClick : ViewDriver_MaskTransparentClick 
{
    protected override void Awake()
    {
        base.Awake();
        imgMask.color = new Color(0, 0, 0, 0.5f);
    }
}
