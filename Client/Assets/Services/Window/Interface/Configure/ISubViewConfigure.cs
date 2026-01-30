public interface ISubViewConfigure
{
    bool TryGetSubViewShow(out SubViewShow subViewShow);
    bool EqualsSubViewShow(SubViewShow subViewShow);
    bool TryGetViewCheck(out IViewCheck viewCheck);
}
