using UnityEngine;
using UnityEngine.UI;

public class HelpView : ViewBase<HelpViewModel>
{
    [SerializeField] private Button btnClose;

    private void Start()
    {
        BindUI();
    }

    private void BindUI()
    {
        btnClose.onClick.AddListener(()=> viewModel.CloseCommand.Execute(this));
    }
    
    private void OnDestroy()
    {
        // 清理按钮事件
        btnClose.onClick.RemoveAllListeners();
    }
}
