public class SubViewMultipleContainer : ISubViewContainer
{
    private SubViewContainerType subViewContainerType;

    void ISubViewContainer.AddSubViewContainerType(SubViewContainerType subViewContainerType) => this.subViewContainerType = subViewContainerType;
}
