using System;
using UnityEngine;

public class MainView : MonoBehaviour
{
    private MainViewModel _viewModel;

    public void Start()
    {
        _viewModel = new MainViewModel();

        _viewModel.PropertyChanged += (sender, e) =>
        {
            UpdateUIFromViewModel();
        };
    }

    private void UpdateUIFromViewModel()
    {
        
    }
}
