using System.Collections.Generic;

public class AccountModel: IAccountLevelModel
{
    public List<AccountRoleModel> accountRoleModels;
    public int selectedIndex;

    public int SelectedIndex() => selectedIndex;

    public AccountRoleModel GetSelectedRoleArchive()
    {
        return accountRoleModels[selectedIndex];
    }
}

public class AccountRoleModel
{
}
