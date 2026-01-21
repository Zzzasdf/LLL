using System.ComponentModel;

public enum ViewLayer
{
    [Description("背景")]
    Bg = 1,
    [Description("常驻")]
    Permanent = 2,
    [Description("全屏")]
    FullScreen = 3,
    [Description("窗口")]
    Window = 4,
    [Description("弹窗")]
    Popup = 5,
    [Description("提示")]
    Tip = 6,
    [Description("系统")]
    System = 7,
}
