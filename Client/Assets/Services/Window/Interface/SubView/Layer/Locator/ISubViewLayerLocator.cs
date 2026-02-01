public interface ISubViewLayerLocator
{
    void Init(IViewConfigure viewConfigure, SubViewShow? firstSubViewShow = null);
    void HideViews();
}
