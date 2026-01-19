using System.Collections.Generic;

public partial class RedPointTreeManager: IRedPointTreeManager 
{
    /// 映射节点
    private Dictionary<ERedPoint, RedPointNode> nodeMap;
    /// 映射根节点
    private Dictionary<ERedPoint, HashSet<ERedPoint>> rootMap;
    
    void IInit.OnInit()
    {
        nodeMap = new Dictionary<ERedPoint, RedPointNode>();
        rootMap = new Dictionary<ERedPoint, HashSet<ERedPoint>>();
        InitConfigure();
    }
    void IReset.OnReset()
    {
        
    }
    void IDestroy.OnDestroy()
    {
        
    }
}