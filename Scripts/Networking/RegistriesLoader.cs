namespace GitHubRegistryNetworking.Scripts.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataTypes;
    using GitHubAPI;
    using Registries;
    using UnityEngine;

    public class RegistriesLoader
    {
        public void LoadAllRegistriesData(List<RegistryInfo> registryInfos, Action callback = null)
        {
            foreach (var registryInfo in registryInfos)
            {
                LoadRegistry(registryInfo);
            }
            
            callback?.Invoke();
        }
         
        private async void LoadRegistry(RegistryInfo registryInfo)
        {
            var loaded = false;
            
            GitHubRequests.GetAllPackagesForAuthor<List<PackageInfo>>(
                registryInfo.Token, 
                registryInfo.AuthorName,
                (result) =>
                {
                    registryInfo.Packages = result;
                    loaded = true;
                });
 
            while (!loaded)
            {
                await Task.Delay(50);
            }

            LoadReleasesForPackages(registryInfo);
        }

        private async void LoadReleasesForPackages(RegistryInfo registryInfo, Action callback = null)
        {
            var loaded = false;

            foreach (var package in registryInfo.Packages)
            {
                loaded = false;
                 
                GitHubRequests.GetAllReleasesForPackage<List<ReleaseInfo>>(
                    registryInfo.Token, 
                    registryInfo.AuthorName, 
                    package.name,
                    (releases) =>
                    {
                        package.releases = releases;
                        package.GetInstalledInfo();
                        loaded = true;
                    });
                
                while (!loaded)
                {
                    await Task.Delay(50);
                }
            }
        }
    }
}
