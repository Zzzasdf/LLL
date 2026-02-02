using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class MainView : ViewEntityBase<MainViewModel>
{
    [SerializeField] private PhotographyLoader photographyLoader;
    [SerializeField] private Button btnClose;

    public override void InitUI(IViewCheck viewCheck)
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
        btnClose.onClick.AddListener(() => viewModel.CloseCommand.Execute(this));
    }
    public override void UnBindUI()
    {
        btnClose.onClick.RemoveAllListeners();
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
