namespace GitHubRegistryNetworking.Scripts.Editor
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;

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


        [MenuItem ("Window/GitHub Package Registry Editor Window")]
        public static void  ShowWindow () {
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

        private void OnGUI()
        {
            LoadScopeRegistryDataBase();
            RegistryAddition();
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
            GUILayout.Label (label, EditorStyles.boldLabel);
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
                reader.Close();
            }
        }
    }
}
