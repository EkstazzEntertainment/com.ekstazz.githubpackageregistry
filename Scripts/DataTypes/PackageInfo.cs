namespace GitHubRegistryNetworking.Scripts.DataTypes
{
    using System;
    using System.Collections.Generic;
    using Editor;

    [Serializable]
    public class PackageInfo
    {
        public long id;
        public string name;
        public string description;
        public bool installed;
        public string installedVersion = "-1, -1, -1";
        public List<ReleaseInfo> releases = new List<ReleaseInfo>();
        public bool folded = true;

        
        public string GetInstalledInfo()
        {
            var packageHandler = new PackageInfoHandler();
            installed = packageHandler.CheckIfPackageIsInstalled(name);
            if (installed)
            {
                installedVersion = packageHandler.GetInstalledVersion(name);
            }

            return installedVersion;
        }
    }

    [Serializable]
    public class PackageInfos
    {
        public List<PackageInfo> packages;
    }
}
