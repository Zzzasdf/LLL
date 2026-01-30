public class SubViewCollectContainer: ISubViewCollectContainer
{
    private ISubViewCollectLocator subViewCollectLocator;

    void ISubViewCollectContainer.Bind(ISubViewCollectLocator subViewCollectLocator)
    {
        this.subViewCollectLocator = subViewCollectLocator;
    }
}

