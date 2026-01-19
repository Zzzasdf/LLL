using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HybridCLR;
using UnityEngine;
using UniFramework.Event;
using YooAsset;

public class Boot : MonoBehaviour
{
    private static List<string> AOTMetaAssemblyFiles { get; } = new List<string>
    {
        // "mscorlib.dll",
        // "System.dll",
        // "System.Core.dll",
        // "YooAsset.dll",
        // "UniFramework.Event.dll",
        // "UniFramework.Machine.dll",
        // "UniFramework.Utility.dll",
        // "UniTask.dll",
    };
    private static Dictionary<string, TextAsset> s_assetDatas = new Dictionary<string, TextAsset>();
    
    /// <summary>
    /// 资源系统运行模式
    /// </summary>
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

    private ResourcePackage gamePackage;

    private static Assembly _hotUpdateAss;
    
    void Awake()
    {
        Debug.Log($"资源系统运行模式：{PlayMode}");
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        DontDestroyOnLoad(this.gameObject);
    }
    IEnumerator Start()
    {
        // 游戏管理器
        GameManager.Instance.Behaviour = this;

        // 初始化事件系统
        UniEvent.Initalize();

        // 初始化资源系统
        YooAssets.Initialize();

        // 加载更新页面
        var go = Resources.Load<GameObject>("PatchWindow");
        GameObject.Instantiate(go);

        // 开始补丁更新流程
        var operation = new PatchOperation("DefaultPackage", PlayMode);
        YooAssets.StartOperation(operation);
        yield return operation;

        // 设置默认的资源包
        gamePackage = YooAssets.GetPackage("DefaultPackage");
        YooAssets.SetDefaultPackage(gamePackage);

        yield return LoadDlls();
    }

    private IEnumerator LoadDlls()
    {
        var assets = new List<string> { "HotUpdate.dll" }.Concat(AOTMetaAssemblyFiles);
        // 逐个加载程序集字节数据
        foreach (var asset in assets)
        {
            Debug.Log($"正在加载程序集：{asset}");

            var handle = gamePackage.LoadAssetAsync<TextAsset>(asset);
            yield return handle;
            if (handle.Status == EOperationStatus.Succeed)
            {
                TextAsset textAsset = handle.AssetObject as TextAsset;
                s_assetDatas[asset] = textAsset;
                Debug.Log($"程序集加载成功：{asset}, 大小：{textAsset.bytes.Length} 字节");
            }
            else
            {
                Debug.LogError($"程序集加载失败：{asset}, 错误：{handle.LastError}");
            }
            // 释放句柄
            handle.Release();
        }
        LoadMetadataForAOTAssemblies();
        LoadHotUpdateDlls();
        
        // 切换到主页面场景
        SceneEventDefine.ChangeToHomeScene.SendEventMessage();
    }

    /// <summary>
    /// 补充元数据
    /// </summary>
    private void LoadMetadataForAOTAssemblies()
    {
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in AOTMetaAssemblyFiles)
        {
            byte[] dllBytes = s_assetDatas[aotDllName].bytes;
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
        }
    }

    private void LoadHotUpdateDlls()
    {
        if (s_assetDatas.Count == 0)
        {
            Debug.Log("程序集字节数据为空，跳过加载热更dll");
            return;
        }
        
        // 加载热更dll
#if !UNITY_EDITOR
        _hotUpdateAss = Assembly.Load(s_assetDatas["HotUpdate.dll"].bytes);
#else
        _hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
#endif
    }
}