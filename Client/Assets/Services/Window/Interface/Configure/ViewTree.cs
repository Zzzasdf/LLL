using System.Collections.Generic;

public class ViewNode
{
    public ViewNode Previous { get; set; }
    
    public IViewConfigure Value { get; }
    
    public List<ViewNode> Nexts { get; set; }

    public ViewNode(IViewConfigure value)
    {
        Value = value;
    }
}

public class ViewTree
{
    private Dictionary<ViewType, ViewNode> viewNodes;

    public ViewTree(IViewConfigure viewConfigure)
    {
        viewNodes = new Dictionary<ViewType, ViewNode>();
        ViewNode viewNode = new ViewNode(viewConfigure);
        viewNodes.Add(viewConfigure.ViewType, viewNode);
        Assembly(viewNode, viewConfigure);
    }
    private void Assembly(ViewNode viewNode, IViewConfigure viewConfigure)
    {
        List<IViewConfigure> subViewConfigures = viewConfigure.SubViewConfigures;
        if (subViewConfigures == null || subViewConfigures.Count == 0) return;
        viewNode.Nexts = new List<ViewNode>(subViewConfigures.Count);
        for (int i = 0; i < subViewConfigures.Count; i++)
        {
            IViewConfigure subViewConfigure = subViewConfigures[i];
            ViewNode subViewNode = new ViewNode(subViewConfigure);
            viewNodes.Add(subViewConfigure.ViewType, subViewNode);
            viewNode.Nexts.Add(subViewNode);
            subViewNode.Previous = viewNode;
            Assembly(subViewNode, subViewConfigure);
        }
    }

    public ViewNode this[ViewType viewType] => viewNodes[viewType];

    public bool TryGetStackWithCheckTip(ViewType viewType, out Stack<ViewNode> stack)
    {
        stack = null;
        ViewNode node = viewNodes[viewType];
        if (node.Value.ViewCheck != null && !node.Value.ViewCheck.IsFuncOpenWithTip())
        {
            return false;
        }
        stack = new Stack<ViewNode>();
        stack.Push(node);
        while (node.Previous != null)
        {
            node = node.Previous;
            if (node.Value.ViewCheck != null && !node.Value.ViewCheck.IsFuncOpenWithTip())
            {
                return false;
            }
            stack.Push(node);
        }
        return true;
    }

    public Stack<ViewNode> GetStack(ViewType viewType)
    {
        ViewNode node = viewNodes[viewType];
        Stack<ViewNode> stack = new Stack<ViewNode>();
        stack.Push(node);
        while (node.Previous != null)
        {
            node = node.Previous;
            stack.Push(node);
        }
        return stack;
    }
}
