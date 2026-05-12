using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

public class PrefabSortingOrderScaler : EditorWindow
{
    private List<GameObject> targetPrefabs = new List<GameObject>();
    private int minOrder = 0;
    private int maxOrder = 19;
    private Vector2 scrollPosition;
    private string componentStatus = "等待处理...";

    [MenuItem("Tools/调整Prefab层级排序值")]
    public static void ShowWindow()
    {
        GetWindow<PrefabSortingOrderScaler>("Prefab Sorting Order 缩放工具");
    }

    private void OnGUI()
    {
        GUILayout.Label("将多个 Prefab 下所有 Renderer 的 Sorting Order 重新映射到指定范围", EditorStyles.wordWrappedLabel);
        EditorGUILayout.Space();

        // 显示组件状态
        EditorGUILayout.HelpBox(componentStatus, MessageType.Info);
        
        // 显示和编辑 Prefab 列表
        GUILayout.Label("目标 Prefab 列表:", EditorStyles.boldLabel);
        
        this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition, GUILayout.Height(150));
        
        for (int i = this.targetPrefabs.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal();
            
            GameObject newPrefab = (GameObject)EditorGUILayout.ObjectField(this.targetPrefabs[i], typeof(GameObject), false);
            if (newPrefab != this.targetPrefabs[i])
            {
                this.targetPrefabs[i] = newPrefab;
            }
            
            if (GUILayout.Button("移除", GUILayout.Width(50)))
            {
                this.targetPrefabs.RemoveAt(i);
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
        
        // 添加 Prefab 的按钮区域
        EditorGUILayout.BeginHorizontal();
        GameObject prefabToAdd = (GameObject)EditorGUILayout.ObjectField("拖入新的 Prefab", null, typeof(GameObject), false);
        if (prefabToAdd != null && !this.targetPrefabs.Contains(prefabToAdd))
        {
            this.targetPrefabs.Add(prefabToAdd);
        }
        
        if (GUILayout.Button("清空列表", GUILayout.Width(80)))
        {
            this.targetPrefabs.Clear();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // 参数设置
        this.minOrder = EditorGUILayout.IntField("最小层级 (Min Order)", this.minOrder);
        this.maxOrder = EditorGUILayout.IntField("最大层级 (Max Order)", this.maxOrder);
        
        if (this.maxOrder < this.minOrder)
            this.maxOrder = this.minOrder;
        
        // SetSortingLayer 组件设置
        EditorGUILayout.Space();
        GUILayout.Label("SetSortingLayer 组件设置:", EditorStyles.boldLabel);
        
        int setOrderInLayer = EditorGUILayout.IntField("orderInLayer", 0);
        bool setCanvas = EditorGUILayout.Toggle("setCanvas", true);
        bool addOrderTo = EditorGUILayout.Toggle("addOrderTo", true);
        bool addCanvasLayer = EditorGUILayout.Toggle("addCanvasLayer", true);
        
        EditorGUILayout.Space();
        
        // 额外选项
        GUILayout.Label("选项:", EditorStyles.boldLabel);
        bool processEachIndependently = EditorGUILayout.Toggle("每个Prefab独立处理", true);
        bool addSetSortingLayerComponent = EditorGUILayout.Toggle("自动添加/更新 SetSortingLayer 组件", true);
        
        // 显示组件查找按钮
        if (GUILayout.Button("检测 SetSortingLayer 组件", GUILayout.Width(200)))
        {
            DetectSetSortingLayerComponent();
        }
        
        EditorGUILayout.Space();
        
        // 添加批量拖拽区域
        HandleDragAndDrop();
        
        EditorGUILayout.Space();
        
        // 执行按钮
        EditorGUI.BeginDisabledGroup(this.targetPrefabs == null || this.targetPrefabs.Count == 0);
        if (GUILayout.Button(string.Format("批量处理 ({0} 个 Prefab)", this.targetPrefabs.Count), GUILayout.Height(30)))
        {
            this.BatchApplySortingOrderMapping(processEachIndependently, addSetSortingLayerComponent, setOrderInLayer, setCanvas, addOrderTo, addCanvasLayer);
        }
        EditorGUI.EndDisabledGroup();
        
        // 显示提示信息
        EditorGUILayout.HelpBox(
            "提示：\n" +
            "• 直接拖入 Project 窗口中的 Prefab 资源到上方列表或下方区域\n" +
            "• 每个 Prefab 会独立处理其内部的层级顺序\n" +
            "• 会自动为根节点添加/更新 SetSortingLayer 组件\n" +
            "• 处理前建议备份或使用版本控制\n" +
            "• 默认层级范围: 0-19, orderInLayer默认: 0", 
            MessageType.Info);
    }
    
    private void DetectSetSortingLayerComponent()
    {
        System.Type type = FindSetSortingLayerType();
        if (type != null)
        {
            componentStatus = string.Format("✓ 找到 SetSortingLayer 组件！完整类型: {0}, 程序集: {1}", 
                type.FullName, type.Assembly.FullName);
        }
        else
        {
            componentStatus = "✗ 未找到 SetSortingLayer 组件！请确保 SetSortingLayer.cs 脚本已存在于项目中并编译成功。";
        }
        Repaint();
    }

    private void HandleDragAndDrop()
    {
        Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "拖拽 Prefab 到这里批量添加", EditorStyles.helpBox);
        
        Event evt = Event.current;
        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    return;
                
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                
                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (UnityEngine.Object obj in DragAndDrop.objectReferences)
                    {
                        GameObject go = obj as GameObject;
                        if (go != null && !this.targetPrefabs.Contains(go))
                        {
                            this.targetPrefabs.Add(go);
                        }
                    }
                }
                break;
        }
    }

    private void BatchApplySortingOrderMapping(bool processEachIndependently, bool addSetSortingLayerComponent, int orderInLayer, bool setCanvas, bool addOrderTo, bool addCanvasLayer)
    {
        int successCount = 0;
        int failCount = 0;
        List<string> failedPrefabs = new List<string>();
        
        foreach (GameObject prefab in this.targetPrefabs)
        {
            if (prefab == null)
            {
                failCount++;
                failedPrefabs.Add("null");
                continue;
            }
            
            if (this.ApplySortingOrderToPrefab(prefab, processEachIndependently, addSetSortingLayerComponent, orderInLayer, setCanvas, addOrderTo, addCanvasLayer))
            {
                successCount++;
            }
            else
            {
                failCount++;
                failedPrefabs.Add(prefab.name);
            }
        }
        
        // 显示处理结果
        string resultMessage = string.Format("批量处理完成！\n成功: {0} 个\n失败: {1} 个", successCount, failCount);
        
        if (failCount > 0)
        {
            resultMessage = resultMessage + string.Format("\n失败的 Prefab: {0}", string.Join(", ", failedPrefabs.ToArray()));
            EditorUtility.DisplayDialog("处理结果", resultMessage, "确定");
        }
        else
        {
            EditorUtility.DisplayDialog("处理结果", resultMessage, "确定");
        }
        
        AssetDatabase.Refresh();
    }

    private bool ApplySortingOrderToPrefab(GameObject prefab, bool processIndependently, bool addSetSortingLayerComponent, int orderInLayer, bool setCanvas, bool addOrderTo, bool addCanvasLayer)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Prefab 为空，跳过处理");
            return false;
        }
        
        string path = AssetDatabase.GetAssetPath(prefab);
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError(string.Format("无法获取 Prefab 路径: {0}，请确保拖入的是 Project 窗口中的 Prefab 资源。", prefab.name));
            return false;
        }
        
        // 检查是否是真正的 Prefab 资源
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) == null)
        {
            Debug.LogError(string.Format("'{0}' 不是一个有效的 Prefab 资源。", prefab.name));
            return false;
        }
        
        GameObject prefabInstance = null;
        
        try
        {
            prefabInstance = PrefabUtility.LoadPrefabContents(path);
            
            // 处理 SetSortingLayer 组件
            if (addSetSortingLayerComponent)
            {
                AddOrUpdateSetSortingLayerComponent(prefabInstance, orderInLayer, setCanvas, addOrderTo, addCanvasLayer);
            }
            
            // 收集所有 Renderer 组件
            List<Renderer> renderers = new List<Renderer>();
            this.CollectRenderers(prefabInstance.transform, renderers);
            
            if (renderers.Count == 0)
            {
                Debug.LogWarning(string.Format("Prefab '{0}' 下没有找到任何 Renderer 组件，跳过处理。", prefab.name));
                return false;
            }
            
            if (processIndependently)
            {
                // 每个 Prefab 独立处理
                this.ProcessRenderersIndependently(renderers);
            }
            else
            {
                // 全局处理（暂不实现，保持独立处理方式）
                this.ProcessRenderersIndependently(renderers);
            }
            
            // 保存修改后的 Prefab
            PrefabUtility.SaveAsPrefabAsset(prefabInstance, path);
            
            Debug.Log(string.Format("成功处理 Prefab: '{0}'，共处理 {1} 个 Renderer，层级范围 [{2}, {3}]", prefab.name, renderers.Count, this.minOrder, this.maxOrder));
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError(string.Format("处理 Prefab '{0}' 时出错: {1}", prefab.name, e.Message));
            Debug.LogError(e.StackTrace);
            return false;
        }
        finally
        {
            if (prefabInstance != null)
            {
                PrefabUtility.UnloadPrefabContents(prefabInstance);
            }
        }
    }

    private void AddOrUpdateSetSortingLayerComponent(GameObject rootObject, int orderInLayer, bool setCanvas, bool addOrderTo, bool addCanvasLayer)
    {
        if (rootObject == null) return;
        
        // 查找 SetSortingLayer 类型
        System.Type setSortingLayerType = FindSetSortingLayerType();
        
        if (setSortingLayerType == null)
        {
            Debug.LogError("未找到 SetSortingLayer 组件类型！请确保 SetSortingLayer.cs 脚本已存在于项目中并且编译没有错误。");
            Debug.Log("尝试手动查找 SetSortingLayer 组件...");
            
            // 尝试通过 GameObject 查找现有组件
            Component[] allComponents = rootObject.GetComponents<Component>();
            foreach (Component comp in allComponents)
            {
                if (comp != null && comp.GetType().Name == "SetSortingLayer")
                {
                    Debug.Log(string.Format("找到现有的 SetSortingLayer 组件，类型: {0}", comp.GetType().FullName));
                    setSortingLayerType = comp.GetType();
                    break;
                }
            }
            
            if (setSortingLayerType == null)
            {
                Debug.LogError("无法找到或创建 SetSortingLayer 组件，将跳过组件处理。");
                return;
            }
        }
        
        // 查找或添加 SetSortingLayer 组件
        Component setSortingLayerComponent = rootObject.GetComponent(setSortingLayerType);
        if (setSortingLayerComponent == null)
        {
            // 添加新组件
            setSortingLayerComponent = rootObject.AddComponent(setSortingLayerType);
            Debug.Log(string.Format("为 Prefab '{0}' 的根节点添加了 SetSortingLayer 组件", rootObject.name));
        }
        else
        {
            Debug.Log(string.Format("Prefab '{0}' 的根节点已有 SetSortingLayer 组件，正在更新配置", rootObject.name));
        }
        
        // 使用反射设置字段值
        FieldInfo orderInLayerField = setSortingLayerType.GetField("orderInLayer");
        if (orderInLayerField != null)
        {
            orderInLayerField.SetValue(setSortingLayerComponent, orderInLayer);
            Debug.Log(string.Format("设置 orderInLayer = {0}", orderInLayer));
        }
        else
        {
            Debug.LogWarning("SetSortingLayer 组件中没有找到 orderInLayer 字段");
        }
        
        FieldInfo setCanvasField = setSortingLayerType.GetField("setCanvas");
        if (setCanvasField != null)
        {
            setCanvasField.SetValue(setSortingLayerComponent, setCanvas);
            Debug.Log(string.Format("设置 setCanvas = {0}", setCanvas));
        }
        else
        {
            Debug.LogWarning("SetSortingLayer 组件中没有找到 setCanvas 字段");
        }
        
        FieldInfo addOrderToField = setSortingLayerType.GetField("addOrderTo");
        if (addOrderToField != null)
        {
            addOrderToField.SetValue(setSortingLayerComponent, addOrderTo);
            Debug.Log(string.Format("设置 addOrderTo = {0}", addOrderTo));
        }
        else
        {
            Debug.LogWarning("SetSortingLayer 组件中没有找到 addOrderTo 字段");
        }
        
        FieldInfo addCanvasLayerField = setSortingLayerType.GetField("addCanvasLayer");
        if (addCanvasLayerField != null)
        {
            addCanvasLayerField.SetValue(setSortingLayerComponent, addCanvasLayer);
            Debug.Log(string.Format("设置 addCanvasLayer = {0}", addCanvasLayer));
        }
        else
        {
            Debug.LogWarning("SetSortingLayer 组件中没有找到 addCanvasLayer 字段");
        }
        
        // 标记组件为已修改
        EditorUtility.SetDirty(setSortingLayerComponent);
    }

    private System.Type FindSetSortingLayerType()
    {
        // 方法1：通过完整名称查找（包括程序集限定名）
        System.Type type = System.Type.GetType("SetSortingLayer, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
        if (type != null) return type;
        
        // 方法2：通过简单名称查找
        type = System.Type.GetType("SetSortingLayer");
        if (type != null) return type;
        
        // 方法3：遍历所有程序集查找
        foreach (Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            // 跳过系统程序集
            if (assembly.FullName.StartsWith("System") || assembly.FullName.StartsWith("mscorlib") || 
                assembly.FullName.StartsWith("UnityEngine") || assembly.FullName.StartsWith("UnityEditor"))
                continue;
            
            try
            {
                // 查找简单名称
                type = assembly.GetType("SetSortingLayer");
                if (type != null) 
                {
                    Debug.Log(string.Format("在程序集 {0} 中找到 SetSortingLayer", assembly.FullName));
                    return type;
                }
                
                // 查找带命名空间的名称
                type = assembly.GetType("SetSortingLayer");
                if (type != null) 
                {
                    Debug.Log(string.Format("在程序集 {0} 中找到 SetSortingLayer", assembly.FullName));
                    return type;
                }
                
                // 遍历所有类型查找名称匹配的
                foreach (System.Type t in assembly.GetTypes())
                {
                    if (t.Name == "SetSortingLayer")
                    {
                        Debug.Log(string.Format("在程序集 {0} 中找到类型 {1}", assembly.FullName, t.FullName));
                        return t;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log(string.Format("无法加载程序集 {0}: {1}", assembly.FullName, ex.Message));
            }
        }
        
        return null;
    }

    private void ProcessRenderersIndependently(List<Renderer> renderers)
    {
        if (renderers.Count == 0) return;
        
        // 获取所有原始层级值
        List<int> originalOrders = new List<int>();
        foreach (Renderer rend in renderers)
        {
            originalOrders.Add(rend.sortingOrder);
        }
        
        // 找出最小和最大值
        int curMin = int.MaxValue;
        int curMax = int.MinValue;
        
        foreach (int order in originalOrders)
        {
            if (order < curMin) curMin = order;
            if (order > curMax) curMax = order;
        }
        
        if (curMin == curMax)
        {
            // 所有层级相同，直接设置为 minOrder
            foreach (Renderer rend in renderers)
            {
                rend.sortingOrder = this.minOrder;
            }
        }
        else
        {
            // 线性映射，保持相对顺序
            foreach (Renderer rend in renderers)
            {
                int oldOrder = rend.sortingOrder;
                float t = (float)(oldOrder - curMin) / (float)(curMax - curMin);
                int newOrder = Mathf.RoundToInt(Mathf.Lerp((float)this.minOrder, (float)this.maxOrder, t));
                rend.sortingOrder = newOrder;
            }
        }
    }

    private void CollectRenderers(Transform root, List<Renderer> outList)
    {
        Renderer rend = root.GetComponent<Renderer>();
        if (rend != null)
        {
            outList.Add(rend);
        }
        
        foreach (Transform child in root)
        {
            this.CollectRenderers(child, outList);
        }
    }
}