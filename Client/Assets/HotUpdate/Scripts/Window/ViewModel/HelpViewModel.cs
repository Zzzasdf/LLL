using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

public partial class HelpViewModel : ObservableObject
{
    private readonly IDataService dataService;
    private readonly HelpModel helpModel;
    private readonly IViewService viewService;
    
    public HelpViewModel(IDataService dataService, HelpModel helpModel)
    {
        this.dataService = dataService;
        this.helpModel = helpModel;
        viewService = Ioc.Default.GetService<IViewService>();
    }
    
    [RelayCommand]
    private void Close(IView view)
    {
        viewService.HideAsync(view);
    }
}
