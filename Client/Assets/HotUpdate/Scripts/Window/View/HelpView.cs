using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelpView : ViewBase<HelpViewModel>
{
    [SerializeField] private TextMeshProUGUI lbContent;
    [SerializeField] private Button btnClose;

    public override void InitUI(IViewCheck viewCheck)
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
