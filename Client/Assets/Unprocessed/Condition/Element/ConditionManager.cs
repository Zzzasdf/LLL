using System;
using System.Collections.Generic;

namespace ConditionModule.Element
{
    public partial class ConditionManager: IManager
    {
        /// 条件响应器
        private IResponder<Test, EConditionType> testResponder;
        
        void IInit.OnInit()
        {
            testResponder = new Responder<Test, EConditionType>();
            SubscribeFireAll();
        }
        void IReset.OnReset()
        {
            UnsubscribeFireAll();
            testResponder.Reset();
        }
        void IDestroy.OnDestroy()
        {
            testResponder.Destroy();
            testResponder = null;
        }
        
#region Test
        public void Subscribe(Test key, Action<Test, bool> callback) => testResponder.Subscribe(key, callback);
        public void Unsubscribe(Test key, Action<Test, bool> callback) => testResponder.Unsubscribe(key, callback);
        public bool Status(Test key) => testResponder.Status(key);
                        
        private void FireByTest(EConditionType eConditionType) => testResponder.Fire(eConditionType);
        private void FireByTest() => testResponder.Fire();
                        
        private void FireNowByTest(EConditionType eConditionType) => testResponder.FireNow(eConditionType);
        private void FireNowByTest() => testResponder.FireNow();
#endregion
    }
    
    public enum EConditionType
    {
        Test1 = 1,
        Test2 = 2,
        Test3 = 3,
    }
    
    public class Test: ICondition<EConditionType>
    {
        void ICondition<EConditionType>.GetConditionTypes(ICollection<EConditionType> types)
        {
            throw new NotImplementedException();
        }

        bool ICondition<EConditionType>.CheckCondition()
        {
            throw new NotImplementedException();
        }
    }
}