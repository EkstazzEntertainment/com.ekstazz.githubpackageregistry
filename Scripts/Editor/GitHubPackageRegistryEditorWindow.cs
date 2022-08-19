namespace GitHubRegistryNetworking.Scripts.Editor
{
    using System.Collections.Generic;
    using System.IO;
    using Networking;
    using Registries;
    using UnityEditor;
    using UnityEngine;
    using PackageInfo = DataTypes.PackageInfo;


    public class GitHubPackageRegistryEditorWindow : EditorWindow
    {
        private const string WindowTitle = "GitHub Package Registry";
        private const string ParentFolder = "Assets";
        private const string FolderName = "CustomGitHubRegistries";
            
        private string ScopeRegistryDataBasePath = ParentFolder + "/" + FolderName + "/";

        private static EditorWindow window;

        private string httpsRegistryLink;
        private string gitHubAccountOwner;
        private string accessToken;
        
        private bool registriesHaveLoaded = false;
        private List<RegistryInfo> registryInfos = new List<RegistryInfo>();

        private RegistriesLoader registriesLoader = new RegistriesLoader();
        

        [MenuItem ("Window/GitHub Package Registry Editor Window")]
        public static void ShowWindow () {
            window = ConfigureWindow();
        }

        private static EditorWindow ConfigureWindow()
        {
            var windowInstance = GetWindow(typeof(GitHubPackageRegistryEditorWindow), true, WindowTitle, true);

            windowInstance.name = WindowTitle;
            
            windowInstance.minSize = new Vector2(500, 600);
            windowInstance.maxSize = new Vector2(500, 600);

            return windowInstance;
        }

        private void OnEnable()
        {
            LoadData();
        }
 
        private void LoadData()
        {
            registriesHaveLoaded = false;
            LoadScopeRegistryDataBase();
            registriesLoader.LoadAllRegistriesData(registryInfos, () =>
            {
                registriesHaveLoaded = true;
            });
        }

        private void OnGUI()
        {
            RegistryAddition();
            if (registriesHaveLoaded)
            {
                DrawAllRegistries();
            }
        }

        private void RegistryAddition()
        {
            LabelTextPair("The HTTP(S) link to the GitHub scoped registry", ref httpsRegistryLink);
            LabelTextPair("Owner of the GitHub account", ref gitHubAccountOwner);
            LabelTextPair("Your personal access token generated for you by the account owner", ref accessToken);
            
            if (GUILayout.Button("Add a new GitHub registry"))
            {
                HandleRegistryAddition();
            }
        }
        
        private void LabelTextPair(string label, ref string variable)
        {
            GUILayout.Label(label, EditorStyles.boldLabel);
            variable = EditorGUILayout.TextField ("", variable);
        }

        private void HandleRegistryAddition()
        {
            if (string.IsNullOrWhiteSpace(httpsRegistryLink) 
                || string.IsNullOrWhiteSpace(ScopeRegistryDataBasePath) 
                || string.IsNullOrWhiteSpace(accessToken))
            {
                return;
            }
            
            var fullPath = ScopeRegistryDataBasePath + gitHubAccountOwner + ".txt";
            if (File.Exists(fullPath))
            {
                Debug.Log("A Scope Registry DataBase exists. No need to make a new one.");
            }
            else
            {
                if(!Directory.Exists(ScopeRegistryDataBasePath))
                {
                    Debug.Log("The Registry Folder does not exist. Creating it!");
                    CreateTheRegistryFolder();
                }
                
                Debug.Log("No Scope Registry DataBase has been found. Creating a new one.");
                CreateAScopeRegistryDataBase();
            }
            
            AssetDatabase.Refresh();
        }

        private void CreateTheRegistryFolder()
        {
            string guid = AssetDatabase.CreateFolder(ParentFolder, FolderName);
            string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log(newFolderPath);
        }

        private void CreateAScopeRegistryDataBase()
        {
            var fullPath = ScopeRegistryDataBasePath + gitHubAccountOwner + ".txt";
            StreamWriter writer = new StreamWriter(fullPath, true);
            writer.WriteLine(httpsRegistryLink);
            writer.WriteLine(gitHubAccountOwner);
            writer.WriteLine(accessToken);
            writer.Close();

            AssetDatabase.ImportAsset(ScopeRegistryDataBasePath); 
        }
 
        private void LoadScopeRegistryDataBase()
        {
            if(!Directory.Exists(ScopeRegistryDataBasePath))
            {
                return;
            }
             
            string[] filePaths = Directory.GetFiles(ScopeRegistryDataBasePath, "*.txt", SearchOption.TopDirectoryOnly);
            foreach (var path in filePaths)
            {
                StreamReader reader = new StreamReader(path);
                var httpLink = reader.ReadLine();
                var ownerName = reader.ReadLine();
                var token = reader.ReadLine();
                var registryInfo = new RegistryInfo { RepositoryLink = httpLink, AuthorName = ownerName, Token = token};
                registryInfos.Add(registryInfo);
                reader.Close();
            }
        }


        private float verticalPosition = 0;
        private void DrawAllRegistries()
        {
            GUILayout.Space(30);
            GUILayout.BeginVertical();
            var scrollView = GUILayout.BeginScrollView(new Vector2(0, verticalPosition));
            verticalPosition = scrollView.y;
            
            foreach (var registryInfo in registryInfos)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                DrawRegistry(registryInfo);
                GUILayout.EndVertical();
                GUILayout.Space(30);
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawRegistry(RegistryInfo registryInfo)
        {
            var headerStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 18 };
            EditorGUILayout.LabelField(registryInfo.AuthorName, headerStyle, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Remove (also packages)", GUILayout.Width(150), GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Are you sure you wanna delete this registry and all its packages?", "Think again! You will have to reinstall everything later.", "delete", "CANCEL!!!"))
                {
                    //remove registries and its packages //todo 
                }
            }
            
            DrawPackages(registryInfo);
        }

        private void DrawPackages(RegistryInfo registryInfo)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            foreach (var package in registryInfo.Packages)
            {
                DrawPackage(package);
                DrawPackageVersions(package);
                GUILayout.Space(5);
            }
 
            GUILayout.EndVertical();
        }

        private void DrawPackage(PackageInfo package)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            if (GUILayout.Button(package.description, GUILayout.Height(30), GUILayout.Width(300)))
            {
                package.folded = !package.folded;
            }

            GUILayout.BeginVertical();
            GUILayout.Space(5);
            DrawPackageVersionInfo(package);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void DrawPackageVersionInfo(PackageInfo package)
        {
            var versionTextStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 15 };
            string installedInfo;
            if (!package.installed)
            {
                installedInfo = "NOT INSTALLED";
                versionTextStyle.normal.textColor = Color.red;
            }
            else
            {
                installedInfo = package.installedVersion;
                versionTextStyle.normal.textColor = Color.green;
            }
            EditorGUILayout.LabelField(installedInfo, versionTextStyle, GUILayout.ExpandWidth(true));
        }
 
        private void DrawPackageVersions(PackageInfo package)
        {
            if (!package.folded)
            {
                var color = GUI.backgroundColor;
                GUI.backgroundColor = Color.cyan;

                foreach (var release in package.releases)
                {
                    if (GUILayout.Button(release.tag_name, GUILayout.Width(300)))
                    {
                     
                    }
                }
                 
                GUI.backgroundColor = color;
            }
        }
    }
}
