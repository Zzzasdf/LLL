using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectRoleView : ViewBase<SelectRoleViewModel>
{
    [SerializeField] private List<RoleArchiveItem> roleArchiveItems;
    [SerializeField] private Button btnReturn;

    protected override void BindUI()
    {
        for (int i = 0; i < roleArchiveItems.Count; i++)
        {
            RoleArchiveItem roleArchiveItem = roleArchiveItems[i];
            AccountRoleSimpleModel accountRoleSimpleModel = viewModel.GetOrAddRoleModel(i);
            roleArchiveItem.BindData(accountRoleSimpleModel);
        }
        
        btnReturn.onClick.AddListener(()=> viewModel.ReturnCommand.Execute(null));
    }

    protected override void UnBindUI()
    {
        btnReturn.onClick.RemoveAllListeners();
    }
}
