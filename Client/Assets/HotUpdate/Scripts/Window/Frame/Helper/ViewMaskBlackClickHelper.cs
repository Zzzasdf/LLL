using UnityEngine;

public class ViewMaskBlackClickHelper : ViewMaskTransparentClickHelper
{
    protected override void Awake()
    {
        base.Awake();
        imgMask.color = new Color(0, 0, 0, 0.5f);
    }
}
