using UnityEngine;

public class DeviceService : IDeviceService
{
    // public DeviceService()
    // {
    //     // 1. 基础设备信息
    //     Debug.Log($"设备型号: {SystemInfo.deviceModel}");
    //     Debug.Log($"设备类型: {SystemInfo.deviceType}");
    //     Debug.Log($"操作系统: {SystemInfo.operatingSystem}");
    //
    //     // 2. CPU与内存信息
    //     Debug.Log($"CPU: {SystemInfo.processorType}");
    //     Debug.Log($"CPU核心(线程)数: {SystemInfo.processorCount}");
    //     Debug.Log($"系统内存: {SystemInfo.systemMemorySize} MB");
    //
    //     // 3. 图形设备信息
    //     Debug.Log($"显卡: {SystemInfo.graphicsDeviceName}");
    //     Debug.Log($"显存: {SystemInfo.graphicsMemorySize} MB");
    //     Debug.Log($"图形API: {SystemInfo.graphicsDeviceType}");
    //     Debug.Log($"最大纹理尺寸: {SystemInfo.maxTextureSize}");
    //
    //     // 4. 功能支持检测 (常用于适配不同设备)
    //     Debug.Log($"支持计算着色器: {SystemInfo.supportsComputeShaders}");
    //     Debug.Log($"支持陀螺仪: {SystemInfo.supportsGyroscope}");
    //
    //     // 5. 电池信息 (移动端)
    //     if (SystemInfo.batteryStatus != BatteryStatus.Unknown) {
    //         Debug.Log($"电量: {SystemInfo.batteryLevel * 100}%");
    //         Debug.Log($"电池状态: {SystemInfo.batteryStatus}");
    //     }
    //
    //     // 6. 简易性能适配示例：根据显存调整画质
    //     AdjustQualityBasedOnVRAM();
    // }
    //
    // private void AdjustQualityBasedOnVRAM()
    // {
    //     int vramMB = SystemInfo.graphicsMemorySize;
    //     if (vramMB >= 8192) { // 8GB及以上
    //         QualitySettings.SetQualityLevel(5); // 超高画质
    //     } else if (vramMB >= 4096) { // 4GB - 8GB
    //         QualitySettings.SetQualityLevel(3); // 高画质
    //     } else { // 4GB以下
    //         QualitySettings.SetQualityLevel(1); // 中或低画质
    //     }
    //     Debug.Log($"根据显存({vramMB}MB)设置画质等级为: {QualitySettings.GetQualityLevel()}");
    // }

    void IDeviceService.ApplicationQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("Play mode stopped by script");
#else
        Application.Quit();
#endif
    }
}
