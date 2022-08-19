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
        public List<string> releases = new List<string>();
        public bool folded = true;
    }

    [Serializable]
    public class PackageInfos
    {
        public List<PackageInfo> packages;
    }
}
