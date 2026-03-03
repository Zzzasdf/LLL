using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public interface IViewLayerCore: IReusableDisposable
{
    void Init(IViewLayerDriver previousViewLayerViewDriver, IViewLayerDriver viewLayerDriver, IViewDriver viewDriver, ViewNode viewNode);
    
    UniTask<bool> ShowViewAsync(Stack<ViewNode> stack);
    bool HideView(Stack<ViewNode> stack);
    void HideView(ViewNode viewNode);
    void HideAllSubLayerView();

    void PushHide();
    void Push();
    void Pop();
    
    
    // void Bind(IViewLayerDriver viewLayerDriver);
    //
    // UniTask<(IView view, int? removeId)> ShowViewAndTryRemoveAsync(Type type);
    // UniTask<List<int>> PopViewAndTryRemove(List<int> popIds);
    //
    // List<int> HideViewTryPop(int uniqueId);
    // /// 隐藏所有激活 + 暂存界面
    // void HideAllView();
    // /// 隐藏所有激活界面
    // void HideAllActivateView();
    // /// 隐藏所有暂存界面
    // void HideAllStashView();
    //
    // void Stash(int uniqueId);
    // bool TryStashPop(int uniqueId, out List<int> popIds);
    // void StashClear(int uniqueId);

    string ToString();
}
