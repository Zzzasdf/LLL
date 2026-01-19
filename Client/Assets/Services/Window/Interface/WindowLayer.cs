using System.ComponentModel;

public enum WindowLayer
{
    [Description("常驻")]
    Permanent = 1,
    [Description("窗口")]
    Windows = 2,
    [Description("弹窗")]
    Popups = 3,
    [Description("提示")]
    Tips = 4,
    [Description("系统")]
    System = 5,
}
