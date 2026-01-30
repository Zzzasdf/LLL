public class SubActivityCheck : IViewCheck
{
    private int subActivityId;
    private string btnEntryName;

    public SubActivityCheck(int subActivityId, string btnEntryName)
    {
        this.subActivityId = subActivityId;
        this.btnEntryName = btnEntryName;
    }

    object IViewCheck.GetViewCheckValue() => null;
    string IViewCheck.BtnEntryName() => btnEntryName;

    bool IViewCheck.IsFuncOpen()
    {
        return true;
    }
    bool IViewCheck.IsFuncOpenWithTip()
    {
        IViewCheck viewCheck = this;
        bool result = viewCheck.IsFuncOpen();
        LLogger.Warning($"{nameof(SubActivityCheck)} => {result}");
        return result;
    }
}
