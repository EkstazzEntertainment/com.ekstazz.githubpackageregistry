namespace GitHubRegistryNetworking.Scripts.Networking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading;
    using DataTypes;
    using GitHubAPI;
    using UnityEditor;
    using UnityEditor.PackageManager;
    using UnityEditor.VersionControl;
    using UnityEngine;
    using PackageInfo = DataTypes.PackageInfo;
    using RegistryInfo = Registries.RegistryInfo;
    using Task = System.Threading.Tasks.Task;


    public class RepoDownloaderAndHandler
    {
        private const string CustomPackagesFolder = "CustomPackages";
        
        
        public void DownloadPackageVersion(RegistryInfo registryInfo, PackageInfo packageInfo, ReleaseInfo releaseInfo, Action callback = null)
        {
            GitHubRequests.DownloadPackageVersion(
                            registryInfo.Token, 
                            registryInfo.AuthorName, 
                            packageInfo.name,
                            releaseInfo.tag_name,
                            (bytes) =>
                            {
                                HandleDownloadedPackage(bytes, packageInfo, ".zip");
                            });
        }

        private void HandleDownloadedPackage(byte[] bytes, PackageInfo packageInfo, string format)
        {
            SaveToDisk(bytes, packageInfo, format);
            DeCompressDownloadedZipPackage(packageInfo.name, format);
            ExportExtractedPackageToUnityPackage(packageInfo.name, () =>
            {
                ImportUnityPackage(packageInfo.name);
            });
        }

        private void SaveToDisk(byte[] bytes, PackageInfo packageInfo, string format)
        {
            DeleteDirectory(Application.persistentDataPath + "/" + CustomPackagesFolder);
            DeleteDirectory("Assets/" + packageInfo.name);
            DeleteFile("Assets/" + packageInfo.name + ".meta");
            
            CreatePackagesFolder();
            File.WriteAllBytes(BuildPackageSavePath(packageInfo.name) + format, bytes);
        }

        private void DeCompressDownloadedZipPackage(string packageName, string format)
        {
            ZipFile.ExtractToDirectory(
                BuildPackageSavePath(packageName) + format, 
                BuildPackageSavePath(packageName));
            RenameExtractedFile(packageName);
        }
 
        private string BuildPackageSavePath(string packageName)
        {
            return Application.persistentDataPath + "/" + CustomPackagesFolder + "/" + packageName;
        }

        private void RenameExtractedFile(string packageName)
        {
            var files = Directory.EnumerateDirectories(BuildPackageSavePath(packageName)).ToList();
            Directory.Move(files.First(), BuildPackageSavePath(packageName) + "/" + packageName);
        }
  
        private async void ExportExtractedPackageToUnityPackage(string packageName, Action callback)
        {  
            var link = BuildPackageSavePath(packageName) + "/" + packageName;
            MovePackageTemporarilyToAssets(packageName);
            AssetDatabase.Refresh();
            while (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                await Task.Delay(100);
            }
            
            AssetDatabase.ExportPackage(
                $"Assets/{packageName}", 
                BuildPackageSavePath(packageName) + $"/{packageName}.unitypackage", 
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);

            DeleteDirectory("Assets/" + packageName);
            DeleteFile("Assets/" + packageName + ".meta");
            
            callback.Invoke();
        }

        private void ImportUnityPackage(string packageName)
        {
            AssetDatabase.ImportPackage(BuildPackageSavePath(packageName) + $"/{packageName}.unitypackage", false);
            DeleteDirectory(Application.persistentDataPath + "/" + CustomPackagesFolder);
        }
  
        private void MovePackageTemporarilyToAssets(string packageName)
        {
            var link = BuildPackageSavePath(packageName) + "/" + packageName;
            Directory.Move(link, "Assets/" + packageName);
        }

        private void CreatePackagesFolder()
        {
            var path = Application.persistentDataPath + "/" + CustomPackagesFolder;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void DeleteDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }

        private void DeleteFile(string directory)
        {
            if (File.Exists(directory))
            {
                File.Delete(directory);
            }
        }
    }
}
