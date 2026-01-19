using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public partial class RedPointTreeManager
{
    private void InitConfigure()
    {
        // 新建一颗红点树
        {
            // ERedPoint.Root.NewTree(ERedPointTree.Root)
            //     .TreeLinker(() => true, ERedPointTree.Test1).Linker(
            //         ERedPoint.Tab1.Condition(() => true).Linker(
            //             ERedPoint.Item1.Condition(() => true)).Linker(
            //             ERedPoint.Item2.Condition(() => true)))
            //     .TreeLinker(() => true, ERedPointTree.Test1).Linker(
            //         ERedPoint.Tab2.Condition(() => true).Linker(
            //             ERedPoint.Item3.Condition(() => true))).Linker(
            //         ERedPoint.Item1.Condition(() => true))
            //     .Build();
        }
        // ...
        {
            
        }
    }
}

public enum ERedPointTree
{
    Root = 1,
    Test1 = 2,
}

public enum ERedPoint
{
    Root = 1,
    Tab1 = 2,
    Tab2 = 3,
    Item1 = 4,
    Item2 = 5,
    Item3 = 6,
}

public static class ERedPointExtensions
{
    // private static Dictionary<ERedPoint, RedPointNode> nodeDict;
    // private static Dictionary<ERedPointTree, RedPointTree> treeDict;
    // private static ERedPoint root;
    //
    // /// 新建树
    // public static ERedPoint NewTree(this ERedPoint self, ERedPointTree belongToTree)
    // {
    //     root = self;
    //     nodeDict = new Dictionary<ERedPoint, RedPointNode>();
    //     treeDict = new Dictionary<ERedPointTree, RedPointTree>();
    //     
    //     RedPointNode nodeSelf = new RedPointNode(self, belongToTree);
    //     nodeDict.Add(self, nodeSelf);
    //     
    //     RedPointTree redPointTree = new RedPointTree(belongToTree, self);
    //     treeDict.Add(belongToTree, redPointTree);
    //
    //     return self;
    // }
    // /// 构建完成，导入 manager
    // public static void Build(this ERedPoint self)
    // {
    //     StringBuilder sbSpace = new StringBuilder();
    // }
    //
    // /// 连接节点
    // public static ERedPoint Linker(this ERedPoint self, ERedPoint child)
    // {
    //     if (!nodeDict.TryGetValue(self, out RedPointNode nodeParent))
    //     {
    //         Debug.LogError($"未先初始化 {self}");
    //         return default(ERedPoint);
    //     }
    //     RedPointNode nodeChild = new RedPointNode(child, nodeDict[self].BelongToTree);
    //     nodeDict.Add(child, nodeChild);
    //     
    //     nodeChild.AddParentNode(nodeParent);
    //     nodeParent.AddChildNode(nodeChild);
    //     return self;
    // }
    // /// 添加节点条件
    // public static ERedPoint Condition(this ERedPoint self, Func<bool> displayCondition)
    // {
    //     if (!nodeDict.TryGetValue(self, out RedPointNode nodeCurrent))
    //     {
    //         Debug.LogError($"未先初始化 {self}");
    //         return default(ERedPoint);
    //     }
    //     nodeCurrent.AddCondition(displayCondition);
    //     return self;
    // }
    //
    // /// 连接子树
    // public static ERedPoint TreeLinker(this ERedPoint self, Func<bool> linkCondition, ERedPointTree belongToTree)
    // {
    //     if (!nodeDict.TryGetValue(self, out RedPointNode nodeCurrent))
    //     {
    //         Debug.LogError($"未先初始化 {self}");
    //         return default(ERedPoint);
    //     }
    //     RedPointNode nodeChild = new RedPointNode(child, belongToTree);
    //     nodeDict.Add(child, nodeChild);
    //     
    //     RedPointTree redPointTree = new RedPointTree(belongToTree, self);
    //     treeDict.Add(belongToTree, redPointTree);
    //     
    //     nodeChild.AddParentNode(nodeCurrent);
    //     nodeCurrent.AddChildTree(redPointTree);
    //     return self;
    // }
}