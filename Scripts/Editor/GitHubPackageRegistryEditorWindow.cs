namespace GitHubRegistryNetworking.Scripts.Editor
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public class GitHubPackageRegistryEditorWindow : EditorWindow
    {
        private const string WindowTitle = "GitHub Package Registry";
        private const string ScopeRegistryDataBasePath = "Assets/CustomGitHubRegistry.txt";
            
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
            RegistryAddition();
        }

        private void RegistryAddition()
        {
            LabelTextPair("The HTTP(S) link to the GitHub scoped registry", ref httpsRegistryLink);
            LabelTextPair("Owner of the GitHub account", ref gitHubAccountOwner);
            LabelTextPair("Your personal access token generated for you by the account owner", ref accessToken);
            
            if (GUILayout.Button("Add a new GitHub registry"))
            {
                LoadScopeRegistryDataBase();
            }
        }

        private void LabelTextPair(string label, ref string variable)
        {
            GUILayout.Label (label, EditorStyles.boldLabel);
            variable = EditorGUILayout.TextField ("", variable);
        }

        private void CreateAScopeRegistryDataBase()
        {
            StreamWriter writer = new StreamWriter(ScopeRegistryDataBasePath, true);
            writer.WriteLine("");
            writer.Close();

            AssetDatabase.ImportAsset(ScopeRegistryDataBasePath); 
        }

        private void LoadScopeRegistryDataBase()
        {
            StreamReader reader = new StreamReader(ScopeRegistryDataBasePath); 
            Debug.Log(reader.ReadToEnd());
            reader.Close();
        }
    }
}
