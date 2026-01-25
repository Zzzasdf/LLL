using System;
using Cysharp.Threading.Tasks;

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

    public class ConfirmAgainViewEvent
    {
        public string Content { get; }
        public Func<UniTask> ConfirmFunc { get; }
        public ConfirmAgainViewEvent(string content, Func<UniTask> confirmFunc)
        {
            Content = content;
            ConfirmFunc = confirmFunc;
        }
    }
}
