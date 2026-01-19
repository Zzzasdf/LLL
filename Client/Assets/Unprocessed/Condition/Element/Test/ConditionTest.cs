// using ConditionModule.Element;
// using Cysharp.Threading.Tasks;
// using System;
// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEngine;
//
// public class ConditionTest : MonoBehaviour
// {
//     public class Condition: ICondition<EConditionType> 
//     {
//         public bool status;
//         public void GetConditionTypes(in ICollection<EConditionType> types)
//         {
//             types.Add(EConditionType.Test1);
//         }
//         public bool CheckCondition() => status;
//     }
//
//     private Condition condition;
//
//     [SerializeField][Header("迭代次数")] private int Iterations = 1_000_000; // 百万次迭代
//     [SerializeField][Header("性能测试 KeyCode")] private KeyCode keyCode = KeyCode.Space;
//
//     private void Awake()
//     {
//         this.GetOrAddComponent<GameEntry>();
//     }
//
//     private void Start()
//     {
//         condition = new Condition();
//         GameEntry.ConditionManager.Subscribe(condition, DebugLog);
//     }
//     private void DebugLog(bool isRight)
//     {
//         // Debug.LogError($"执行时间 => {Time.time}，状态 => {isRight}");
//     }
//
//     private void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.G))
//         {
//             GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
//             // Debug.Log("手动触发GC");
//         }
//         if (Input.GetKeyDown(keyCode))
//         {
//             PerformanceTest();
//         }
//         if (Input.GetKeyDown(KeyCode.U))
//         {
//             UNormalTest();
//         }
//         if (Input.GetKeyDown(KeyCode.S))
//         {
//             condition.status = !condition.status;
//             // Debug.LogError($"改变状态 => {condition.status}");
//         }
//     }
//
//     /// 性能测试
//     private void PerformanceTest()
//     {
//         // long before = GC.GetTotalMemory(true);
//         {
//             for (int i = 0; i < Iterations; i++)
//             {
//                 GameEntry.ConditionManager.Fire(EConditionType.Test1);
//             }
//         }
//         // long after = GC.GetTotalMemory(true);
//         // StringBuilder sb = new StringBuilder("PerformanceTest");
//         // sb.AppendLine($"GC分配总量: {after - before} bytes");
//         // sb.AppendLine($"当前时间 => {Time.time}");
//         // Debug.Log(sb.ToString());
//     }
//
//     
//     private void UNormalTest()
//     {
//         UNoramlTestAsync().Forget();
//     }
//     private async UniTaskVoid UNoramlTestAsync()
//     {
//         // await UniTask.Delay(10);
//     }
// }
