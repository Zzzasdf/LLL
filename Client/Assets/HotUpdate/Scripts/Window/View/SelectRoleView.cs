using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectRoleView : ViewEntityBase<SelectRoleViewModel>
{
    [SerializeField] private List<RoleArchiveItem> roleArchiveItems;
    [SerializeField] private Button btnReturn;
    [SerializeField] private Button btnClose;

    public override void InitUI(IViewCheck viewCheck)
    {
        for (int i = 0; i < roleArchiveItems.Count; i++)
        {
            RoleArchiveItem roleArchiveItem = roleArchiveItems[i];
            AccountRoleSimpleModel accountRoleSimpleModel = viewModel.GetOrAddRoleModel(i);
            roleArchiveItem.BindData(accountRoleSimpleModel);
        }
    }
    public override void DestroyUI()
    {
    }

    public override void BindUI()
    {
        for (int i = 0; i < roleArchiveItems.Count; i++)
        {
            RoleArchiveItem roleArchiveItem = roleArchiveItems[i];
            roleArchiveItem.BindUI();
        }
        btnReturn.onClick.AddListener(()=> viewModel.ReturnCommand.Execute(null));
        btnClose.onClick.AddListener(() => viewModel.CloseCommand.Execute(this));
    }
    public override void UnBindUI()
    {
        for (int i = 0; i < roleArchiveItems.Count; i++)
        {
            RoleArchiveItem roleArchiveItem = roleArchiveItems[i];
            roleArchiveItem.UnBindUI();
        }
        btnReturn.onClick.RemoveAllListeners();
        btnClose.onClick.RemoveAllListeners();
    }
}
