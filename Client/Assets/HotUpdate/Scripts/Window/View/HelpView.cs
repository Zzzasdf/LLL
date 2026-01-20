using System;
using System.Collections;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.DependencyInjection;
using UnityEngine;
using UnityEngine.UI;

public class HelpView : ViewBase
{
    [SerializeField] private Button btnClose;

    private HelpViewModel viewModel;
    
    private void Start()
    {
        viewModel = Ioc.Default.GetService<HelpViewModel>();
        BindUI();
    }

    private void BindUI()
    {
        btnClose.onClick.AddListener(()=> viewModel.CloseCommand.Execute(this));
    }
}
