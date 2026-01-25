using UnityEngine;
using UnityEngine.UI;

public class LoadingView : ViewBase<LoadingViewModel>
{
    [SerializeField] private Slider sliderProgress;

    protected override void BindUI()
    {
    }

    protected override void UnBindUI()
    {
    }
}
