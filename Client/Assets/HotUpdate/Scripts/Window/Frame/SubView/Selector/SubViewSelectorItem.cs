using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubViewSelectorItem : SubViewSelectorItemBase
{
    [SerializeField] private Button btnSelect;
    [SerializeField] private TextMeshProUGUI lbSelect;
    private ISubViewConfigure subViewConfigure;
    private Func<int, UniTask> selectIndexFunc;
    private int index;

    private void Awake()
    {
        btnSelect.onClick.AddListener(OnBtnSelectClick);
    }
    private void OnDestroy()
    {
        btnSelect.onClick.RemoveAllListeners();
    }

    public override void Init(ISubViewConfigure subViewConfigure, Func<int, UniTask> selectIndexFunc, int index)
    {
        this.subViewConfigure = subViewConfigure;
        this.selectIndexFunc = selectIndexFunc;
        this.index = index;
        
        this.subViewConfigure.TryGetViewCheck(out IViewCheck viewCheck);
        string lbBtnName = viewCheck.BtnEntryName();
        lbSelect.SetText(lbBtnName);
    }
    
    private void OnBtnSelectClick()
    {
        if (selectIndexFunc == null) return;
        OnBtnSelectClickAsync().Forget();
    }
    private async UniTask OnBtnSelectClickAsync()
    {
        await selectIndexFunc.Invoke(index);
    }
}
