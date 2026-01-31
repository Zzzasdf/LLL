using UnityEngine;
using UnityEngine.UI;

public class LoadingView : ViewBase<LoadingViewModel>
{
    [SerializeField] private Slider sliderProgress;

    public override void InitUI(IViewCheck viewCheck)
    {
    }
    public override void DestroyUI()
    {
    }

    public override void BindUI()
    {
    }
    public override void UnBindUI()
    {
    }
}
