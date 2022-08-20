namespace GitHubRegistryNetworking.Scripts.Networking
{
    using System;
    using DataTypes;
    using GitHubAPI;
    using PackageInfo = DataTypes.PackageInfo;
    using RegistryInfo = Registries.RegistryInfo;


    public class RepoDownloaderAndHandler
    {
        public void DownloadPackageVersion(RegistryInfo registryInfo, PackageInfo packageInfo, ReleaseInfo releaseInfo, Action callback = null)
        {
            GitHubRequests.DownloadPackageVersion(
                registryInfo.Token, 
                registryInfo.AuthorName, 
                packageInfo.name,
                releaseInfo.tag_name,
                (releases) =>
                {
                    
                });
        }

        private void DeCompressDownloadedZipPackage()
        {
            
        }
    }
}
