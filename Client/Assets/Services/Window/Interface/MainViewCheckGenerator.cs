// using System;
// using System.Collections.Generic;
// using JetBrains.Annotations;
//
// public class MainViewCheckGenerator
// {
//     private static MainViewCheckGenerator _default = new MainViewCheckGenerator();
//     public static MainViewCheckGenerator Default => _default;
//     
//     private Dictionary<Type, IViewCheck> viewChecks;
//
//     public void Assembly(IViewConfigures viewConfigures)
//     {
//         viewChecks = new Dictionary<Type, IViewCheck>();
//         foreach (var pair in viewConfigures)
//         {
//             List<IViewConfigure> configures = pair.Value;
//             for (int i = 0; i < configures.Count; i++)
//             {
//                 IViewConfigure viewConfigure = configures[i];
//                 Type viewType = viewConfigure.GetViewType();
//                 if (viewConfigure.TryGetViewCheck(out IViewCheck viewCheck))
//                 {
//                     viewChecks.Add(viewType, viewCheck);
//                 }
//             }
//         }
//     }
//     [CanBeNull]
//     public IViewCheck Get(Type type)
//     {
//         if (!viewChecks.TryGetValue(type, out IViewCheck viewCheck))
//         {
//             return null;
//         }
//         return viewCheck;
//     }
// }