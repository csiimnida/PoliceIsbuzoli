using System.IO;
using Plugins.ScriptFinder.RunTime.DevLogs;
using UnityEditor;
using UnityEngine;

namespace Plugins.ScriptFinder.Editor.Temp
{
    public class TempFolderToggler : EditorWindow
    {
        const string TargetFolder = "Assets/Plugins/ScriptFinder/Temp";
        const string TempCreateRoot = "Assets/Plugins/ScriptFinder";
        const string PackagePath = "Assets/Plugins/ScriptFinder/Editor/Temp/TempTemplate.unitypackage";

        [MenuItem("ScriptFinder/Temp Folder Toggler")]
        private static void OpenWindow() 
        {
            GetWindow<TempFolderToggler>("Temp Folder");
        }

        private void OnGUI()
        {
            GUILayout.Label("Temp folder manager", EditorStyles.boldLabel);
            GUILayout.Space(6);

            if (GUILayout.Button("Create Temp from package (overwrite if exists)"))
            {
                CreateFromPackageOverwrite();
            }

            if (GUILayout.Button("Delete Temp (if exists)"))
            {
                DeleteTempFolder();
            }

            #region BackupRegion
            // if (GUILayout.Button("Backup Temp to package (overwrite)"))
            // {
            //     BackupTempToPackage();
            // }
            #endregion
        }

        private static void CreateFromPackageOverwrite()
        {
            string placeholderPath = null;

            try
            {
                if (!File.Exists(PackagePath))
                {
                    DevLog.LogError("Package not found: " + PackagePath);
                    return;
                }

                if (AssetDatabase.IsValidFolder(TargetFolder))
                {
                    FileUtil.DeleteFileOrDirectory(TargetFolder);
                    FileUtil.DeleteFileOrDirectory(TargetFolder + ".meta");
                    AssetDatabase.Refresh();
                    DevLog.Log("Existing target deleted: " + TargetFolder);
                }
                else
                {
                    DevLog.Log("Target folder not found. Creating empty folder: " + TargetFolder);
                    EnsureFolderExists(TargetFolder);

                    placeholderPath = Path.Combine(TargetFolder, "README.txt");
                    if (!File.Exists(placeholderPath))
                    {
                        File.WriteAllText(placeholderPath, "Auto-generated placeholder for import.");
                        AssetDatabase.ImportAsset(placeholderPath);
                    }
                }

                EnsureFolderExists(TempCreateRoot);

                AssetDatabase.ImportPackage(PackagePath, false);
                AssetDatabase.Refresh();

                if (AssetDatabase.IsValidFolder(TargetFolder))
                {
                    DevLog.Log("Created target from package: " + TargetFolder);
                }
                else
                {
                    DevLog.LogError("Import completed but target folder not found: " + TargetFolder);
                }
            }
            catch (System.Exception e)
            {
                DevLog.LogError("Failed to create from package: " + e);
            }
            finally
            {
                if (!string.IsNullOrEmpty(placeholderPath) && File.Exists(placeholderPath))
                {
                    FileUtil.DeleteFileOrDirectory(placeholderPath);
                    FileUtil.DeleteFileOrDirectory(placeholderPath + ".meta");
                    AssetDatabase.Refresh();
                    DevLog.Log("Placeholder removed: " + placeholderPath);
                }
            }
        }



        private static void DeleteTempFolder()
        {
            try
            {
                if (AssetDatabase.IsValidFolder(TargetFolder))
                {
                    FileUtil.DeleteFileOrDirectory(TargetFolder);
                    FileUtil.DeleteFileOrDirectory(TargetFolder + ".meta");
                    AssetDatabase.Refresh();
                    DevLog.Log("Deleted: " + TargetFolder);
                }
                else
                {
                    DevLog.Log("Delete skipped. Target does not exist: " + TargetFolder);
                }
            }
            catch (System.Exception e)
            {
                DevLog.LogError("Failed to delete target: " + e);
            }
        }

        private static void BackupTempToPackage()
        {
            string placeholderPath = null;

            try
            {
                if (!AssetDatabase.IsValidFolder(TargetFolder))
                {
                    DevLog.Log("Target folder not found. Creating empty target: " + TargetFolder);
                    EnsureFolderExists(TargetFolder);

                    placeholderPath = Path.Combine(TargetFolder, "README.txt");
                    if (!File.Exists(placeholderPath))
                    {
                        File.WriteAllText(placeholderPath, "Auto-generated placeholder for backup.");
                        AssetDatabase.ImportAsset(placeholderPath);
                    }
                }

                string packageDir = Path.GetDirectoryName(PackagePath);
                if (!string.IsNullOrEmpty(packageDir))
                {
                    EnsureFolderExists(packageDir);
                }

                AssetDatabase.ExportPackage(new[] { TargetFolder }, PackagePath, ExportPackageOptions.Recurse);
                AssetDatabase.Refresh();

                if (File.Exists(PackagePath))
                    DevLog.Log("Backup package created at: " + PackagePath);
                else
                    DevLog.LogError("Failed to create backup package: " + PackagePath);
            }
            catch (System.Exception e)
            {
                DevLog.LogError("Failed to backup to package: " + e);
            }
            finally
            {
                if (!string.IsNullOrEmpty(placeholderPath) && File.Exists(placeholderPath))
                {
                    FileUtil.DeleteFileOrDirectory(placeholderPath);
                    FileUtil.DeleteFileOrDirectory(placeholderPath + ".meta");
                    AssetDatabase.Refresh();
                    DevLog.Log("Placeholder file removed: " + placeholderPath);
                }
            }
        }


        private static void EnsureFolderExists(string folderPath)
        {
            folderPath = folderPath.Replace("\\", "/").TrimEnd('/');
            if (string.IsNullOrEmpty(folderPath)) return;
            if (AssetDatabase.IsValidFolder(folderPath)) return;
            string[] parts = folderPath.Split('/');
            string cur = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = cur + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(cur, parts[i]);
                }
                cur = next;
            }
        }
    }
}