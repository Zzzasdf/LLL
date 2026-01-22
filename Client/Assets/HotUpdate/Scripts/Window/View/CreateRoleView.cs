using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoleView : ViewBase<CreateRoleViewModel>
{
    [SerializeField] private TMP_InputField ifName;
    [SerializeField] private Button btnCancel;
    [SerializeField] private Button btnConfirm;

    protected override void BindUI()
    {
        ifName.onEndEdit.AddListener(roleName => viewModel.SetRoleNameCommand.Execute(roleName));
        btnCancel.onClick.AddListener(() => viewModel.CancelCommand.Execute(null));
        btnConfirm.onClick.AddListener(() => viewModel.ConfirmCommand.Execute(null));
    }

    protected override void UnBindUI()
    {
        ifName.onEndEdit.RemoveAllListeners();
        btnCancel.onClick.RemoveAllListeners();
        btnConfirm.onClick.RemoveAllListeners();
    }
}
