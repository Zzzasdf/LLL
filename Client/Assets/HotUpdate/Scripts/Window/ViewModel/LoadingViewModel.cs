using CommunityToolkit.Mvvm.ComponentModel;

public partial class LoadingViewModel : ObservableObject
{
    private readonly IDataService dataService;
    private readonly LoadingModel loadingModel;
    
    public LoadingViewModel(IDataService dataService, LoadingModel loadingModel)
    {
        
    }
}
