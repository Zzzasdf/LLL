using UnityEngine;
using UnityEngine.UI;

public class HelpView : ViewBase<HelpViewModel>
{
    [SerializeField] private Button btnClose;

    protected override void BindUI()
    {
        btnClose.onClick.AddListener(()=> viewModel.CloseCommand.Execute(this));
    }
    
    protected override void UnBindUI()
    {
        // 清理按钮事件
        btnClose.onClick.RemoveAllListeners();
    }
}
