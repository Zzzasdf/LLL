using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectRoleView : ViewBase<SelectRoleViewModel>
{
    [SerializeField] private List<RoleArchiveItem> roleArchiveItems;
    [SerializeField] private Button btnReturn;

    private void Start()
    {
        BindUI();
        for (int i = 0; i < roleArchiveItems.Count; i++)
        {
            RoleArchiveItem roleArchiveItem = roleArchiveItems[i];
            AccountRoleSimpleModel accountRoleSimpleModel = viewModel.GetOrAddRoleModel(i);
            roleArchiveItem.BindData(accountRoleSimpleModel);
        }
    }

    private void BindUI()
    {
        btnReturn.onClick.AddListener(()=> viewModel.ReturnCommand.Execute(null));
    }

    private void OnDestroy()
    {
        btnReturn.onClick.RemoveAllListeners();
    }
}
