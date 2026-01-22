using CommunityToolkit.Mvvm.ComponentModel;

public partial class MainViewModel : ObservableRecipient,
    IViewModel
{
    [ObservableProperty]
    private string _modelLocation = "Cube";
    [ObservableProperty]
    private float _cameraDistance = 10;
}
