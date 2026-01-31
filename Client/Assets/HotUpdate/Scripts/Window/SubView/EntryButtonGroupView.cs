using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntryButtonGroupView : ViewBase<EntryButtonGroupViewModel>
{
    [SerializeField] private Button btnActivity;
    
    public override void InitUI(IViewCheck viewCheck)
    {
    }

    public override void DestroyUI()
    {
    }

    public override void BindUI()
    {
        btnActivity.onClick.AddListener(() => viewModel.ShowActivityCommand.Execute(null));
    }

    public override void UnBindUI()
    {
        btnActivity.onClick.RemoveAllListeners();
    }
}
