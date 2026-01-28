public class ViewCheck : IViewCheck
{
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
