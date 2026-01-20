using CommunityToolkit.Mvvm.ComponentModel;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IDataService dataService;
    private readonly SettingsModel settingsModel;
    
    public SettingsViewModel(IDataService dataService, SettingsModel settingsModel)
    {
        
    }
}
