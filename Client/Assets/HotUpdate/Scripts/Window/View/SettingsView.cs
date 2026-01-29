using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : ViewBase<SettingsViewModel>
{
    [SerializeField] private TextMeshProUGUI lbContent;
    [SerializeField] private Button btnClose;

    public override void InitUI(object viewCheckValue)
    {
        lbContent.SetText(((IView)this).GetLocator().GetUniqueId().ToString());
    }
    public override void DestroyUI()
    {
    }

    public override void BindUI()
    {
        btnClose.onClick.AddListener(()=> viewModel.CloseCommand.Execute(this));
    }
    
    public override void UnBindUI()
    {
        // 清理按钮事件
        btnClose.onClick.RemoveAllListeners();
    }
}
