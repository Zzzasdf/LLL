public interface IDataService
{
    T AccountLevelGet<T>() where T : IAccountLevelModel, new();

    T Get<T>() where T: new();
}
