using System;
using System.Collections.Generic;
using ConditionModule.Element;
using Google.Protobuf.Collections;

namespace ConditionModule.Integration
{
    public partial class ConditionManager: IManager
    {
        private IResponder<RepeatedField<uint>, uint> repeatedFieldResponder;
        private IResponder<Test, EConditionType> testResponder;
        
        void IInit.OnInit()
        {
            repeatedFieldResponder = new RepeatedFieldResponder();
            testResponder = new TestResponder();
            SubscribeFireAll();
        }
        void IReset.OnReset()
        {
            UnsubscribeFireAll();
            testResponder.Reset();
            repeatedFieldResponder.Reset();
        }
        void IDestroy.OnDestroy()
        {
            testResponder.Destroy();
            testResponder = null;
            repeatedFieldResponder.Destroy();
            repeatedFieldResponder = null;
        }

#region RepeatedField
        public void Subscribe(RepeatedField<uint> key, Action<RepeatedField<uint>, bool> callback) => repeatedFieldResponder.Subscribe(key, callback);
        public void Unsubscribe(RepeatedField<uint> key, Action<RepeatedField<uint>, bool> callback) => repeatedFieldResponder.Unsubscribe(key, callback);
        public bool Status(RepeatedField<uint> key) => repeatedFieldResponder.Status(key);
                
        private void FireByRepeatedField(uint eConditionType) => repeatedFieldResponder.Fire(eConditionType);
        private void FireByRepeatedField() => repeatedFieldResponder.Fire();
                
        private void FireNowByRepeatedField(uint eConditionType) => repeatedFieldResponder.FireNow(eConditionType);
        private void FireNowByRepeatedField() => repeatedFieldResponder.FireNow();
#endregion

#region Test
        public void Subscribe(Test key, Action<Test, bool> callback) => testResponder.Subscribe(key, callback);
        public void Unsubscribe(Test key, Action<Test, bool> callback) => testResponder.Unsubscribe(key, callback);
        public bool Status(Test key) => testResponder.Status(key);
                        
        private void FireByTest(EConditionType eConditionType) => testResponder.Fire(eConditionType);
        private void FireByTest() => testResponder.Fire();
                        
        private void FireNowByTest(EConditionType eConditionType) => testResponder.FireNow(eConditionType);
        private void FireNowByTest() => testResponder.FireNow();
#endregion

        private class RepeatedFieldResponder: ResponderBase<RepeatedField<uint>, uint>
        {
            public override void GetConditionTypes(ICollection<uint> types)
            {
                int index = 0;
                foreach (var item in types)
                {
                    if ((index & 1) == 0)
                    {
                        types.Add(item);
                    }
                    index++;
                }
            }
            
            public override bool CheckCondition(RepeatedField<uint> t)
            {
                // TODO ZZZ
                return true;
            }
        }

        private class TestResponder : ResponderBase<Test, EConditionType>
        {
            public override void GetConditionTypes(ICollection<EConditionType> types)
            {
                throw new NotImplementedException();
            }

            public override bool CheckCondition(Test t)
            {
                throw new NotImplementedException();
            }
        }

        public class Test
        {
            
        }
    }
}