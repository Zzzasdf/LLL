using UnityEngine;
using UnityEngine.UI;

public class StartView : ViewBase<StartViewModel>
{
    [SerializeField] private Button btnStart;
    [SerializeField] private Button btnShaderExamples;
    [SerializeField] private Button btnSettings;
    [SerializeField] private Button btnHelp;
    [SerializeField] private Button btnQuit;
    [SerializeField] private Button btnClose;

    public override void InitUI(IViewCheck viewCheck)
    {
    }
    public override void DestroyUI()
    {
    }

    public override void BindUI()
    {
        // 绑定命令到按钮
        btnStart.onClick.AddListener(() => viewModel.StartCommand.Execute(null));
        btnShaderExamples.onClick.AddListener(() => viewModel.ShaderExamplesCommand.Execute(null));
        btnSettings.onClick.AddListener(() => viewModel.SettingsCommand.Execute(null));
        btnHelp.onClick.AddListener(() => viewModel.HelpCommand.Execute(null));
        btnQuit.onClick.AddListener(() => viewModel.QuitCommand.Execute(null));
        btnClose.onClick.AddListener(() => viewModel.CloseCommand.Execute(this));
    }
    
    public override void UnBindUI()
    {
        // 清理按钮事件
        btnStart.onClick.RemoveAllListeners();
        btnShaderExamples.onClick.RemoveAllListeners();
        btnSettings.onClick.RemoveAllListeners();
        btnHelp.onClick.RemoveAllListeners();
        btnQuit.onClick.RemoveAllListeners();
        btnClose.onClick.RemoveAllListeners();
    }
}
