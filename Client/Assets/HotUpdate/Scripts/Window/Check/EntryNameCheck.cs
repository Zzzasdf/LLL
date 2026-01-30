public class EntryNameCheck : IViewCheck
{
    private string btnEntryName;

    public EntryNameCheck(string btnEntryName)
    {
        this.btnEntryName = btnEntryName;
    }
    
    object IViewCheck.GetViewCheckValue() => null;

    string IViewCheck.BtnEntryName() => btnEntryName;

    bool IViewCheck.IsFuncOpen() => true;
    bool IViewCheck.IsFuncOpenWithTip()
    {
        IViewCheck viewCheck = this;
        return viewCheck.IsFuncOpen();
    }
}
