using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.StateMachineModule
{
    public class StateMachineEditorWindow : EditorWindow
    {
        [MenuItem("Tools/State Machine Editor")]
        public static void ShowWindow()
        {
            GetWindow<StateMachineEditorWindow>(nameof(StateMachineEditorWindow));
        }

        private IStateMachine[] stateMachines;
        private string[] displayStateMachines;
        private int inRefIndex;
        private IStateMachine selectedStateMachine;
        
        private string folderPath = string.Empty;
        private List<FileInfo> stateMachinesDataFileInfos;
        
        private Vector2 scrollPos;
        private Rect canvasSize = new Rect(0, 0, 5000, 5000); // 虚拟画布大小
        
        private void OnEnable()
        {
            stateMachines = UtilityModule.Utility.Type.GetImplementingClasses<IStateMachine>(typeof(StateMachineEditorWindow).Assembly).ToArray();
            displayStateMachines = stateMachines.Select(x => x.GetType().Name).ToArray();
            Scan();
        }

        private void OnGUI()
        {
            // 绘制滚动视图（支持拖拽画布）
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                // 绘制网格背景
                DrawGrid(20, 0.2f, Color.gray);
                DrawGrid(100, 0.4f, Color.gray);
                
                using (new EditorGUILayout.HorizontalScope())
                {
                    int changeIndex = EditorGUILayout.Popup(inRefIndex, displayStateMachines);
                    if (changeIndex != inRefIndex)
                    {
                        inRefIndex = changeIndex;
                        Scan();
                    }
                    DrawSelectedFolder();
                }
                selectedStateMachine?.OnGUI();
            }
            EditorGUILayout.EndScrollView();
        }
        
        /// 绘制网格背景
        private void DrawGrid(float spacing, float opacity, Color color)
        {
            int widthDivs = Mathf.CeilToInt(canvasSize.width / spacing);
            int heightDivs = Mathf.CeilToInt(canvasSize.height / spacing);

            Handles.BeginGUI();
            Handles.color = new Color(color.r, color.g, color.b, opacity);

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(i * spacing, 0), new Vector3(i * spacing, canvasSize.height));
            }

            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(0, j * spacing), new Vector3(canvasSize.width, j * spacing));
            }

            Handles.EndGUI();
        }
        
        private void DrawSelectedFolder()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    string folderPath = SetFolderPath(this.folderPath, "指定资源文件夹");
                    if (!string.IsNullOrEmpty(folderPath))
                    {
                        if (!folderPath.Contains(Application.dataPath))
                        {
                            Debug.LogError("暂不支持加载项目外的资源");
                            return;
                        }
                        this.folderPath = folderPath;
                    }
                    if (GUILayout.Button("Rescan"))
                    {
                        Scan();
                    }
                }
            }
        }
        private string SetFolderPath(string showPath, string defaultName)
        {
            string tempPath = string.Empty;
            if (string.IsNullOrEmpty(showPath))
            {
                showPath = defaultName;
            }
            GUI.enabled = false;
            EditorGUILayout.TextField("", showPath);
            GUI.enabled = true;
            if (GUILayout.Button("指定资源文件夹"))
            {
                tempPath = EditorUtility.OpenFolderPanel("选取资源文件夹", "", string.Empty);
                return tempPath;
            }
            return tempPath;
        }
        private void Scan()
        {
            if (string.IsNullOrEmpty(folderPath)) return;
            if (!folderPath.Contains(Application.dataPath))
            {
                Debug.LogError("暂不支持加载项目外的资源");
                return;
            }
            List<FileInfo> fileInfos = UtilityModule.Utility.IO.GetAllFilesWithInfo(this.folderPath);
            selectedStateMachine = stateMachines[inRefIndex];
            Type type = selectedStateMachine.BindDataType();
            stateMachinesDataFileInfos ??= new List<FileInfo>();
            stateMachinesDataFileInfos.Clear();
            {
                for (int i = 0; i < fileInfos.Count; i++)
                {
                    FileInfo fileInfo = fileInfos[i];
                    string path = fileInfo.FullName[fileInfo.FullName.IndexOf("Assets", StringComparison.Ordinal)..];
                    Object obj = AssetDatabase.LoadAssetAtPath(path, type);
                    if (obj is not IStateMachineData stateMachineData)
                    {
                        continue;
                    }
                    stateMachinesDataFileInfos.Add(fileInfo);
                }
            }
            selectedStateMachine.Init(folderPath, stateMachinesDataFileInfos, Repaint);
        }
    }
    
    public partial class StateMachineEditorWindow<TStateMachineData, TStateNode, TTransition>: IStateMachine
        where TStateMachineData: ScriptableObject, IStateMachineData<TStateNode, TTransition> 
        where TStateNode: class, IStateNode, new()
        where TTransition: class, ITransition, new()
    {
        
        // Styles
        class Styles 
        {
            public GUIStyle header = "frameBox";
            public GUIStyle detail = "window";
        }
        
        // Constants
        
        // Static variables
        private static Styles s_Styles;

        // Member variables
        private FileInfo[] stateMachineDataFileInfos;
        private string[] displayName;
        private int inRefIndex;
        private TStateMachineData selectedStateMachineData;
        private Action repaintCallback;

        private EEventStatus eEventStatus;
        
        void IStateMachine.Init(string folderRootPath, List<FileInfo> stateMachineDataFileInfos, Action repaintCallback)
        {
            stateMachineDataFileInfos ??= new List<FileInfo>();
            this.stateMachineDataFileInfos = stateMachineDataFileInfos.ToArray();
            this.displayName = this.stateMachineDataFileInfos.Select(x => x.FullName.Remove(0, folderRootPath.Length + 1)).ToArray();
            
            this.repaintCallback = repaintCallback;
            // 初始化选中
            selectedStateMachineData = null;
            inRefIndex = 0;
            if (this.stateMachineDataFileInfos.Length > inRefIndex)
            {
                FileInfo fileInfo = this.stateMachineDataFileInfos[inRefIndex];
                string path = fileInfo.FullName[fileInfo.FullName.IndexOf("Assets", StringComparison.Ordinal)..];
                selectedStateMachineData = AssetDatabase.LoadAssetAtPath<TStateMachineData>(path);
            }
        }
        void IStateMachine.Repaint()
        {
            repaintCallback.Invoke();
        }
        void IStateMachine.OnGUI()
        {
            if (s_Styles == null)
                s_Styles = new Styles();
            
            using (new EditorGUILayout.VerticalScope())
            {
                // 1.顶部工具栏
                using (new EditorGUILayout.VerticalScope(s_Styles.header))
                {
                    DrawToolbar();
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    // 2.检视面板
                    if (selectedElement != null)
                    {
                        using (new EditorGUILayout.VerticalScope(s_Styles.detail, GUILayout.MaxWidth(250)))
                        {
                            DrawInspector();
                        }
                    }
                    // 3、绘制元素
                    using (new EditorGUILayout.VerticalScope())
                    {
                        // 4. 绘制所有过渡曲线
                        DrawTransitions();
                        // 5. 绘制所有状态节点
                        DrawStateNodes();
                    }
                }
            }

            if (Event.current.type == EventType.MouseDown)
            {
                // 当前选中的元素
                IElement selectedElement;

                // 优先检测过渡线
                TryGetTransitionAtPosition(Event.current.mousePosition, out selectedElement);
                // 其次检测 State
                if (selectedElement == null) 
                    TryGetStateNodeAtPosition(Event.current.mousePosition, out selectedElement);
                
                // 事件分支
                switch(eEventStatus)
                {
                    case EEventStatus.Normal: // 普通操作
                    {
                        if (selectedElement == null)
                        {
                            if (Event.current.button == 1) // 菜单
                            {
                                DrawNullClickMenu();
                                Event.current.Use();
                            }
                        }
                        else
                        {
                            switch(selectedElement)
                            {
                                case TStateNode stateNode: 
                                {
                                    if (Event.current.button == 0) // 选中编辑
                                        this.selectedElement = selectedElement;
                                    else if (Event.current.button == 1) // 菜单
                                    {
                                        DrawSelectedStateMenu(stateNode);
                                        Event.current.Use();
                                    }
                                    break;
                                }
                                case TTransition transition:
                                {
                                    if (Event.current.button == 0) // 选中编辑
                                        this.selectedElement = selectedElement;
                                    else if (Event.current.button == 1) // 菜单
                                    {
                                        DrawSelectedTransitionMenu(transition);
                                        Event.current.Use();
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                    }
                    case EEventStatus.AddTransition: // 添加过渡线操作
                    {
                        if (Event.current.button == 0 && selectedElement is TStateNode stateNode)
                        {
                            EndAddTransition(stateNode);
                        }
                        else
                        {
                            dragStartNode = null;
                            eEventStatus = EEventStatus.Normal;
                        }
                        break;
                    }
                }
            }
        }
        Type IStateMachine.BindDataType()
        {
            return typeof(TStateMachineData);
        }

        private enum EEventStatus 
        {
            Normal,
            AddTransition,
        }
    }
}