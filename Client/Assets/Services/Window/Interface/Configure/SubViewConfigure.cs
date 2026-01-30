using System;

public class SubViewConfigure : ISubViewConfigure
{
    private Type type;
    private SubViewShow? subViewShow;
    private IViewCheck viewCheck;

    public SubViewConfigure(Type type)
    {
        this.type = type;
    }
    public SubViewConfigure(Type type, SubViewShow subViewShow)
    {
        this.type = type;
        this.subViewShow = subViewShow;
    }
    public SubViewConfigure(Type type, IViewCheck viewCheck)
    {
        this.type = type;
        this.viewCheck = viewCheck;
    }
    public SubViewConfigure(Type type, SubViewShow subViewShow, IViewCheck viewCheck)
    {
        this.type = type;
        this.subViewShow = subViewShow;
        this.viewCheck = viewCheck;
    }

    bool ISubViewConfigure.TryGetSubViewShow(out SubViewShow subViewShow)
    {
        if (!this.subViewShow.HasValue)
        {
            subViewShow = default;
            return false;
        }
        subViewShow = this.subViewShow.Value;
        return true;
    }

    bool ISubViewConfigure.EqualsSubViewShow(SubViewShow subViewShow)
    {
        if (!this.subViewShow.HasValue)
        {
            return false;
        }
        return this.subViewShow.Value.Equals(subViewShow);
    }

    bool ISubViewConfigure.TryGetViewCheck(out IViewCheck viewCheck)
    {
        if (this.viewCheck == null)
        {
            viewCheck = null;
            return false;
        }
        viewCheck = this.viewCheck;
        return true;
    }
}
