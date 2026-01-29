public class SubViewUniqueContainer : ISubViewContainer
{
    private SubViewContainerType subViewContainerType;

    void ISubViewContainer.AddSubViewContainerType(SubViewContainerType subViewContainerType) => this.subViewContainerType = subViewContainerType;
}
