// using System.Collections.Generic;
// using ConditionModule.Element;
// using System.Text;
// using UnityEngine;
//
// public class ConditionTypeTest : ICondition<EConditionType>
// {
//     private List<uint> list;
//     
//     public ConditionTypeTest(List<uint> list) => this.list = list;
//     public void GetConditionTypes(in ICollection<EConditionType> types)
//     {
//         GameEntry.ConditionManager.GetConditionTypes(types, list);
//         StringBuilder sb = new StringBuilder();
//         foreach (var type in types)
//         {
//             sb.Append($"{type.ToString()}, ");
//         }
//         Debug.LogError(sb.ToString());
//     }
//     public bool CheckCondition() => GameEntry.ConditionManager.CheckCondition(list);
// }
//
// public class DisplayConditionTest : DisplayConditionBase<DisplayConditionTest>
// {
//     private ConditionTypeTest conditionTest = new ConditionTypeTest(new List<uint>
//     {
//        1,2,3,2,1,4,3,4
//     });
//     private ConditionTypeTest conditionTest2 = new ConditionTypeTest(new List<uint>
//     {
//         1,2,3,2,5,6,7,8 
//     });
//     private ConditionTypeTest conditionTest3 = new ConditionTypeTest(new List<uint>
//     {
//         1,2,3,2,1,4,3,4
//     });
//     
//     protected override void Awake()
//     {
//         new GameObject("GameEntry").AddComponent<GameEntry>();
//         base.Awake();
//     }
//
//     private void Start()
//     {
//         AppendSubscribe(conditionTest).AppendSeparate()
//             .AppendSubscribe(conditionTest2).AppendSubscribe(conditionTest3);
//         // .AppendSubscribe(conditionTest3);
//     }
// }