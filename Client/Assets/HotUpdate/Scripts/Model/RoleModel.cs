public class RoleModel
{
    private AccountRoleSimpleModel accountRoleSimpleModel;
    public RoleModel Bind(AccountRoleSimpleModel accountRoleSimpleModel)
    {
        this.accountRoleSimpleModel = accountRoleSimpleModel;
        return this;
    }
    
    public long GetId() => accountRoleSimpleModel.Id;
    public string GetName() => accountRoleSimpleModel.Name;
    public void SetName(string value) => accountRoleSimpleModel.Name = value;
    public int GetLevel() => accountRoleSimpleModel.Level;
    public void SetLevel(int value) => accountRoleSimpleModel.Level = value;

    public int Test;
}
