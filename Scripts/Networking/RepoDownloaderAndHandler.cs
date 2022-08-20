namespace GitHubRegistryNetworking.Scripts.Networking
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using DataTypes;
    using GitHubAPI;
    using UnityEngine;
    using PackageInfo = DataTypes.PackageInfo;
    using RegistryInfo = Registries.RegistryInfo;


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
        }

        private void SaveToDisk(byte[] bytes, PackageInfo packageInfo, string format)
        {
            DeleteDirectory(Application.persistentDataPath + "/" + CustomPackagesFolder);
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
    }
}
