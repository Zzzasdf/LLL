using System;
using System.Collections.Generic;

public class RedPointTree: IRedPointLinkNode
{
    private ERedPoint parent;
    
    private ERedPointTree treeCurrent;
    private Func<bool> linkCondition;

    private HashSet<ERedPoint> nodeChildren;

    public RedPointTree(ERedPointTree treeCurrent):this(treeCurrent, null)
    {
        
    }
    public RedPointTree(ERedPointTree treeCurrent, Func<bool> linkCondition)
    {
        this.treeCurrent = treeCurrent;
        this.linkCondition = linkCondition;
    }

    public void AddParent(ERedPoint eRedPoint)
    {
        this.parent = eRedPoint;
    }
    public void AddChildren(ERedPoint eRedPoint)
    {
        nodeChildren ??= new HashSet<ERedPoint>();
        nodeChildren.Add(eRedPoint);
    }
}
