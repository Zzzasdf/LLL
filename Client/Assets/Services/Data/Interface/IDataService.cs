public interface IDataService
{
    public T AccountLevelGet<T>() where T : IAccountLevelModel, new();
    public void AccountLevelSave<T>(T data) where T : IAccountLevelModel, new();
    
    public T Get<T>() where T: new();
    public void Save<T>(T data) where T : new();
}
