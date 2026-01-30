public interface ISubViewsLocator
{
    void Init(ISubViewCollectLocator subViewCollectLocator, IViewConfigure viewConfigure, SubViewShow firstSubViewShow);
    void Init(ISubViewCollectLocator subViewCollectLocator, IViewConfigure viewConfigure);
    void SwitchSubView(int showIndex);
}
