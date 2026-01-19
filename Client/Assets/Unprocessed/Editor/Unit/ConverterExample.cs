using System.Collections.Generic;
using System.IO;
using Google.Protobuf.Collections;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class ConverterExample
{
    [Test]
    public void Save()
    { 
        // BitmapConverter bitmapConverter = ClientConverterCollectorSystem.EBitmap.Test_0.GetConverter();
        // {
        //     bitmapConverter[1] = true;
        //     bitmapConverter.AddFlag(64);
        //     bitmapConverter.SetFlag(2,false);
        //     bitmapConverter.RemoveFlag(63);
        //     bitmapConverter.Clear();
        // }
        
        // BitmapReverseConverter bitmapReverseConverter = ClientConverterCollectorSystem.EBitmapReverse.Test_1.GetConverter();
        // {
        //     bitmapReverseConverter.RemoveFlag(127);
        //     bitmapReverseConverter.AddFlag(127);
        //     bitmapReverseConverter[127] = false;
        //     bitmapReverseConverter.Fill();
        // }

        // RepeatedField<int> listInt = ClientConverterCollectorSystem.EListInt.Test_2.GetConverter();
        // {
        //     listInt.Add(123456789);
        //     listInt.Remove(123456789);
        //     listInt.Add(321);
        //     listInt.Add(1234);
        //     listInt.Clear();
        // }

        // RepeatedField<long> listLong = ClientConverterCollectorSystem.EListLong.Test_3.GetConverter();
        // {
        //     listLong.Add(123456789);
        //     listLong.Remove(123456789);
        //     listLong.Add(321);
        //     listLong.Add(1234);
        //     listLong.Clear();
        // }

        // HashSet<int> hashSetInt = ClientConverterCollectorSystem.EHashSetInt.Test_4.GetConverter();
        // {
        //     hashSetInt.Add(123456789);
        //     hashSetInt.Remove(123456789);
        //     hashSetInt.Add(321);
        //     hashSetInt.Add(1234);
        //     hashSetInt.Clear();
        // }

        // HashSetIntReverseConverter hashSetIntReverseConverter = ClientConverterCollectorSystem.EHashSetIntReverse.Test_5.GetConverter();
        // {
        //     hashSetIntReverseConverter.Remove(123456789);
        //     hashSetIntReverseConverter.Add(123456789);
        //     hashSetIntReverseConverter.Remove(321);
        //     hashSetIntReverseConverter.Remove(1234);
        //     hashSetIntReverseConverter.Fill();
        // }

        // HashSet<long> hashSetLong = ClientConverterCollectorSystem.EHashSetLong.Test_6.GetConverter();
        // {
        //     hashSetLong.Add(123456789);
        //     hashSetLong.Remove(123456789);
        //     hashSetLong.Add(321);
        //     hashSetLong.Add(1234);
        //     hashSetLong.Clear();
        // }

        // HashSetLongReverseConverter hashSetLongReverseConverter = ClientConverterCollectorSystem.EHashSetLongReverse.Test_7.GetConverter();
        // {
        //     hashSetLongReverseConverter.Add(123456789);
        //     hashSetLongReverseConverter.Remove(123456789);
        //     hashSetLongReverseConverter.Add(321);
        //     hashSetLongReverseConverter.Add(1234);
        //     hashSetLongReverseConverter.Fill();
        // }

        // MapIntBitConverter mapIntBitConverter = ClientConverterCollectorSystem.EMapIntBit.Test_8.GetConverter();
        // {
        //     mapIntBitConverter[1, 31] = true;
        //     mapIntBitConverter.RemoveFlag(1, 31);
        //     mapIntBitConverter.AddFlag(3, 30);
        //     mapIntBitConverter.SetFlag(3, 30, false);
        //     mapIntBitConverter.Clear();
        // }

        // MapIntBitReverseConverter mapIntBitReverseConverter = ClientConverterCollectorSystem.EMapIntBitReverse.Test_9.GetConverter();
        // {
        //     mapIntBitReverseConverter[1, 31] = true;
        //     mapIntBitReverseConverter.RemoveFlag(1, 31);
        //     mapIntBitReverseConverter.AddFlag(3, 30);
        //     mapIntBitReverseConverter.SetFlag(3, 30, false);
        //     mapIntBitReverseConverter.Fill();
        // }

        // Dictionary<int, int> mapIntInt = ClientConverterCollectorSystem.EMapIntInt.Test_10.GetConverter();
        // {
        //     mapIntInt.Add(123, 321);
        //     mapIntInt.Add(123456789, 987654321);
        // }

        // Dictionary<int, long> mapIntLong = ClientConverterCollectorSystem.EMapIntLong.Test_11.GetConverter();
        // {
        //     mapIntLong.Add(123, 321);
        //     mapIntLong.Add(123456789, 987654321);
        //     mapIntLong.Add(456, 654);
        // }

        // MapLongBitConverter mapLongBitConverter = ClientConverterCollectorSystem.EMapLongBit.Test_12.GetConverter();
        // {
        //     mapLongBitConverter[1, 63] = true;
        //     mapLongBitConverter.RemoveFlag(1, 63);
        //     mapLongBitConverter.AddFlag(3, 62);
        //     mapLongBitConverter.SetFlag(3, 62, false);
        //     mapLongBitConverter.Clear();
        // }

        // MapLongBitReverseConverter mapLongBitReverseConverter = ClientConverterCollectorSystem.EMapLongBitReverse.Test_13.GetConverter();
        // {
        //     mapLongBitReverseConverter[1, 63] = true;
        //     mapLongBitReverseConverter.RemoveFlag(1, 63);
        //     mapLongBitReverseConverter.AddFlag(3, 62);
        //     mapLongBitReverseConverter.SetFlag(3, 62, false);
        //     mapLongBitReverseConverter.Fill();
        // }

        // Dictionary<long, int> mapLongInt = ClientConverterCollectorSystem.EMapLongInt.Test_14.GetConverter();
        // {
        //     mapLongInt.Add(123, 321);
        //     mapLongInt.Add(123456789, 987654321);
        //     mapLongInt.Add(456, 654);
        // }

        // Dictionary<long, long> mapLongLong = ClientConverterCollectorSystem.EMapLongLong.Test_15.GetConverter();
        // {
        //     mapLongLong.Add(123, 321);
        //     mapLongLong.Add(123456789, 987654321);
        //     mapLongLong.Add(456, 654);
        // }
        
        // ClientDailyFirstRedDotConverter clientDailyFirstRedDotConverter = ClientConverterCollectorSystem.EDailyFirstRedDot.Unique_16.GetConverter();
        // {
        //     clientDailyFirstRedDotConverter.AddFlag(EDailyFirstRedDot.Test0);
        //     clientDailyFirstRedDotConverter.RemoveFlag(EDailyFirstRedDot.Test0);
        //     clientDailyFirstRedDotConverter.AddFlag(EDailyFirstRedDot.Test100);
        //     clientDailyFirstRedDotConverter[EDailyFirstRedDot.Test1024] = true;
        //     clientDailyFirstRedDotConverter.SetFlag(EDailyFirstRedDot.Test1024, false);
        // }

        ClientConverterCollectorSystem converterCollectorSystem = new ClientConverterCollectorSystem();
        converterCollectorSystem.SaveAll();
        Debug.LogError("保存 => " + converterCollectorSystem);
    }

    [Test]
    public void Read()
    {
        RepeatedField<RepeatedField<long>> data = null;
        // BinaryDataSaver.TryLoadData(BinaryDataSaver.EFile.ClientConverterData, out data);
        ClientConverterCollectorSystem converterCollectorSystem = new ClientConverterCollectorSystem();
        converterCollectorSystem.AddSource(data);

        // BitmapConverter bitmapConverter = ClientConverterCollectorSystem.EBitmap.Test_0.GetConverter();
        // {
        //     Debug.LogError($"63 Exist => { bitmapConverter.HasFlag(63)}");
        // }
        
        // BitmapReverseConverter bitmapReverseConverter = ClientConverterCollectorSystem.EBitmapReverse.Test_1.GetConverter();
        // {
        //     Debug.LogError($"63 Exist => {bitmapReverseConverter.HasFlag(63)}");
        //     Debug.LogError($"127 Exist => { bitmapReverseConverter.HasFlag(127)}");
        // }
       
        // RepeatedField<int> listInt = ClientConverterCollectorSystem.EListInt.Test_2.GetConverter();
        // {
        //     Debug.LogError($"123456789 Exist => {listInt.Contains(123456789)}");
        //     Debug.LogError($"321 Exist => {listInt.Contains(321)}");
        //     Debug.LogError($"1234 Exist => {listInt.Contains(1234)}");
        // }
        
        // RepeatedField<long> listLong = ClientConverterCollectorSystem.EListLong.Test_3.GetConverter();
        // {
        //     Debug.LogError($"123456789 Exist => {listLong.Contains(123456789)}");
        //     Debug.LogError($"321 Exist => {listLong.Contains(321)}");
        //     Debug.LogError($"1234 Exist => {listLong.Contains(1234)}");
        // }
        
        // HashSet<int> hashSetInt = ClientConverterCollectorSystem.EHashSetInt.Test_4.GetConverter();
        // {
        //     Debug.LogError($"123456789 Exist => {hashSetInt.Contains(123456789)}");
        //     Debug.LogError($"321 Exist => {hashSetInt.Contains(321)}");
        //     Debug.LogError($"1234 Exist => {hashSetInt.Contains(1234)}");
        // }
        
        // HashSetIntReverseConverter hashSetIntReverseConverter = ClientConverterCollectorSystem.EHashSetIntReverse.Test_5.GetConverter();
        // {
        //     Debug.LogError($"123456789 Exist => {hashSetIntReverseConverter.Contains(123456789)}");
        //     Debug.LogError($"321 Exist => {hashSetIntReverseConverter.Contains(321)}");
        //     Debug.LogError($"1234 Exist => {hashSetIntReverseConverter.Contains(1234)}");
        // }
        
        // HashSet<long> hashSetLong = ClientConverterCollectorSystem.EHashSetLong.Test_6.GetConverter();
        // {
        //     Debug.LogError($"123456789 Exist => {hashSetLong.Contains(123456789)}");
        //     Debug.LogError($"321 Exist => {hashSetLong.Contains(321)}");
        //     Debug.LogError($"1234 Exist => {hashSetLong.Contains(1234)}");
        // }
        
        // HashSetLongReverseConverter hashSetLongReverseConverter = ClientConverterCollectorSystem.EHashSetLongReverse.Test_7.GetConverter();
        // {
        //     Debug.LogError($"123456789 Exist => {hashSetLongReverseConverter.Contains(123456789)}");
        //     Debug.LogError($"321 Exist => {hashSetLongReverseConverter.Contains(321)}");
        //     Debug.LogError($"1234 Exist => {hashSetLongReverseConverter.Contains(1234)}");
        // }
        
        // MapIntBitConverter mapIntBitConverter = ClientConverterCollectorSystem.EMapIntBit.Test_8.GetConverter();
        // {
        //     Debug.LogError($"1,31 Exist => {mapIntBitConverter.HasFlag(1, 31)}");
        //     Debug.LogError($"3,30 Exist => {mapIntBitConverter.HasFlag(3, 30)}");
        // }
        
        // MapIntBitReverseConverter mapIntBitReverseConverter = ClientConverterCollectorSystem.EMapIntBitReverse.Test_9.GetConverter();
        // {
        //     Debug.LogError($"1,31 Exist => {mapIntBitReverseConverter.HasFlag(1, 31)}");
        //     Debug.LogError($"3,30 Exist => {mapIntBitReverseConverter.HasFlag(3, 30)}");
        // }
        
        // Dictionary<int, int> mapIntInt = ClientConverterCollectorSystem.EMapIntInt.Test_10.GetConverter();
        // {
        //     Debug.LogError($"123 Exist => {mapIntInt[123]}");
        //     Debug.LogError($"123456789 Exist => {mapIntInt[123456789]}");
        // }
        
        // Dictionary<int, long> mapIntLong = ClientConverterCollectorSystem.EMapIntLong.Test_11.GetConverter();
        // {
        //     Debug.LogError($"123 Exist => {mapIntLong[123]}");
        //     Debug.LogError($"123456789 Exist => {mapIntLong[123456789]}");
        // }
        
        // MapLongBitConverter mapLongBitConverter = ClientConverterCollectorSystem.EMapLongBit.Test_12.GetConverter();
        // {
        //     Debug.LogError($"1,63 Exist => {mapLongBitConverter.HasFlag(1, 63)}");
        //     Debug.LogError($"3,62 Exist => {mapLongBitConverter.HasFlag(3, 62)}");
        // }
        
        // MapLongBitReverseConverter mapLongBitReverseConverter = ClientConverterCollectorSystem.EMapLongBitReverse.Test_13.GetConverter();
        // {
        //     Debug.LogError($"1,63 Exist => {mapLongBitReverseConverter.HasFlag(1, 63)}");
        //     Debug.LogError($"3,62 Exist => {mapLongBitReverseConverter.HasFlag(3, 62)}");
        // }
        
        // Dictionary<long, int> mapLongInt = ClientConverterCollectorSystem.EMapLongInt.Test_14.GetConverter();
        // {
        //     Debug.LogError($"123 Exist => {mapLongInt[123]}");
        //     Debug.LogError($"123456789 Exist => {mapLongInt[123456789]}");
        // }
        
        // Dictionary<long, long> mapLongLong = ClientConverterCollectorSystem.EMapLongLong.Test_15.GetConverter();
        // {
        //     Debug.LogError($"123 Exist => {mapLongLong[123]}");
        //     Debug.LogError($"123456789 Exist => {mapLongLong[123456789]}");
        // }

        // ClientDailyFirstRedDotConverter clientDailyFirstRedDotConverter = ClientConverterCollectorSystem.EDailyFirstRedDot.Unique_16.GetConverter();
        // {
        //     Debug.LogError($"100 Exist => {clientDailyFirstRedDotConverter.HasFlag(EDailyFirstRedDot.Test100)}");
        //     Debug.LogError($"1024 Exist => {clientDailyFirstRedDotConverter[EDailyFirstRedDot.Test1024]}");
        // }
        
        Debug.LogError("读取 => " + converterCollectorSystem);
    }
}
