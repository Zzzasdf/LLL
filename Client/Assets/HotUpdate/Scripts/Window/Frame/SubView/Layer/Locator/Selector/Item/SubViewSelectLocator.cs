using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SubViewSelectLocator: MonoBehaviour, ISubViewSelectLocator
{
    [SerializeField] private RectTransform rtItemParent;
    [SerializeField] private SubViewSelectItemLocatorBase itemLocatorTemp;
    [SerializeField] private RectTransform rtPanelParent;
    
    private Func<RectTransform, int, UniTask> selectIndexFunc;
    private List<SubViewSelectItemLocatorBase> btnSelects;
    private bool isAllowSelect;

    void ISubViewSelectLocator.Init(List<ISubViewConfigure> subViewConfigures, Func<RectTransform, int, UniTask> selectIndexFunc, int firstShowIndex)
    {
        if (btnSelects != null)
        {
            for (int i = btnSelects.Count - 1; i >= 0; i--)
            {
                Destroy(btnSelects[i]);
            }
            btnSelects.Clear();
        }
        itemLocatorTemp.gameObject.SetActive(false);
        this.selectIndexFunc = selectIndexFunc;
        btnSelects = new List<SubViewSelectItemLocatorBase>();
        for (int i = 0; i < subViewConfigures.Count; i++)
        {
            ISubViewConfigure subViewConfigure = subViewConfigures[i];
            SubViewSelectItemLocatorBase btnSelect = Instantiate(itemLocatorTemp, Vector3.zero, Quaternion.identity, rtItemParent);
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
