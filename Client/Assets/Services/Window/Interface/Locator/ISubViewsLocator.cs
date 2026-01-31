public interface ISubViewsLocator
{
    void Init(ISubViewCollectLocator subViewCollectLocator, IViewConfigure viewConfigure, SubViewShow? firstSubViewShow = null);
    void HideViews();
}
