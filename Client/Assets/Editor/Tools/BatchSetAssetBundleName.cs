using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class BatchSetAssetBundleName : EditorWindow
{
    private string targetDirectory = "Assets";
    private string assetBundleName = "";
    private bool includeSubDirectories = true;
    private Vector2 scrollPosition;
    private List<AssetInfo> assetList = new List<AssetInfo>();
    private bool showPreview = false;

    [System.Serializable]
    private class AssetInfo
    {
        public string path;
        public string currentABName;
        public string newABName;
    }

    [MenuItem("Tools/批量设置AssetBundle名称")]
    public static void ShowWindow()
    {
        BatchSetAssetBundleName window = GetWindow<BatchSetAssetBundleName>("批量设置AssetBundle名称");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("批量设置AssetBundle名称", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // 目录设置
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("目标目录:", GUILayout.Width(80));
        targetDirectory = EditorGUILayout.TextField(targetDirectory);
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("选择目标目录", "Assets", "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                targetDirectory = GetRelativePath(selectedPath);
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        // AssetBundle名称设置
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("AB名称:", GUILayout.Width(80));
        assetBundleName = EditorGUILayout.TextField(assetBundleName);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        // 选项
        includeSubDirectories = EditorGUILayout.Toggle("包含子目录", includeSubDirectories);
        
        GUILayout.Space(10);

        // 按钮
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("预览", GUILayout.Height(30)))
        {
            PreviewAssets();
            showPreview = true;
        }

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("应用设置", GUILayout.Height(30)))
        {
            ApplyAssetBundleName();
        }
        GUI.backgroundColor = Color.white;

        if (GUILayout.Button("清除所有AB名称", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("确认清除", "确定要清除所有资源的AssetBundle名称吗？", "确定", "取消"))
            {
                ClearAllAssetBundleNames();
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        // 预览区域
        if (showPreview && assetList.Count > 0)
        {
            GUILayout.Label(string.Format("预览 (共 {0} 个资源)", assetList.Count), EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
            
            foreach (AssetInfo asset in assetList)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(asset.path, GUILayout.Width(300));
                
                string currentAB = string.IsNullOrEmpty(asset.currentABName) ? "<无>" : asset.currentABName;
                EditorGUILayout.LabelField("当前: " + currentAB, GUILayout.Width(150));
                EditorGUILayout.LabelField("->", GUILayout.Width(20));
                
                string newAB = string.IsNullOrEmpty(asset.newABName) ? "<无>" : asset.newABName;
                EditorGUILayout.LabelField("新: " + newAB, GUILayout.Width(150));
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
        }
        else if (showPreview)
        {
            GUILayout.Label("没有找到资源", EditorStyles.centeredGreyMiniLabel);
        }

        GUILayout.Space(10);
        
        string helpText = "提示：\n1. AssetBundle名称支持路径格式，例如：\"prefabs/characters\"\n2. 设置为空字符串会清除资源的AssetBundle名称\n3. 操作会立即生效并保存";
        EditorGUILayout.HelpBox(helpText, MessageType.Info);
    }

    private void PreviewAssets()
    {
        assetList.Clear();
        
        if (string.IsNullOrEmpty(targetDirectory) || !Directory.Exists(targetDirectory))
        {
            EditorUtility.DisplayDialog("错误", string.Format("目录不存在: {0}", targetDirectory), "确定");
            return;
        }

        // 获取所有资源
        string[] allAssets = GetAssetsInDirectory(targetDirectory, includeSubDirectories);
        
        foreach (string assetPath in allAssets)
        {
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (importer != null)
            {
                AssetInfo info = new AssetInfo();
                info.path = assetPath;
                info.currentABName = importer.assetBundleName;
                info.newABName = assetBundleName;
                assetList.Add(info);
            }
        }
    }

    private void ApplyAssetBundleName()
    {
        if (assetList.Count == 0)
        {
            PreviewAssets();
        }

        if (assetList.Count == 0)
        {
            EditorUtility.DisplayDialog("提示", "没有找到任何资源", "确定");
            return;
        }

        bool confirmed = EditorUtility.DisplayDialog("确认操作", 
            string.Format("将要设置 {0} 个资源的AssetBundle名称为:\n\"{1}\"\n\n确定要继续吗？", assetList.Count, assetBundleName), 
            "确定", "取消");
        
        if (!confirmed)
        {
            return;
        }

        int changedCount = 0;
        int skippedCount = 0;

        try
        {
            foreach (AssetInfo asset in assetList)
            {
                AssetImporter importer = AssetImporter.GetAtPath(asset.path);
                if (importer != null)
                {
                    // 只有当名称不同时才修改
                    if (importer.assetBundleName != assetBundleName)
                    {
                        importer.assetBundleName = assetBundleName;
                        importer.SaveAndReimport();
                        changedCount++;
                    }
                    else
                    {
                        skippedCount++;
                    }
                }
            }

            // 刷新
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("完成", 
                string.Format("设置完成！\n修改: {0} 个资源\n跳过: {1} 个资源（已是目标名称）", changedCount, skippedCount), 
                "确定");
            
            // 更新预览
            PreviewAssets();
        }
        catch (System.Exception e)
        {
            Debug.LogError(string.Format("批量设置时发生错误: {0}", e.Message));
            EditorUtility.DisplayDialog("错误", string.Format("操作失败: {0}", e.Message), "确定");
        }
    }

    private void ClearAllAssetBundleNames()
    {
        if (assetList.Count == 0)
        {
            PreviewAssets();
        }

        if (assetList.Count == 0)
        {
            EditorUtility.DisplayDialog("提示", "没有找到任何资源", "确定");
            return;
        }

        int clearedCount = 0;
        
        foreach (AssetInfo asset in assetList)
        {
            AssetImporter importer = AssetImporter.GetAtPath(asset.path);
            if (importer != null && !string.IsNullOrEmpty(importer.assetBundleName))
            {
                importer.assetBundleName = "";
                importer.SaveAndReimport();
                clearedCount++;
            }
        }
        
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("完成", string.Format("已清除 {0} 个资源的AssetBundle名称", clearedCount), "确定");
        
        // 更新预览
        PreviewAssets();
    }

    private string[] GetAssetsInDirectory(string directory, bool includeSubdirs)
    {
        // 获取所有资源文件
        string[] guids;
        if (includeSubdirs)
        {
            guids = AssetDatabase.FindAssets("", new string[] { directory });
        }
        else
        {
            string[] allGuids = AssetDatabase.FindAssets("", new string[] { directory });
            List<string> filteredGuids = new List<string>();
            
            foreach (string guid in allGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string dirName = Path.GetDirectoryName(path);
                if (dirName == directory)
                {
                    filteredGuids.Add(guid);
                }
            }
            
            guids = filteredGuids.ToArray();
        }

        List<string> assetPaths = new List<string>();
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            
            // 排除文件夹和特殊文件
            if (!AssetDatabase.IsValidFolder(path) && 
                !path.EndsWith(".meta") && 
                !path.EndsWith(".cs") &&
                !path.EndsWith(".shader") &&
                !path.EndsWith(".asmdef"))
            {
                assetPaths.Add(path);
            }
        }
        
        return assetPaths.ToArray();
    }

    private string GetRelativePath(string absolutePath)
    {
        if (absolutePath.StartsWith(Application.dataPath))
        {
            return "Assets" + absolutePath.Substring(Application.dataPath.Length);
        }
        return absolutePath;
    }
}