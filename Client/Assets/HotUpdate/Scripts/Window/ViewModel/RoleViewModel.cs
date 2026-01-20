using CommunityToolkit.Mvvm.ComponentModel;

public partial class RoleViewModel : ObservableObject
{
    private readonly IDataService dataService;
    private readonly RoleModel roleModel;
    
    public RoleViewModel(IDataService dataService, RoleModel roleModel)
    {
        
    }
}
