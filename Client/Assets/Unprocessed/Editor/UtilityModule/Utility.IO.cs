using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Editor.UtilityModule
{
    public static partial class Utility
    {
        public static class IO
        {
            [Flags]
            public enum ECopyFolder
            {
                None = 0,
                /// 目标文件夹是否要覆盖
                OverrideTargetFolder = 1 << 0,
                /// 目标文件下文件是否要覆盖
                OverrideTargetFile = 1 << 2,
                /// 目标子文件夹是否要覆盖
                OverrideTargetSubFolder = 1 << 1,
                /// 目标子文件是否要覆盖
                OverrideTargetSubFile = 1 << 2,
                /// 覆盖所有
                OverrideAll = OverrideTargetFolder | OverrideTargetFile | OverrideTargetSubFolder | OverrideTargetSubFile,
            }

#region Copy
            /// 拷贝文件夹
            public static void CopyFolder(string sourceFolder, string targetFolder, ECopyFolder eCopyFolder)  
            {
                // 确保源目录存在  
                if (!Directory.Exists(sourceFolder))  
                {  
                    throw new DirectoryNotFoundException($"源文件夹不存在 => {sourceFolder}");  
                }
                // 文件夹
                {
                    if (Directory.Exists(targetFolder))
                    {
                        if (!eCopyFolder.HasFlag(ECopyFolder.OverrideTargetFolder))
                        {
                            Debug.LogError($"已存在目标文件夹 => {targetFolder}");
                            return;
                        }
                        Directory.Delete(targetFolder);
                        Directory.CreateDirectory(targetFolder);  
                    }
                    else
                    {
                        Directory.CreateDirectory(targetFolder);  
                    }
                }
                // 文件夹下文件
                {
                    FileInfo[] files = new DirectoryInfo(sourceFolder).GetFiles();  
                    foreach (FileInfo file in files)  
                    {  
                        string tempPath = Path.Combine(targetFolder, file.Name);
                        if (File.Exists(tempPath))
                        {
                            if (!eCopyFolder.HasFlag(ECopyFolder.OverrideTargetFile))
                            {
                                Debug.LogWarning($"已存在目标文件 => {tempPath}");
                                continue;
                            }
                        }
                        file.CopyTo(tempPath, true); 
                    }  
                }
                // 获取源目录中的子目录  
                DirectoryInfo[] dirs = new DirectoryInfo(sourceFolder).GetDirectories();  
                // 递归拷贝子目录  
                foreach (DirectoryInfo dir in dirs)  
                {  
                    string newDestinationSubDir = Path.Combine(targetFolder, dir.Name);  
                    CopySubFolder(dir.FullName, newDestinationSubDir, eCopyFolder);  
                }  
            }

            /// 拷贝子文件夹
            private static void CopySubFolder(string sourceFolder, string targetFolder, ECopyFolder eCopyFolder)
            {
                // 确保源目录存在  
                if (!Directory.Exists(sourceFolder))  
                {  
                    throw new DirectoryNotFoundException($"源文件夹不存在 => {sourceFolder}");  
                }
                // 文件夹
                {
                    if (Directory.Exists(targetFolder))
                    {
                        if (!eCopyFolder.HasFlag(ECopyFolder.OverrideTargetSubFolder))
                        {
                            Debug.LogWarning($"已存在目标文件夹 = > {targetFolder}");
                            return; 
                        }
                        Directory.Delete(targetFolder);
                        Directory.CreateDirectory(targetFolder); 
                    }
                    else
                    {
                        Directory.CreateDirectory(targetFolder);  
                    }
                }
                // 文件夹下文件
                {
                    FileInfo[] files = new DirectoryInfo(sourceFolder).GetFiles();  
                    foreach (FileInfo file in files)  
                    {  
                        string tempPath = Path.Combine(targetFolder, file.Name);
                        if (File.Exists(tempPath))
                        {
                            if (!eCopyFolder.HasFlag(ECopyFolder.OverrideTargetSubFile))
                            {
                                Debug.LogError($"已存在目标文件 => {tempPath}");
                                continue;
                            }
                        }
                        file.CopyTo(tempPath, true); 
                    }  
                }
                // 获取源目录中的子目录  
                DirectoryInfo[] dirs = new DirectoryInfo(sourceFolder).GetDirectories();  
                // 递归拷贝子目录  
                foreach (DirectoryInfo dir in dirs)  
                {  
                    string newDestinationSubDir = Path.Combine(targetFolder, dir.Name);  
                    CopySubFolder(dir.FullName, newDestinationSubDir, eCopyFolder);  
                }  
            }
#endregion

#region Scan
            /// <summary>
            /// 获取目录下所有文件（包括子目录）的 FileInfo 列表
            /// </summary>
            /// <param name="directoryPath">要扫描的目录路径</param>
            /// <param name="excludeMeta">是否排除 .meta 文件</param>
            /// <returns>文件信息列表</returns>
            public static List<FileInfo> GetAllFilesWithInfo(string directoryPath, bool excludeMeta = true)
            {
                List<FileInfo> fileList = new List<FileInfo>();
                
                try
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
                    
                    // 检查目录是否存在
                    if (!dirInfo.Exists)
                    {
                        Debug.LogWarning($"目录不存在: {directoryPath}");
                        return fileList;
                    }

                    // 获取当前目录的所有文件
                    FileInfo[] files = dirInfo.GetFiles();
                    foreach (FileInfo file in files)
                    {
                        if (excludeMeta && file.Extension.Equals(".meta")) 
                            continue;
                        
                        fileList.Add(file);
                    }

                    // 递归处理所有子目录
                    DirectoryInfo[] subDirs = dirInfo.GetDirectories();
                    foreach (DirectoryInfo subDir in subDirs)
                    {
                        fileList.AddRange(GetAllFilesWithInfo(subDir.FullName, excludeMeta));
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"扫描目录时出错: {directoryPath}\n{ex.Message}");
                }
                
                return fileList;
            }

            /// <summary>
            /// 带过滤条件的文件扫描
            /// </summary>
            public static List<FileInfo> GetFilesWithInfo(string directoryPath, string searchPattern = "*", bool recursive = true, bool excludeMeta = true)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
                
                if (!dirInfo.Exists)
                    return new List<FileInfo>();

                // 获取文件并转换为List
                IEnumerable<FileInfo> files = dirInfo.EnumerateFiles(searchPattern);
                
                if (excludeMeta)
                    files = files.Where(f => f.Extension != ".meta");
                
                List<FileInfo> result = files.ToList();
                
                // 递归处理子目录
                if (recursive)
                {
                    foreach (DirectoryInfo subDir in dirInfo.GetDirectories())
                    {
                        result.AddRange(GetFilesWithInfo(subDir.FullName, searchPattern, recursive, excludeMeta));
                    }
                }
                
                return result;
            }
#endregion
            
        }
    }
}
