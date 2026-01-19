// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.Pool;
// using Splitter.Mono;
// using Splitter.Core;
// using NUnit.Framework;
//
// public class DisplaySplitterExample: UnityEditor.Editor
// {
//     /// Mono 示例
//     void Mono()
//     {
//         DisplaySplitter splitter = null; // mono
//         DisplaySplitterCore core = splitter;
//         core.DisplayUnique(1);
//     }
//     
//     /// Core 示例（不回收）
//     void Core()
//     {
//         IDisplaySplitterCore core = new DisplaySplitterCore();
//         core.Subscribe(0, new List<GameObject>())
//             .Subscribe(1, new List<GameObject>());
//         // 唯一 显示
//         {
//             core.DisplayUnique(0);
//         }
//         // 多显示
//         {
//             List<int> keys = new List<int>();
//             keys.Add(0);
//             keys.Add(1);
//             core.DisplaySet(keys);
//         }
//     }
//     
//     /// Core 示例: Flags 用法（不回收）
//     void CoreFlags()
//     {
//         IDisplaySplitterCore core = new DisplaySplitterCore();
//         core.Subscribe(1 << 0, new List<GameObject>())
//             .Subscribe(1 << 1, new List<GameObject>());
//         // 唯一 显示
//         {
//             core.DisplayFlags(1 << 0);
//         }
//         // 多显示
//         {
//             core.DisplayFlags(1 << 0 | 1 << 1);
//         }
//     }
//
//     /// Core 回收 示例
//     void CoreRelease()
//     {
//         // 触发 回收 时，主要 回收 内置的 items 字典，顺带 回收 自身
//         IDisplaySplitterCore core = DisplaySplitterCore.Get();
//         // 触发 回收 时，该列表 会被回收
//         // 以及 为了标记这个回收状态 所生成出的 内置回收标记列表
//         core.Subscribe(0, ListPool<GameObject>.Get(), true) // 可选回收
//             // 触发 回收 时，该列表 不会被回收
//             .Subscribe(1, new List<GameObject>()); // 请勿将 非对象池 生成 的列表 标记为回收
//         // 唯一 显示
//         {
//             core.DisplayUnique(0);
//         }
//         // 多显示
//         {
//             List<int> keys = ListPool<int>.Get();
//             keys.Add(0);
//             keys.Add(1);
//             core.DisplaySet(keys, true); // 可选回收，调用后 直接触发，不予 Release 关联
//         }
//         // 回收
//         core.Release();
//         // 需要调用 Release 的 时候 ||
//         // 1、对象池 Get 出来的 Core
//         // 2、订阅了 标记为 回收 的 list
//     }
//
//     /// Core 回收 示例: Flags 用法
//     void CoreReleaseFlags()
//     {
//         // 触发 回收 时，主要 回收 内置的 items 字典，顺带 回收 自身
//         IDisplaySplitterCore core = DisplaySplitterCore.Get();
//         // 触发 回收 时，该列表 会被回收
//         // 以及 为了标记这个回收状态 所生成出的 内置回收标记列表
//         core.Subscribe(1 << 0, ListPool<GameObject>.Get(), true) // 可选回收
//             // 触发 回收 时，该列表 不会被回收
//             .Subscribe(1 << 1, new List<GameObject>()); // 请勿将 非对象池 生成 的列表 标记为回收
//         // 唯一 显示
//         {
//             core.DisplayFlags(1 << 0);
//         }
//         // 多显示
//         {
//             core.DisplayFlags(1 << 0 | 1 << 1);
//         }
//         // 回收
//         core.Release();
//         // 需要调用 Release 的 时候 ||
//         // 1、对象池 Get 出来的 Core
//         // 2、订阅了 标记为 回收 的 list
//     }
// }