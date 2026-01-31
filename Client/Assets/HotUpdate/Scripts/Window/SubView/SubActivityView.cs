using TMPro;
using UnityEngine;

public class SubActivityView : ViewBase<SubActivityViewModel>
{
    [SerializeField] private TextMeshProUGUI lbContent;
    
    public override void InitUI(IViewCheck viewCheck)
    {
        lbContent.SetText(viewCheck.BtnEntryName());
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
