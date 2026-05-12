using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Text;

public class ShaderBatchModifier : EditorWindow
{
    [System.Serializable]
    public class ShaderMapping
    {
        public List<Shader> originalShaders = new List<Shader>();
        public Shader targetShader;
        public ShaderMapping() { originalShaders = new List<Shader>(); }
    }

    [System.Serializable]
    public class ModificationConfig
    {
        public List<ShaderMappingSerializable> shaderMappings = new List<ShaderMappingSerializable>();
        public int stencilComparisonValue = 5;
        public int stencilIDValue = 10;
        public List<string> prefabPaths = new List<string>();
        public ModificationConfig() 
        { 
            shaderMappings = new List<ShaderMappingSerializable>();
            prefabPaths = new List<string>();
        }
    }

    [System.Serializable]
    public class ShaderMappingSerializable
    {
        public List<string> originalShaderNames = new List<string>();
        public string targetShaderName;
        public ShaderMappingSerializable() { originalShaderNames = new List<string>(); }
        public ShaderMappingSerializable(List<Shader> originals, Shader target)
        {
            originalShaderNames = new List<string>();
            foreach (Shader s in originals)
                if (s != null) originalShaderNames.Add(s.name);
            targetShaderName = (target != null) ? target.name : "";
        }
    }

    private List<ShaderMapping> shaderMappings = new List<ShaderMapping>();
    private List<GameObject> targetPrefabs = new List<GameObject>();
    private Vector2 scrollPosition;
    private Vector2 mappingScrollPosition;
    private int stencilComparisonValue = 5;
    private int stencilIDValue = 10;
    private const string CONFIG_FOLDER = "ShaderModifierConfigs";
    private List<string> savedConfigs = new List<string>();
    private int selectedConfigIndex = -1;
    private Dictionary<string, Shader> shaderCache = new Dictionary<string, Shader>();
    private List<bool> mappingFoldouts = new List<bool>();
    private bool dryRun = false;

    [MenuItem("Tools/批量修改 Prefab Shader")]
    public static void ShowWindow()
    {
        GetWindow<ShaderBatchModifier>("批量修改 Prefab Shader");
    }

    private void OnEnable()
    {
        LoadSavedConfigsList();
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        DrawConfigManagementUI();
        GUILayout.Space(10);
        DrawShaderMappingUI();
        GUILayout.Space(20);
        DrawStencilConfigUI();
        GUILayout.Space(20);
        DrawPrefabListUI();
        GUILayout.Space(20);
        DrawActionButtons();
        EditorGUILayout.EndScrollView();
    }

