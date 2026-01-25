using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmAgainView : ViewBase<ConfirmAgainViewModel>
{
    [SerializeField] private TextMeshProUGUI lbContent;
    [SerializeField] private Button btnCancel;
    [SerializeField] private Button btnConfirm;

    protected override void BindUI()
    {
        viewModel.PropertyChanged += PropertyChanged;
        btnCancel.onClick.AddListener(() => viewModel.CancelCommand.Execute(this));
        btnConfirm.onClick.AddListener(() => viewModel.ConfirmCommand.Execute(this));
    }

    protected override void UnBindUI()
    {
        viewModel.PropertyChanged -= PropertyChanged;
        btnCancel.onClick.RemoveAllListeners();
        btnConfirm.onClick.RemoveAllListeners();
    }
    
    private void PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ConfirmAgainViewModel.ConfirmAgainViewEvent):
                lbContent.SetText(viewModel.ConfirmAgainViewEvent.Content);
                break;
        }
    }
}
