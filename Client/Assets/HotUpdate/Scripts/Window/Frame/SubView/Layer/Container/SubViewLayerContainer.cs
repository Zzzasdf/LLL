public class SubViewLayerContainer: ISubViewLayerContainer
{
    private ISubViewLayerLocator subViewLayerLocator;
    
    void ISubViewLayerContainer.Bind(ISubViewLayerLocator subViewLayerLocator)
    {
        this.subViewLayerLocator = subViewLayerLocator;
    }
}

