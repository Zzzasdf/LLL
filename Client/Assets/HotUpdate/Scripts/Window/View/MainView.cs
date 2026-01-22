using UnityEngine;

public class MainView : ViewBase<MainViewModel>
{
    [SerializeField] private PhotographyLoader photographyLoader;

    private void Start()
    {
        photographyLoader.Load(viewModel.ModelLocation, viewModel.CameraDistance);
        
        viewModel.PropertyChanged += (sender, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(MainViewModel.ModelLocation):
                case nameof(MainViewModel.CameraDistance):
                    photographyLoader.Load(viewModel.ModelLocation, viewModel.CameraDistance);
                    break;
            }
        };
    }
}
