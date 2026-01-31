using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class SubViewSelectorBase : MonoBehaviour
{
    [SerializeField] private RectTransform rtItemParent;
    [SerializeField] private SubViewSelectorItemBase itemTemp;
    [SerializeField] private RectTransform rtPanelParent;
    
    private Func<RectTransform, int, UniTask> selectIndexFunc;
    private List<SubViewSelectorItemBase> btnSelects;
    private bool isAllowSelect;

    public void Init(List<ISubViewConfigure> subViewConfigures, Func<RectTransform, int, UniTask> selectIndexFunc, int firstShowIndex)
    {
        if (btnSelects != null)
        {
            for (int i = btnSelects.Count - 1; i >= 0; i--)
            {
                Destroy(btnSelects[i]);
            }
            btnSelects.Clear();
        }
        itemTemp.gameObject.SetActive(false);
        this.selectIndexFunc = selectIndexFunc;
        btnSelects = new List<SubViewSelectorItemBase>();
        for (int i = 0; i < subViewConfigures.Count; i++)
        {
            ISubViewConfigure subViewConfigure = subViewConfigures[i];
            SubViewSelectorItemBase btnSelect = Instantiate(itemTemp, Vector3.zero, Quaternion.identity, rtItemParent);
            btnSelect.gameObject.SetActive(true);
            btnSelect.Init(subViewConfigure, OnBtnSelectClickAsync, i);
            btnSelects.Add(btnSelect);
        }
        isAllowSelect = true;
        selectIndexFunc.Invoke(rtPanelParent, firstShowIndex);
    }

    private async UniTask OnBtnSelectClickAsync(int index)
    {
        if (!isAllowSelect) return;
        isAllowSelect = false;
        await selectIndexFunc.Invoke(rtPanelParent, index);
        isAllowSelect = true;
    }
}
