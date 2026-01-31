using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoleView : ViewBase<CreateRoleViewModel>
{
    [SerializeField] private TMP_InputField ifName;
    [SerializeField] private Button btnCancel;
    [SerializeField] private Button btnConfirm;

    public override void InitUI(IViewCheck viewCheck)
    {
    }
    public override void DestroyUI()
    {
    }

    public override void BindUI()
    {
        ifName.onEndEdit.AddListener(roleName => viewModel.SetRoleNameCommand.Execute(roleName));
        btnCancel.onClick.AddListener(() => viewModel.CancelCommand.Execute(this));
        btnConfirm.onClick.AddListener(() => viewModel.ConfirmCommand.Execute(null));
    }
    public override void UnBindUI()
    {
        ifName.onEndEdit.RemoveAllListeners();
        btnCancel.onClick.RemoveAllListeners();
        btnConfirm.onClick.RemoveAllListeners();
    }
}
