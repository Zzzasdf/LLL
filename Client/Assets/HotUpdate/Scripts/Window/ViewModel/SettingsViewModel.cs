using CommunityToolkit.Mvvm.ComponentModel;

public partial class SettingsViewModel : ObservableObject, IViewModel
{
    private readonly IDataService dataService;
    private readonly GlobalSettingsModel globalSettingsModel;
    
    public SettingsViewModel(IDataService dataService, GlobalSettingsModel globalSettingsModel)
    {
        
    }
}
