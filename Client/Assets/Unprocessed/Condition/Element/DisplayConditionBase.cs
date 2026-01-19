// using System;
// using UnityEngine;
//
// namespace ConditionModule.Element
// {
//     [Serializable]
//     public abstract class DisplayConditionBase<T> : DisplayCollectorItemBase
//         where T : DisplayConditionBase<T> 
//     {
//         private DisplayCollector _displayCollector;
//         private DisplayCollector displayCollector 
//         {
//             get {
//                 if (_displayCollector == null)
//                 {
//                     _displayCollector = GetComponent<DisplayCollector>();
//                 }
//                 return _displayCollector;
//             }
//         }
//         private PooledList<PooledHashSet<ICondition<EConditionType>>> subScribeEvents;
//         private int flagIndex;
//         private Action<bool> fireCallback;
//
//         protected virtual void Awake()
//         {
//             subScribeEvents = PooledList<PooledHashSet<ICondition<EConditionType>>>.Get();
//             fireCallback = _ => displayCollector.SetStatus();
//         }
//         protected virtual void OnDestroy()
//         {
//             foreach (var subScribeEvent in subScribeEvents)
//             {
//                 foreach (var item in subScribeEvent)
//                 {
//                     GameEntry.ConditionManager.Unsubscribe(item, fireCallback);
//                 }
//                 ((IDisposable)subScribeEvent).Dispose();
//             }
//             ((IDisposable)subScribeEvents).Dispose();
//         }
//
//         protected T AppendSubscribe(ICondition<EConditionType> eConditionType)
//         {
//             for (int i = subScribeEvents.Count; i < flagIndex + 1; i++)
//             {
//                 subScribeEvents.Add(PooledHashSet<ICondition<EConditionType>>.Get());
//             }
//             PooledHashSet<ICondition<EConditionType>> subScribeEvent = subScribeEvents[flagIndex];
//             if (subScribeEvent.Contains(eConditionType))
//             {
//                 Debug.LogError($"重复订阅 {eConditionType}");
//                 return this as T;
//             }
//             subScribeEvent.Add(eConditionType);
//             GameEntry.ConditionManager.Subscribe(eConditionType, fireCallback);
//             return this as T;
//         }
//         protected T AppendSeparate()
//         {
//             flagIndex++;
//             return this as T;
//         }
//
//         public override bool Status()
//         {
//             if (subScribeEvents.Count == 0) return true;
//             foreach (var subScribeEvent in subScribeEvents)
//             {
//                 bool result = true;
//                 foreach (var item in subScribeEvent)
//                 {
//                     if (!GameEntry.ConditionManager.Status(item))
//                     {
//                         result = false;
//                         break;
//                     }
//                 }
//                 if (result)
//                 {
//                     return true;
//                 }
//             }
//             return false;
//         }
//     }
// }