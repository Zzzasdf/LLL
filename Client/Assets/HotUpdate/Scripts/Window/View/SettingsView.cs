using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : ViewBase<SettingsViewModel>
{
    [SerializeField] private TextMeshProUGUI lbContent;
    [SerializeField] private Button btnClose;

    protected override void BindUI()
    {
        lbContent.SetText(((IView)this).GetUniqueId().ToString());
        btnClose.onClick.AddListener(()=> viewModel.CloseCommand.Execute(this));
    }
    
    protected override void UnBindUI()
    {
        // 清理按钮事件
        btnClose.onClick.RemoveAllListeners();
    }
}
