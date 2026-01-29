using System.ComponentModel;
using UnityEngine;

public class MainView : ViewBase<MainViewModel>
{
    [SerializeField] private PhotographyLoader photographyLoader;

    public override void InitUI(object viewCheckValue)
    {
        photographyLoader.Load(viewModel.ModelLocation, viewModel.CameraDistance);
        viewModel.PropertyChanged += PropertyChanged;
    }
    public override void DestroyUI()
    {
        viewModel.PropertyChanged -= PropertyChanged;
    }

    public override void BindUI()
    {
    }
    public override void UnBindUI()
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
