public class SubActivityCheck : IViewCheck
{
    private int subActivityId;

    public SubActivityCheck(int subActivityId)
    {
        this.subActivityId = subActivityId;
    }

    object IViewCheck.GetViewCheckValue() => null;

    bool IViewCheck.IsFuncOpen()
    {
        return true;
    }

    bool IViewCheck.IsFuncOpenWithTip()
    {
        IViewCheck viewCheck = this;
        bool result = viewCheck.IsFuncOpen();
        LLogger.Warning($"{nameof(ViewCheck)} => {result}");
        return result;
    }
}
