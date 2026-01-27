using System.ComponentModel;
using UnityEngine;

public class MainView : ViewBase<MainViewModel>
{
    [SerializeField] private PhotographyLoader photographyLoader;

    protected override void InitUI()
    {
        photographyLoader.Load(viewModel.ModelLocation, viewModel.CameraDistance);
        viewModel.PropertyChanged += PropertyChanged;
    }
    protected override void DestroyUI()
    {
        viewModel.PropertyChanged -= PropertyChanged;
    }

    protected override void BindUI()
    {
    }
    protected override void UnBindUI()
    {
    }
    
    private void PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(MainViewModel.ModelLocation):
            case nameof(MainViewModel.CameraDistance):
                photographyLoader.Load(viewModel.ModelLocation, viewModel.CameraDistance);
                break;
        }
    }
}
