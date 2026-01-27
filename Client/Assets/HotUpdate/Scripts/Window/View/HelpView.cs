using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelpView : ViewBase<HelpViewModel>
{
    [SerializeField] private TextMeshProUGUI lbContent;
    [SerializeField] private Button btnClose;

    protected override void InitUI()
    {
        lbContent.SetText(((IView)this).GetUniqueId().ToString());
    }
    protected override void DestroyUI()
    {
    }

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
