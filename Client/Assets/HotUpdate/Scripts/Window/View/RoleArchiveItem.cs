using CommunityToolkit.Mvvm.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoleArchiveItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lbRoleId;
    [SerializeField] private TextMeshProUGUI lbName;
    [SerializeField] private TextMeshProUGUI lbLv;
    [SerializeField] private Button btnSelected;

    private AccountRoleSimpleModel accountRoleSimpleModel;
    
    private void Start()
    {
        BindUI();
    }

    public void BindData(AccountRoleSimpleModel accountRoleSimpleModel)
    {
        this.accountRoleSimpleModel = accountRoleSimpleModel;
        lbRoleId.SetText(accountRoleSimpleModel.Id.ToString());
        lbName.SetText(accountRoleSimpleModel.Name);
        lbLv.SetText(accountRoleSimpleModel.Level.ToString());
    }

    private void BindUI()
    {
        btnSelected.onClick.AddListener(OnBtnSelectedClick);
    }

    private void OnDestroy()
    {
        btnSelected.onClick.RemoveAllListeners();
    }

    private void OnBtnSelectedClick()
    {
        WeakReferenceMessenger.Default.Send(new EventDefine.SelectedRoleArchiveEvent(accountRoleSimpleModel));
    }
}