    private void DrawConfigManagementUI()
    {
        GUILayout.Label("配置管理", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if (savedConfigs.Count > 0)
        {
            string[] configNames = savedConfigs.ToArray();
            int newIndex = EditorGUILayout.Popup("加载配置", selectedConfigIndex, configNames);
            if (newIndex != selectedConfigIndex)
            {
                selectedConfigIndex = newIndex;
                if (selectedConfigIndex >= 0 && selectedConfigIndex < savedConfigs.Count)
                    LoadConfig(savedConfigs[selectedConfigIndex]);
            }
        }
        else
        {
            EditorGUILayout.LabelField("加载配置", "无保存的配置");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("保存配置")) SaveConfigDialog();
        if (GUILayout.Button("另存为配置")) SaveConfigAsDialog();
        if (GUILayout.Button("刷新配置列表")) LoadSavedConfigsList();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawShaderMappingUI()
    {
        GUILayout.Label("Shader 映射配置", EditorStyles.boldLabel);
        while (mappingFoldouts.Count < shaderMappings.Count) mappingFoldouts.Add(true);
        while (mappingFoldouts.Count > shaderMappings.Count) mappingFoldouts.RemoveAt(mappingFoldouts.Count - 1);

        mappingScrollPosition = EditorGUILayout.BeginScrollView(mappingScrollPosition, GUILayout.MaxHeight(300));
        for (int i = 0; i < shaderMappings.Count; i++)
        {
            ShaderMapping mapping = shaderMappings[i];
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            string foldoutLabel = "映射组 " + (i + 1).ToString();
            if (mapping.targetShader != null)
                foldoutLabel = foldoutLabel + " (目标: " + mapping.targetShader.name + ")";
            else
                foldoutLabel = foldoutLabel + " (目标: 未设置)";
            mappingFoldouts[i] = EditorGUILayout.Foldout(mappingFoldouts[i], foldoutLabel, true);
            if (GUILayout.Button("删除", GUILayout.Width(60)))
            {
                shaderMappings.RemoveAt(i);
                i--;
                continue;
            }
            EditorGUILayout.EndHorizontal();

            if (mappingFoldouts[i])
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);
                EditorGUILayout.LabelField("目标 Shader:", GUILayout.Width(80));
                mapping.targetShader = (Shader)EditorGUILayout.ObjectField(mapping.targetShader, typeof(Shader), false);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);
                EditorGUILayout.LabelField("原始 Shader 列表:", GUILayout.Width(80));
                EditorGUILayout.LabelField("(这些 Shader 都将被替换为目标 Shader)", EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();

                for (int j = 0; j < mapping.originalShaders.Count; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(40);
                    string shaderLabel = "Shader " + (j + 1).ToString();
                    mapping.originalShaders[j] = (Shader)EditorGUILayout.ObjectField(shaderLabel, mapping.originalShaders[j], typeof(Shader), false);
                    if (GUILayout.Button("移除", GUILayout.Width(50)))
                    {
                        mapping.originalShaders.RemoveAt(j);
                        j--;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("添加原始 Shader", GUILayout.Width(120)))
                    mapping.originalShaders.Add(null);
                int valid = 0;
                foreach (Shader s in mapping.originalShaders) if (s != null) valid++;
                EditorGUILayout.LabelField("有效 Shader 数量: " + valid.ToString() + " / " + mapping.originalShaders.Count.ToString(), EditorStyles.miniLabel);
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加映射组")) shaderMappings.Add(new ShaderMapping());
        if (GUILayout.Button("清空所有映射"))
        {
            if (EditorUtility.DisplayDialog("确认", "清空所有映射？", "确定", "取消"))
                shaderMappings.Clear();
        }
        if (GUILayout.Button("验证 Shader 是否存在")) ValidateAllShaders();
        EditorGUILayout.EndHorizontal();

        int totalOrig = 0, validOrig = 0, validTarget = 0;
        foreach (ShaderMapping m in shaderMappings)
        {
            totalOrig += m.originalShaders.Count;
            foreach (Shader s in m.originalShaders) if (s != null) validOrig++;
            if (m.targetShader != null) validTarget++;
        }
        EditorGUILayout.HelpBox(
            "统计: " + shaderMappings.Count.ToString() + " 个映射组 | 原始 Shader: " + validOrig.ToString() + "/" + totalOrig.ToString() + " | 目标有效: " + validTarget.ToString(),
            MessageType.Info);
    }

    private void DrawStencilConfigUI()
    {
        GUILayout.Label("Stencil 参数配置", EditorStyles.boldLabel);
        stencilComparisonValue = EditorGUILayout.IntField("Stencil Comparison", stencilComparisonValue);
        stencilIDValue = EditorGUILayout.IntField("Stencil ID", stencilIDValue);
        dryRun = EditorGUILayout.Toggle("预览模式 (不实际保存)", dryRun);
        EditorGUILayout.HelpBox("注意：直接修改原材质，不会创建新实例。预览模式可查看会修改哪些材质但不保存。", MessageType.Warning);
    }

    private void DrawPrefabListUI()
    {
        GUILayout.Label("目标 Prefab 列表", EditorStyles.boldLabel);
        for (int i = 0; i < targetPrefabs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            string prefabLabel = "Prefab " + (i + 1).ToString();
            targetPrefabs[i] = (GameObject)EditorGUILayout.ObjectField(prefabLabel, targetPrefabs[i], typeof(GameObject), false);
            if (GUILayout.Button("删除", GUILayout.Width(60)))
            {
                targetPrefabs.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加 Prefab")) targetPrefabs.Add(null);
        if (GUILayout.Button("从文件夹选择 Prefab"))
        {
            string folder = EditorUtility.OpenFolderPanel("选择包含 Prefab 的文件夹", "Assets", "");
            if (!string.IsNullOrEmpty(folder)) AddPrefabsFromFolder(folder);
        }
        if (GUILayout.Button("清空列表"))
        {
            if (EditorUtility.DisplayDialog("确认", "清空所有Prefab？", "确定", "取消"))
                targetPrefabs.Clear();
        }
        EditorGUILayout.EndHorizontal();

        int validPrefab = 0;
        foreach (GameObject p in targetPrefabs) if (p != null) validPrefab++;
        EditorGUILayout.LabelField("有效 Prefab 数量: " + validPrefab.ToString() + " / " + targetPrefabs.Count.ToString());
    }

    private void DrawActionButtons()
    {
        GUI.backgroundColor = dryRun ? Color.yellow : Color.green;
        string buttonText = dryRun ? "预览修改 (Dry Run)" : "开始批量修改";
        if (GUILayout.Button(buttonText, GUILayout.Height(40))) BatchModifyPrefabs();
        GUI.backgroundColor = Color.white;
        EditorGUILayout.HelpBox(
            "工作流程：\n1. 遍历 Prefab 及其子物体的 Renderer 中的材质\n2. 如果材质的 Shader 匹配任一映射组的原始 Shader，则替换为目标 Shader\n3. 设置 Stencil 参数 (直接修改原材质)\n4. 预览模式仅输出日志，不保存任何修改",
            MessageType.Info);
    }

    private void AddPrefabsFromFolder(string folderPath)
    {
        if (folderPath.StartsWith(Application.dataPath))
        {
            string relativePath = "Assets" + folderPath.Substring(Application.dataPath.Length);
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { relativePath });
            int added = 0;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null && !targetPrefabs.Contains(prefab))
                {
                    targetPrefabs.Add(prefab);
                    added++;
                }
            }
            Debug.Log("从文件夹添加了 " + added.ToString() + " 个 Prefab (共找到 " + guids.Length.ToString() + ")");
            EditorUtility.DisplayDialog("添加完成", "添加了 " + added.ToString() + " 个 Prefab", "确定");
        }
        else
        {
            EditorUtility.DisplayDialog("错误", "文件夹必须在 Assets 目录内", "确定");
        }
    }

    private void ValidateAllShaders()
    {
        int missing = 0;
        int validGroups = 0;
        foreach (ShaderMapping m in shaderMappings)
        {
            bool ok = true;
            if (m.targetShader == null)
            {
                Debug.LogWarning("目标 Shader 为空");
                missing++;
                ok = false;
            }
            for (int i = 0; i < m.originalShaders.Count; i++)
            {
                if (m.originalShaders[i] == null)
                {
                    Debug.LogWarning("原始 Shader 索引 " + i.ToString() + " 为空");
                    missing++;
                    ok = false;
                }
            }
            if (ok && m.originalShaders.Count > 0) validGroups++;
        }
        EditorUtility.DisplayDialog("验证结果", "有效映射组: " + validGroups.ToString() + " / " + shaderMappings.Count.ToString() + "\n无效引用: " + missing.ToString(), "确定");
    }

    private void BatchModifyPrefabs()
    {
        if (shaderMappings.Count == 0)
        {
            EditorUtility.DisplayDialog("错误", "没有 Shader 映射组", "确定");
            return;
        }

        bool hasValidPrefab = false;
        foreach (GameObject p in targetPrefabs) if (p != null) { hasValidPrefab = true; break; }
        if (!hasValidPrefab)
        {
            EditorUtility.DisplayDialog("错误", "没有有效的 Prefab", "确定");
            return;
        }

        // 构建替换字典
        Dictionary<Shader, Shader> replacementMap = new Dictionary<Shader, Shader>();
        foreach (ShaderMapping mapping in shaderMappings)
        {
            if (mapping.targetShader == null) continue;
            foreach (Shader orig in mapping.originalShaders)
            {
                if (orig != null && !replacementMap.ContainsKey(orig))
                    replacementMap.Add(orig, mapping.targetShader);
            }
        }

        if (replacementMap.Count == 0)
        {
            EditorUtility.DisplayDialog("错误", "没有有效的 Shader 映射关系", "确定");
            return;
        }

        int totalModified = 0;
        int totalPrefabsProcessed = 0;
        int totalFailed = 0;
        List<string> processedLog = new List<string>();
        List<string> failedLog = new List<string>();

        for (int idx = 0; idx < targetPrefabs.Count; idx++)
        {
            GameObject prefab = targetPrefabs[idx];
            if (prefab == null) continue;

            string prefabPath = AssetDatabase.GetAssetPath(prefab);
            if (string.IsNullOrEmpty(prefabPath))
            {
                Debug.LogWarning("无法获取 Prefab 路径: " + prefab.name);
                totalFailed++;
                failedLog.Add(prefab.name + " (无路径)");
                continue;
            }

            float progress = (float)idx / (float)targetPrefabs.Count;
            string status = (dryRun ? "预览" : "处理") + ": " + prefab.name;
            EditorUtility.DisplayProgressBar(dryRun ? "预览 Shader 替换" : "批量修改 Prefab", status, progress);

            GameObject prefabInstance = null;
            try
            {
                prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);
                if (prefabInstance == null) throw new System.Exception("加载 Prefab 内容失败");

                int modifiedCount = ModifyGameObjectAndChildren(prefabInstance, replacementMap);
                if (modifiedCount > 0)
                {
                    if (!dryRun)
                    {
                        PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
                        Debug.Log("已保存 Prefab: " + prefab.name + ", 修改了 " + modifiedCount.ToString() + " 个材质");
                    }
                    else
                    {
                        Debug.Log("【预览】将修改 Prefab: " + prefab.name + ", 会修改 " + modifiedCount.ToString() + " 个材质");
                    }
                    totalModified += modifiedCount;
                    totalPrefabsProcessed++;
                    processedLog.Add(prefab.name + " (" + modifiedCount.ToString() + " 个材质)");
                }
                else
                {
                    Debug.Log("未发现需要修改的材质: " + prefab.name);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("处理 Prefab 失败: " + prefab.name + "\n" + e.Message);
                totalFailed++;
                failedLog.Add(prefab.name + " (" + e.Message + ")");
            }
            finally
            {
                if (prefabInstance != null) PrefabUtility.UnloadPrefabContents(prefabInstance);
            }
        }

        EditorUtility.ClearProgressBar();

        string result = (dryRun ? "预览" : "批量修改") + " 完成！\n成功处理 Prefab: " + totalPrefabsProcessed.ToString() + "\n修改材质总数: " + totalModified.ToString() + "\n失败: " + totalFailed.ToString() + "\n\n";
        if (processedLog.Count > 0)
        {
            result += "处理的 Prefab:\n";
            foreach (string s in processedLog) result += s + "\n";
            result += "\n";
        }
        if (failedLog.Count > 0)
        {
            result += "失败的 Prefab:\n";
            foreach (string s in failedLog) result += s + "\n";
        }
        EditorUtility.DisplayDialog(dryRun ? "预览结果" : "完成", result, "确定");
        Debug.Log(result);
        if (!dryRun) AssetDatabase.Refresh();
    }

    private int ModifyGameObjectAndChildren(GameObject obj, Dictionary<Shader, Shader> replacementMap)
    {
        int modifiedCount = 0;
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;
            bool anyChanged = false;
            for (int i = 0; i < materials.Length; i++)
            {
                Material mat = materials[i];
                if (mat == null) continue;
                Shader currentShader = mat.shader;
                if (currentShader == null) continue;

                Shader targetShader = null;
                if (replacementMap.TryGetValue(currentShader, out targetShader))
                {
                    if (targetShader == currentShader) continue;
                    // 直接修改材质
                    mat.shader = targetShader;
                    if (mat.HasProperty("_StencilComp"))
                        mat.SetFloat("_StencilComp", (float)stencilComparisonValue);
                    else
                        Debug.LogWarning("材质 " + mat.name + " 的 Shader " + targetShader.name + " 没有 _StencilComp 属性");
                    if (mat.HasProperty("_Stencil"))
                        mat.SetFloat("_Stencil", (float)stencilIDValue);
                    else
                        Debug.LogWarning("材质 " + mat.name + " 的 Shader " + targetShader.name + " 没有 _Stencil 属性");
                    anyChanged = true;
                    modifiedCount++;
                    Debug.Log("修改材质: " + mat.name + " , Shader: " + currentShader.name + " -> " + targetShader.name + ", StencilComp=" + stencilComparisonValue.ToString() + ", Stencil=" + stencilIDValue.ToString());
                }
            }
            if (anyChanged)
                renderer.sharedMaterials = materials;
        }
        return modifiedCount;
    }

    #region 配置保存与加载 (C#4.0 兼容)

    private void LoadSavedConfigsList()
    {
        savedConfigs.Clear();
        string folder = Path.Combine(Application.dataPath, CONFIG_FOLDER);
        if (Directory.Exists(folder))
        {
            string[] configFiles = Directory.GetFiles(folder, "*.xml");
            foreach (string file in configFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                savedConfigs.Add(fileName);
            }
        }
        savedConfigs.Sort();
        if (selectedConfigIndex >= savedConfigs.Count) selectedConfigIndex = -1;
    }

    private void SaveConfigDialog()
    {
        if (selectedConfigIndex >= 0 && selectedConfigIndex < savedConfigs.Count)
        {
            bool overwrite = EditorUtility.DisplayDialog("保存配置", "覆盖配置 \"" + savedConfigs[selectedConfigIndex] + "\"?", "覆盖", "取消");
            if (overwrite)
                SaveConfig(savedConfigs[selectedConfigIndex]);
        }
        else
        {
            SaveConfigAsDialog();
        }
    }

    private void SaveConfigAsDialog()
    {
        string name = EditorUtility.SaveFilePanel("保存配置", Path.Combine(Application.dataPath, CONFIG_FOLDER), "ShaderConfig", "xml");
        if (!string.IsNullOrEmpty(name))
        {
            string fileName = Path.GetFileNameWithoutExtension(name);
            SaveConfig(fileName);
        }
    }

    private void SaveConfig(string configName)
    {
        try
        {
            string folder = Path.Combine(Application.dataPath, CONFIG_FOLDER);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                AssetDatabase.Refresh();
            }

            ModificationConfig cfg = new ModificationConfig();
            foreach (ShaderMapping m in shaderMappings)
            {
                if (m.originalShaders.Count > 0 || m.targetShader != null)
                {
                    cfg.shaderMappings.Add(new ShaderMappingSerializable(m.originalShaders, m.targetShader));
                }
            }
            cfg.stencilComparisonValue = stencilComparisonValue;
            cfg.stencilIDValue = stencilIDValue;
            foreach (GameObject p in targetPrefabs)
            {
                if (p != null)
                {
                    string path = AssetDatabase.GetAssetPath(p);
                    if (!string.IsNullOrEmpty(path)) cfg.prefabPaths.Add(path);
                }
            }

            XmlSerializer serializer = new XmlSerializer(typeof(ModificationConfig));
            string configPath = Path.Combine(folder, configName + ".xml");
            using (StreamWriter writer = new StreamWriter(configPath, false, Encoding.UTF8))
            {
                serializer.Serialize(writer, cfg);
            }

            Debug.Log("配置已保存: " + configPath);
            EditorUtility.DisplayDialog("保存成功", "配置 \"" + configName + "\" 已保存", "确定");
            LoadSavedConfigsList();
            selectedConfigIndex = savedConfigs.IndexOf(configName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("保存配置失败: " + e.Message);
            EditorUtility.DisplayDialog("保存失败", e.Message, "确定");
        }
    }

    private void LoadConfig(string configName)
    {
        try
        {
            string folder = Path.Combine(Application.dataPath, CONFIG_FOLDER);
            string configPath = Path.Combine(folder, configName + ".xml");
            if (!File.Exists(configPath)) throw new FileNotFoundException("配置文件不存在");

            XmlSerializer serializer = new XmlSerializer(typeof(ModificationConfig));
            ModificationConfig cfg;
            using (StreamReader reader = new StreamReader(configPath, Encoding.UTF8))
            {
                cfg = (ModificationConfig)serializer.Deserialize(reader);
            }
            if (cfg == null) return;

            shaderMappings.Clear();
            targetPrefabs.Clear();
            int missingShaders = 0;
            foreach (ShaderMappingSerializable sm in cfg.shaderMappings)
            {
                ShaderMapping mapping = new ShaderMapping();
                foreach (string origName in sm.originalShaderNames)
                {
                    Shader s = FindShaderByName(origName);
                    if (s != null) mapping.originalShaders.Add(s);
                    else
                    {
                        Debug.LogWarning("找不到原始 Shader: " + origName);
                        missingShaders++;
                    }
                }
                Shader target = FindShaderByName(sm.targetShaderName);
                if (target != null) mapping.targetShader = target;
                else
                {
                    Debug.LogWarning("找不到目标 Shader: " + sm.targetShaderName);
                    missingShaders++;
                }
                if (mapping.originalShaders.Count > 0 || mapping.targetShader != null)
                    shaderMappings.Add(mapping);
            }
            stencilComparisonValue = cfg.stencilComparisonValue;
            stencilIDValue = cfg.stencilIDValue;
            int loadedPrefabs = 0;
            int missingPrefabs = 0;
            foreach (string prefabPath in cfg.prefabPaths)
            {
                GameObject p = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (p != null)
                {
                    targetPrefabs.Add(p);
                    loadedPrefabs++;
                }
                else
                {
                    Debug.LogWarning("无法加载 Prefab: " + prefabPath);
                    missingPrefabs++;
                }
            }
            EditorUtility.DisplayDialog("加载成功",
                "配置 \"" + configName + "\" 加载完成\nShader 映射组: " + shaderMappings.Count.ToString() +
                "\n缺失 Shader: " + missingShaders.ToString() +
                "\nPrefab 加载: " + loadedPrefabs.ToString() + " / " + (loadedPrefabs + missingPrefabs).ToString(),
                "确定");
        }
        catch (System.Exception e)
        {
            Debug.LogError("加载配置失败: " + e.Message);
            EditorUtility.DisplayDialog("加载失败", e.Message, "确定");
        }
    }

    private Shader FindShaderByName(string shaderName)
    {
        if (string.IsNullOrEmpty(shaderName)) return null;
        if (shaderCache.ContainsKey(shaderName))
            return shaderCache[shaderName];
        Shader shader = Shader.Find(shaderName);
        if (shader != null)
            shaderCache[shaderName] = shader;
        return shader;
    }

    #endregion
}