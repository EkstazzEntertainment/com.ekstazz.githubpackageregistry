namespace GitHubRegistryNetworking.Scripts.Networking.DataTypes
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class PackageInfo
    {
        public long id;
        public string name;
        public string description;
        public bool installed;
        public string installedVersion = "-1, -1, -1";
    }

    [Serializable]
    public class PackageInfos
    {
        public List<PackageInfo> packages;
    }
}
