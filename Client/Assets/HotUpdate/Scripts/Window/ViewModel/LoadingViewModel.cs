using CommunityToolkit.Mvvm.ComponentModel;

public partial class LoadingViewModel : ObservableObject, IViewModel
{
    private readonly IDataService dataService;
    
    public LoadingViewModel(IDataService dataService)
    {
        
    }
}
