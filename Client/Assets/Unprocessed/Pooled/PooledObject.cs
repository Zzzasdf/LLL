/*
 todo 1、必须要继承
 todo 2、外部能通过 new() 却无法通过 基类 实现抑制重订 析构函数
 todo 结论：实属不安全，不好用
*/
// using System;
// using UnityEngine.Pool;
//
// public abstract class PooledObject<T> : IDisposable where T: PooledObject<T>, IDisposable, new()
// {
//     private static readonly bool collectionCheck = true;
//     private static readonly int defaultCapacity = 10;
//     private static readonly int maxSize = 10000;
// #if UNITY_EDITOR
//     private static readonly ObjectPool<T> s_Pool = 
//         new(() => new T(), 
//             OnGet,
//             OnRelease,
//             null,
//             collectionCheck,
//             defaultCapacity, maxSize);
//     private static void OnGet(T t)
//     {
//         if (s_Pool.CountAll <= maxSize) return;
//         UnityEngine.Debug.LogError($"pool maxsize eg!! {t.GetType()} => 当前 active:{s_Pool.CountActive} + inactive:{s_Pool.CountInactive} > maxSize:{maxSize}, " +
//                                    $"当所有 active 对象回收时，将存在销毁，若前无集合重复回收报错，则请将最大容量至少提高到 {s_Pool.CountAll}");
//     }
//     private static void OnRelease(T t)
//     {
//         t.Clear();
//     }
// #else
//     private static readonly ObjectPool<T> s_Pool = 
//         new(() => new T(), 
//             null,
//             l => l.Clear(),
//             null,
//             collectionCheck,
//             defaultCapacity, maxSize);
// #endif
//
//     public static UnityEngine.Pool.PooledObject<T> Get(out T value) => s_Pool.Get(out value);
//     public static T Get() => s_Pool.Get();
//     private PooledObject() { }
//
//     protected abstract void Clear();
//     void IDisposable.Dispose()
//     {
//         s_Pool.Release(this as T);
//     }
//
// #if UNITY_EDITOR
//     private class Example
//     {
//         private class PooledAA : PooledObject<PooledAA>
//         {
//             public int a;
//             protected override void Clear()
//             {
//             }
//         }
//         
//         // 修改 obj 方法
//         private void SetObj(in PooledAA obj)
//         {
//             // obj = new PooledAA(); error!! in 限制，不允许在方法内 修改引用对象
//             obj.a = 0; // 支持 元子 操作
//         }
//         
//         #region 局部变量
//         private void Foo()
//         {
//             // 第一种写法，作用域为整个方法，在离开 Foo 方法后 自动释放
//             using var obj = PooledAA.Get();
//             SetObj(obj);
//             // obj = AA.Get(); error!! using 的引用对象不可变
//         
//             // 第二种写法
//             using (var obj2 = PooledAA.Get())
//             {
//                 SetObj(obj2);
//             } // 离开作用域，自动释放 进池 
//         }
//         #endregion
//         
//         #region 全局变量
//         // eg!!! 千万不要使用 obj3 = new() 的方式定义
//         // 脚本重新编译触发重置，每次修改脚本并返回运行模式时，Unity 会重新编译并重新初始化所有字段
//         // 导致会执行多次脚本，触发多次调用，大于实际同时存在的数量变多，error 日志不精确
//         // 快速检查 预制体上的所有 mono 脚本中是否存在该类型定义：
//         //      1、运行前选中过所需预制体，然后运行
//         //      2、重新编译前选中过所需预制体
//         private PooledAA obj3;
//         private void Init()
//         {
//             obj3 = PooledAA.Get(); // eg!! 初始化后不要在脚本中修改引用的对象，除非先释放
//         }
//         private void Foo2()
//         {
//             obj3.Clear();
//             SetObj(obj3);
//         }
//         public void DestroyModel()
//         {
//             // eg!! 在销毁时，需要手动释放 !! 
//             // 若缺少此步骤，当给这个池化集合初始化后，在触发 gc 或者 关闭编辑器时 会出现 error 日志
//             ((IDisposable)obj3).Dispose();
//         }
//         #endregion
//     }
// #endif
// }