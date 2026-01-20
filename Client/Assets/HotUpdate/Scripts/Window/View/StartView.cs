using CommunityToolkit.Mvvm.DependencyInjection;
using UnityEngine;
using UnityEngine.UI;

public class StartView : ViewBase
{
    [SerializeField] private Button btnStart;
    [SerializeField] private Button btnSettings;
    [SerializeField] private Button btnHelp;
    [SerializeField] private Button btnQuit;

    private StartViewModel viewModel;
    
    private void Start()
    {
        // 使用依赖注入获取 ViewModel
        viewModel = Ioc.Default.GetService<StartViewModel>();
        BindUI();
    }

    private void BindUI()
    {
        // 绑定命令到按钮
        btnStart.onClick.AddListener(() => viewModel.StartCommand.Execute(null));
        btnSettings.onClick.AddListener(() => viewModel.SettingsCommand.Execute(null));
        btnHelp.onClick.AddListener(() => viewModel.HelpCommand.Execute(null));
        btnQuit.onClick.AddListener(() => viewModel.QuitCommand.Execute(null));
    }
    
    private void OnDestroy()
    {
        // 清理按钮事件
        btnStart.onClick.RemoveAllListeners();
        btnSettings.onClick.RemoveAllListeners();
        btnHelp.onClick.RemoveAllListeners();
        btnQuit.onClick.RemoveAllListeners();
    }
}
