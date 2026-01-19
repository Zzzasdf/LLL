using System;
using System.Collections.Generic;

public class RedPointNode: IRedPointLinkNode 
{
    // 当前节点
    public readonly ERedPoint Current;
    private Func<bool> condition;
    
    // 归属树
    public readonly ERedPointTree BelongToTree;
    
    // 父节点
    private RedPointNode nodeParent;
    // 子节点
    private HashSet<RedPointNode> nodeChildren;
    // 子树
    private HashSet<RedPointTree> treeChildren;

    public RedPointNode(ERedPoint current, ERedPointTree belongToTree)
    {
        this.Current = current;
        this.BelongToTree = belongToTree;
    }
    
    /// 添加显示的条件
    public void AddCondition(Func<bool> condition)
    {
        this.condition = condition;
    }

    /// 添加父节点
    public void AddParentNode(RedPointNode redPointNode)
    {
        nodeParent = redPointNode;
    }
    /// 添加子节点
    public void AddChildNode(RedPointNode nodeChild)
    {
        nodeChildren ??= new HashSet<RedPointNode>();
        nodeChildren.Add(nodeChild);
    }
    /// 添加子树
    public void AddChildTree(RedPointTree treeChild)
    {
        treeChildren ??= new HashSet<RedPointTree>();
        treeChildren.Add(treeChild);
    }
}
