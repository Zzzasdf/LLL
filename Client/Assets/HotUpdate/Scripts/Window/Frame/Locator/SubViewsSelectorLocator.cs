using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SubViewsSelectorLocator: MonoBehaviour, ISubViewsLocator
{
    private ISubViewCollectLocator subViewCollectLocator;
    private IViewConfigure viewConfigure;
    private IViewLoader viewLoader;
    
    private List<ISubViewConfigure> subViewConfigures;
    private Dictionary<int, (Type type, int uniqueId)> typeUniqueIds;
    private int showIndex;
    
    [SerializeField]
    private SerializableDictionary<int, Type> uniqueTypeDict;
    [SerializeField]
    private SerializableDictionary<int, IView> uniqueViewDict;
    
    void ISubViewsLocator.Init(ISubViewCollectLocator subViewCollectLocator, IViewConfigure viewConfigure, SubViewShow firstSubViewShow)
    {
        this.subViewCollectLocator = subViewCollectLocator;
        this.viewConfigure = viewConfigure;
        this.viewLoader = subViewCollectLocator.GetViewLoader();
        this.subViewConfigures = new List<ISubViewConfigure>();
        this.typeUniqueIds = new Dictionary<int, (Type type, int uniqueId)>();
        this.uniqueTypeDict = new SerializableDictionary<int, Type>();
        this.uniqueViewDict = new SerializableDictionary<int, IView>();
        viewConfigure.TryGetSubViewConfigures(out List<ISubViewConfigure> subViewConfigures);
        for (int i = 0; i < subViewConfigures.Count; i++)
        {
            ISubViewConfigure subViewConfigure = subViewConfigures[i];
            if (!subViewConfigure.TryGetViewCheck(out IViewCheck viewCheck)
                || viewCheck.IsFuncOpen())
            {
                if (subViewConfigure.EqualsSubViewShow(firstSubViewShow))
                {
                    showIndex = subViewConfigures.Count;
                }
                this.subViewConfigures.Add(subViewConfigure);
            }
        }
    }
    void ISubViewsLocator.Init(ISubViewCollectLocator subViewCollectLocator, IViewConfigure viewConfigure)
    {
        this.subViewCollectLocator = subViewCollectLocator;
        this.viewConfigure = viewConfigure;
        this.viewLoader = subViewCollectLocator.GetViewLoader();
        this.subViewConfigures = new List<ISubViewConfigure>();
        this.typeUniqueIds = new Dictionary<int, (Type type, int uniqueId)>();
        this.uniqueTypeDict = new SerializableDictionary<int, Type>();
        this.uniqueViewDict = new SerializableDictionary<int, IView>();
        viewConfigure.TryGetSubViewConfigures(out List<ISubViewConfigure> subViewConfigures);
        for (int i = 0; i < subViewConfigures.Count; i++)
        {
            ISubViewConfigure subViewConfigure = subViewConfigures[i];
            if (!subViewConfigure.TryGetViewCheck(out IViewCheck viewCheck)
                || viewCheck.IsFuncOpen())
            {
                this.subViewConfigures.Add(subViewConfigure);
            }
        }
    }
    
    private async UniTask<IView> ShowViewAsync(Type type)
    {
        IViewLocator viewLocator;
        if (!viewLoader.TryGetActiveView(type, out IView view))
        {
            if (!viewLoader.TryGetPoolView(type, out view))
            {
                view = await viewLoader.CreateView(type);
                GameObject goView = view.GameObject();
                viewLocator = subViewCollectLocator.AddSubViewLocator(goView);
                viewLocator.Bind(view);
                view.BindLocator(viewLocator);
                
                RectTransform windowRt = goView.GetComponent<RectTransform>();
                windowRt.SetParent(transform);
                windowRt.localPosition = Vector3.zero;
                windowRt.localScale = Vector3.one;
                windowRt.anchoredPosition = Vector2.zero;
                windowRt.anchorMin = Vector2.zero;
                windowRt.anchorMax = Vector2.one;
                windowRt.offsetMin = Vector2.zero;
                windowRt.offsetMax = Vector2.zero;
            }
        }
        else
        {
            viewLocator = view.GetLocator();
            int oldUniqueId = viewLocator.GetUniqueId();
            uniqueViewDict.Remove(oldUniqueId);
            viewLocator.Hide();
        }
        int uniqueId = UniqueIdGenerator.Default.Create();
        uniqueTypeDict.Add(uniqueId, type);
        
        uniqueViewDict.Add(uniqueId, view);
        viewLocator = view.GetLocator();
        viewLocator.BindUniqueId(uniqueId);
        viewLocator.GameObject().transform.SetAsLastSibling();
        viewLocator.Show();
        return view;
    }
}
