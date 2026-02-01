using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelpView : ViewEntityBase<HelpViewModel>
{
    [SerializeField] private TextMeshProUGUI lbContent;
    [SerializeField] private Button btnClose;

    public override void InitUI(IViewCheck viewCheck)
    {
        lbContent.SetText(UniqueId.ToString());
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
