using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;

public class ViewLayerUniqueContainer: IViewLayerContainer
{
    private IViewLayerLocator viewLayerLocator;
    
    private List<int> uniqueIds;
    private Dictionary<int, List<int>> stashDict;
    
    void IViewLayerContainer.Bind(IViewLayerLocator viewLayerLocator)
    {
        this.viewLayerLocator = viewLayerLocator;
        uniqueIds = new List<int>();
        stashDict = new Dictionary<int, List<int>>();
    }

    async UniTask<(IView view, int? removeId)> IViewLayerContainer.ShowViewAndTryRemoveAsync(Type type)
    {
        if (uniqueIds.Count > 0)
        {
            int hideId = uniqueIds[^1];
            viewLayerLocator.PushHideView(hideId);
        }
        IView view = await viewLayerLocator.ShowViewAsync(type);
        IViewLocator viewLocator = (IViewLocator)view.GetLocator();
        int uniqueId = viewLocator.GetUniqueId();
        uniqueIds.Add(uniqueId);
        return (view, null);
    }

    async UniTask<List<int>> IViewLayerContainer.PopViewAndTryRemove(List<int> popIds)
    {
        await viewLayerLocator.TryPopViewAsync(popIds);
        return null;
    }

    List<int> IViewLayerContainer.HideViewTryPop(int uniqueId)
    {
        if (!TryRemoveAndTryPop(uniqueId, out List<int> popIds))
        {
            LLogger.FrameError($"请优先关闭栈顶的界面！！当前请求 uniqueId => {uniqueId}, 栈顶 uniqueId => {uniqueIds[^1]}");
        }
        else
        {
            viewLayerLocator.HideView(uniqueId);
        }
        return popIds;
    }
    void IViewLayerContainer.HideAllView()
    {
        IViewLayerContainer viewLayerContainer = this;
        viewLayerContainer.HideAllActivateView();
        viewLayerContainer.HideAllStashView();
    }
    void IViewLayerContainer.HideAllActivateView()
    {
        for (int i = uniqueIds.Count - 1; i >= 0; i--)
        {
            int uniqueId = uniqueIds[i];
            viewLayerLocator.HideView(uniqueId);
        }
        uniqueIds.Clear();
    }
    void IViewLayerContainer.HideAllStashView()
    {
        foreach (var pair in stashDict)
        {
            List<int> list = pair.Value;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                int uniqueId = uniqueIds[i];
                viewLayerLocator.HideView(uniqueId);
            }
        }
        stashDict.Clear();
    }

    private bool TryRemoveAndTryPop(int uniqueId, out List<int> popIds)
    {
        popIds = null;
        if (uniqueIds.Count == 0
            || uniqueIds[^1] != uniqueId)
        {
            return false;
        }
        uniqueIds.RemoveAt(uniqueIds.Count - 1);
        if (uniqueIds.Count == 0)
        {
            return true;
        }
        popIds = new List<int>
        {
            uniqueIds[^1]
        };
        return true;
    }
    
    void IViewLayerContainer.Stash(int uniqueId)
    {
        if (uniqueIds.Count == 0) return;
        if (!stashDict.TryGetValue(uniqueId, out List<int> list))
        {
            stashDict.Add(uniqueId, list = new List<int>());
        }
        foreach (var id in uniqueIds)
        {
            viewLayerLocator.PushHideView(id);
            list.Add(id);
        }
        uniqueIds.Clear();
    }
    
    bool IViewLayerContainer.TryStashPop(int uniqueId, out List<int> popIds)
    {
        if (!stashDict.Remove(uniqueId, out popIds))
        {
            return false;
        }
        for (int i = 0; i < popIds.Count; i++)
        {
            int id = popIds[i];
            uniqueIds.Insert(i, id);
        }
        popIds.Clear();
        popIds.Add(uniqueIds[^1]);
        return true;
    }

    void IViewLayerContainer.StashClear(int uniqueId)
    {
        stashDict.Remove(uniqueId);
    }
    
    string IViewLayerContainer.ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"uniqueIds Count => {uniqueIds.Count}");
        int index = 0;
        foreach (var uniqueId in uniqueIds)
        {
            sb.AppendLine($"[{index}] => {uniqueId}");
            index++;
        }

        sb.AppendLine($"stashDict Count => {stashDict.Count}");
        index = 0;
        foreach (var pair in stashDict)
        {
            int queueIndex = 0;
            StringBuilder sbQueue = new StringBuilder();
            foreach (var uniqueId in pair.Value)
            {
                sbQueue.Append($"[{queueIndex}] => {uniqueId}");
            }
            sb.AppendLine($"[{index}] => key: {pair.Key}, value: {sbQueue}");
            index++;
        }
        return sb.ToString();
    }
}
