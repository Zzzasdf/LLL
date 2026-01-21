using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

public partial class HelpViewModel : ObservableObject, IViewModel
{
    private readonly IDataService dataService;
    
    public HelpViewModel(IDataService dataService)
    {
    }
    
    [RelayCommand]
    private async Task Close(IView view)
    {
        await WeakReferenceMessenger.Default.SendViewHideAsync(view);
    }
}
