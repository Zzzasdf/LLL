// using NUnit.Framework;
//
// [TestFixture]
// public class BusyDisposableExample
// {
//     private static bool result;
//
//     private static BusyDisposable NewBusyDisposable => BusyDisposable.Get(() => result = false);
//
//     [Test] 
//     public static void Foo()
//     { 
//         Foo1();
//         Assert.AreEqual(result, false, "未被更改");
//     }
//     
//     private static void Foo1()
//     {
//         result = true;
//         using var _ = NewBusyDisposable;
//     }
// }