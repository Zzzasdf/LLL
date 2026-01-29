using UnityEngine;

public class ViewMaskBlackClickLocator : ViewMaskTransparentClickLocator
{
    protected override void Awake()
    {
        base.Awake();
        imgMask.color = new Color(0, 0, 0, 0.5f);
    }
}
