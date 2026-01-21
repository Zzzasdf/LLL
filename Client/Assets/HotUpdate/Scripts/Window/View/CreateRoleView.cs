using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoleView : ViewBase<CreateRoleViewModel>
{
    [SerializeField] private TMP_InputField ifName;
    [SerializeField] private Button btnCancel;
    [SerializeField] private Button btnConfirm;

    private void Start()
    {
        BindUI();
    }

    private void BindUI()
    {
        ifName.onEndEdit.AddListener(roleName => viewModel.SetRoleNameCommand.Execute(roleName));
        btnCancel.onClick.AddListener(() => viewModel.CancelCommand.Execute(null));
        btnConfirm.onClick.AddListener(() => viewModel.ConfirmCommand.Execute(null));
    }

    private void OnDestroy()
    {
        ifName.onEndEdit.RemoveAllListeners();
        btnCancel.onClick.RemoveAllListeners();
        btnConfirm.onClick.RemoveAllListeners();
    }
}
