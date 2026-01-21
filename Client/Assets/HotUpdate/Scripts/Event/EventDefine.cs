public class EventDefine
{
    public class SelectedRoleArchiveEvent
    {
        public AccountRoleSimpleModel AccountRoleSimpleModel { get; }
        
        public SelectedRoleArchiveEvent(AccountRoleSimpleModel accountRoleSimpleModel)
        {
            AccountRoleSimpleModel = accountRoleSimpleModel;
        }
    }
}
