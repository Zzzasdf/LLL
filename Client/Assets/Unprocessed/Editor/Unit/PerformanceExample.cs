/* GC =================================================
 * 1、方法代替委托，作为参数传递
 *  a: 委托是一个包装器类，缓存这个委托
 * 2、async 状态机，C# 会将带有 async 的 方法解析成 一个状态机
 *  a: 无解
 * 3、Linq.AddRange，每次调用都会产生枚举器
 *  a: 使用 foreach
 *
 *
 * CPU ==================================================
 * 1、HashSet.Add 前用 Contains 判断计算更快
 *
 *
 *
 *
 *
 *
 * 
 */

using NUnit.Framework;

[TestFixture]
public class PerformanceExample
{
    
}
