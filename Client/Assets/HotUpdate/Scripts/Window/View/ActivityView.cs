using UnityEngine;
using UnityEngine.UI;

public class ActivityView : ViewBase<ActivityViewModel>
{
    [SerializeField] private Button btnClose;
    
    public override void InitUI(object viewCheckValue)
    {
    }

    public override void DestroyUI()
    {
    }

    public override void BindUI()
    {
        btnClose.onClick.AddListener(() => viewModel.CloseCommand.Execute(this));
    }

    public override void UnBindUI()
    {
        btnClose.onClick.RemoveAllListeners();
    }
}
