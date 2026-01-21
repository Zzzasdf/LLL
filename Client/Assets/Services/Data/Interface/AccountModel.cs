using System.Collections.Generic;

public class AccountModel: IAccountLevelModel
{
    public List<AccountRoleSimpleModel> AccountRoleSimpleModels;
    public int SelectedIndex;

    public AccountModel()
    {
        AccountRoleSimpleModels = new List<AccountRoleSimpleModel>();
    }

    public AccountRoleSimpleModel GetOrAddAccountRoleSimpleModel(int index)
    {
        for (int i = AccountRoleSimpleModels.Count - 1; i < index; i++)
        {
            AccountRoleSimpleModels.Add(new AccountRoleSimpleModel());
        }
        return AccountRoleSimpleModels[index];
    }

    public int GetSelectedIndex() => SelectedIndex;
    public void SetSelectedIndex(AccountRoleSimpleModel accountRoleSimpleModel)
    {
        for (int i = 0; i < AccountRoleSimpleModels.Count; i++)
        {
            if (accountRoleSimpleModel != AccountRoleSimpleModels[i])
            {
                continue;
            }
            SelectedIndex = i;
            return;
        }
    }

    public AccountRoleSimpleModel GetSelectedAccountRoleSimpleModel()
    {
        return AccountRoleSimpleModels[SelectedIndex];
    }
}

public class AccountRoleSimpleModel
{
    public long Id;
    public string Name;
    public int Level;
}
