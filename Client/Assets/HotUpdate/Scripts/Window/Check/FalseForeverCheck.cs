public class FalseForeverCheck : IViewCheck
{
    object IViewCheck.GetViewCheckValue() => false;
    string IViewCheck.BtnEntryName() => string.Empty;
    bool IViewCheck.IsFuncOpen() => false;
    bool IViewCheck.IsFuncOpenWithTip() => false;
}
