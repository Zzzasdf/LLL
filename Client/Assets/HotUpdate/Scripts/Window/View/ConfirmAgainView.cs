using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmAgainView : ViewEntityBase<ConfirmAgainViewModel>
{
    [SerializeField] private TextMeshProUGUI lbContent;
    [SerializeField] private Button btnCancel;
    [SerializeField] private Button btnConfirm;

    public override void InitUI(IViewCheck viewCheck)
    {
        viewModel.PropertyChanged += PropertyChanged;
    }
    public override void DestroyUI()
    {
        viewModel.PropertyChanged -= PropertyChanged;
    }

    public override void BindUI()
    {
        btnCancel.onClick.AddListener(() => viewModel.CancelCommand.Execute(this));
        btnConfirm.onClick.AddListener(() => viewModel.ConfirmCommand.Execute(this));
    }
    public override void UnBindUI()
    {
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
