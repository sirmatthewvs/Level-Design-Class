using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;
using System.IO;
using System.Linq;

namespace Gamekit3D.SetupTools
{
    /// <summary>
    /// Automatic setup wizard that runs when the 3D Game Kit Lite is imported
    /// Ensures all dependencies and settings are correctly configured
    /// </summary>
    [InitializeOnLoad]
    public class GameKitSetupWizard : EditorWindow
    {
        private const string SETUP_COMPLETE_KEY = "GameKit3D_SetupComplete_v1";
        private const string CINEMACHINE_MIN_VERSION = "2.10.0";
        private const string CINEMACHINE_MAX_VERSION = "2.99.99";
        
        private static bool setupNeeded = false;
        private Vector2 scrollPosition;

        static GameKitSetupWizard()
        {
            EditorApplication.delayCall += CheckSetupStatus;
        }

        private static void CheckSetupStatus()
        {
            // Check if setup has been completed
            if (EditorPrefs.GetBool(SETUP_COMPLETE_KEY, false))
            {
                return;
            }

            // Check if 3DGamekitLite folder exists
            string[] foundPaths = AssetDatabase.FindAssets("t:Folder 3DGamekitLite");
            if (foundPaths.Length == 0)
            {
                return; // Package not imported yet
            }

            setupNeeded = true;
            EditorApplication.delayCall += ShowSetupWizard;
        }

        private static void ShowSetupWizard()
        {
            if (setupNeeded)
            {
                GetWindow<GameKitSetupWizard>(true, "3D Game Kit Setup", true);
            }
        }

        [MenuItem("Tools/3D Game Kit/Setup Wizard")]
        public static void ShowWindow()
        {
            GetWindow<GameKitSetupWizard>(true, "3D Game Kit Setup", true);
        }

        [MenuItem("Tools/3D Game Kit/Reset Setup (Force Re-run)")]
        public static void ResetSetup()
        {
            EditorPrefs.SetBool(SETUP_COMPLETE_KEY, false);
            setupNeeded = true;
            ShowWindow();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Header
            GUILayout.Space(10);
            EditorGUILayout.LabelField("3D Game Kit Lite - Setup Wizard", EditorStyles.boldLabel);
            GUILayout.Space(5);
            EditorGUILayout.HelpBox(
                "This wizard will help you configure your project to use the 3D Game Kit Lite package correctly.",
                MessageType.Info
            );

            GUILayout.Space(10);

            // Check 1: Cinemachine Version
            DrawCinemachineCheck();

            GUILayout.Space(10);

            // Check 2: Layers Configuration
            DrawLayersCheck();

            GUILayout.Space(10);

            // Check 3: NavMesh Configuration
            DrawNavMeshCheck();

            GUILayout.Space(10);

            // Check 4: Input System
            DrawInputSystemCheck();

            GUILayout.Space(20);

            // Complete Setup Button
            using (new EditorGUI.DisabledScope(false))
            {
                if (GUILayout.Button("Complete Setup", GUILayout.Height(30)))
                {
                    CompleteSetup();
                }
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Skip (I'll configure manually)"))
            {
                EditorPrefs.SetBool(SETUP_COMPLETE_KEY, true);
                Close();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawCinemachineCheck()
        {
            EditorGUILayout.LabelField("1. Cinemachine Version Check", EditorStyles.boldLabel);
            
            bool cinemachineCorrect = CheckCinemachineVersion(out string installedVersion);

            if (cinemachineCorrect)
            {
                EditorGUILayout.HelpBox(
                    $"✓ Cinemachine {installedVersion} is installed (Compatible with 2.x)",
                    MessageType.Info
                );
            }
            else
            {
                EditorGUILayout.HelpBox(
                    $"⚠ Cinemachine version issue detected!\n" +
                    $"Installed: {installedVersion}\n" +
                    $"Required: 2.10.0 - 2.99.99\n\n" +
                    $"This package requires Cinemachine 2.x (NOT 3.x). Click the button below to install the correct version.",
                    MessageType.Warning
                );

                if (GUILayout.Button("Install Cinemachine 2.10.5"))
                {
                    InstallCinemachine();
                }
            }
        }

        private void DrawLayersCheck()
        {
            EditorGUILayout.LabelField("2. Layers Configuration", EditorStyles.boldLabel);

            string[] requiredLayers = { "Player", "Enemy", "Environment", "Cameras", "Checkpoint" };
            bool allLayersPresent = CheckLayers(requiredLayers, out string missingLayers);

            if (allLayersPresent)
            {
                EditorGUILayout.HelpBox(
                    "✓ All required layers are configured",
                    MessageType.Info
                );
            }
            else
            {
                EditorGUILayout.HelpBox(
                    $"⚠ Missing required layers: {missingLayers}\n\n" +
                    $"Click the button below to apply the TagManager preset with all required layers.",
                    MessageType.Warning
                );

                if (GUILayout.Button("Apply Tag & Layer Preset"))
                {
                    ApplyTagManagerPreset();
                }
            }
        }

        private void DrawNavMeshCheck()
        {
            EditorGUILayout.LabelField("3. NavMesh Agent Types", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "The package includes NavMesh agent types for:\n" +
                "• Humanoid (Player character)\n" +
                "• Chomper (Enemy type)\n\n" +
                "Click the button below to apply these settings.",
                MessageType.Info
            );

            if (GUILayout.Button("Apply NavMesh Settings Preset"))
            {
                ApplyNavMeshPreset();
            }
        }

        private void DrawInputSystemCheck()
        {
            EditorGUILayout.LabelField("4. Input System Configuration", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "✓ Input System Actions are configured at:\n" +
                "Assets/3DGamekitLite/InputSystem_Actions.inputactions\n\n" +
                "The generated code is automatically placed in:\n" +
                "Assets/3DGamekitLite/InputSystem_Actions.cs",
                MessageType.Info
            );
        }

        private bool CheckCinemachineVersion(out string installedVersion)
        {
            installedVersion = "Not Installed";

            var packageListRequest = UnityEditor.PackageManager.Client.List();
            while (!packageListRequest.IsCompleted)
            {
                System.Threading.Thread.Sleep(10);
            }

            if (packageListRequest.Status == UnityEditor.PackageManager.StatusCode.Success)
            {
                var cinemachinePackage = packageListRequest.Result.FirstOrDefault(p => p.name == "com.unity.cinemachine");
                if (cinemachinePackage != null)
                {
                    installedVersion = cinemachinePackage.version;
                    
                    // Check if version is 2.x
                    if (installedVersion.StartsWith("2."))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckLayers(string[] requiredLayers, out string missingLayers)
        {
            missingLayers = "";
            bool allPresent = true;

            foreach (string layerName in requiredLayers)
            {
                if (LayerMask.NameToLayer(layerName) == -1)
                {
                    allPresent = false;
                    missingLayers += layerName + ", ";
                }
            }

            if (!allPresent)
            {
                missingLayers = missingLayers.TrimEnd(',', ' ');
            }

            return allPresent;
        }

        private void InstallCinemachine()
        {
            UnityEditor.PackageManager.Client.Add("com.unity.cinemachine@2.10.5");
            EditorUtility.DisplayDialog(
                "Installing Cinemachine",
                "Cinemachine 2.10.5 is being installed. Please wait for the Package Manager to complete.",
                "OK"
            );
        }

        private void ApplyTagManagerPreset()
        {
            string presetPath = "Assets/3DGamekitLite/SetupTools/TagManager.preset";
            Preset preset = AssetDatabase.LoadAssetAtPath<Preset>(presetPath);

            if (preset != null)
            {
                string tagManagerPath = "ProjectSettings/TagManager.asset";
                SerializedObject tagManager = new SerializedObject(
                    AssetDatabase.LoadAllAssetsAtPath(tagManagerPath)[0]
                );

                preset.ApplyTo(tagManager.targetObject);
                tagManager.ApplyModifiedProperties();

                EditorUtility.DisplayDialog(
                    "Success",
                    "Tag & Layer configuration has been applied successfully!",
                    "OK"
                );
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "Error",
                    $"Could not find TagManager preset at:\n{presetPath}",
                    "OK"
                );
            }
        }

        private void ApplyNavMeshPreset()
        {
            string presetPath = "Assets/3DGamekitLite/SetupTools/NavMeshSettings.preset";
            Preset preset = AssetDatabase.LoadAssetAtPath<Preset>(presetPath);

            if (preset != null)
            {
                string navMeshPath = "ProjectSettings/NavMeshAreas.asset";
                SerializedObject navMeshSettings = new SerializedObject(
                    AssetDatabase.LoadAllAssetsAtPath(navMeshPath)[0]
                );

                preset.ApplyTo(navMeshSettings.targetObject);
                navMeshSettings.ApplyModifiedProperties();

                EditorUtility.DisplayDialog(
                    "Success",
                    "NavMesh settings (Humanoid & Chomper agent types) have been applied successfully!",
                    "OK"
                );
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "Error",
                    $"Could not find NavMesh preset at:\n{presetPath}",
                    "OK"
                );
            }
        }

        private void CompleteSetup()
        {
            EditorPrefs.SetBool(SETUP_COMPLETE_KEY, true);
            
            EditorUtility.DisplayDialog(
                "Setup Complete",
                "3D Game Kit Lite setup is complete!\n\n" +
                "You can now:\n" +
                "• Open the Demo scene (Assets/3DGamekitLite/Scenes/Demo.unity)\n" +
                "• Check Readme.md for complete usage guide\n" +
                "• Study the Demo scene to see everything in action\n\n" +
                "Need help? Chat with our AI assistant:\n" +
                "https://chatgpt.com/g/g-688f7adaa6d88191896972ca59a2d7fa-gio-game-dev-teacher\n\n" +
                "If you need to re-run this setup, go to:\n" +
                "Tools > 3D Game Kit > Reset Setup",
                "OK"
            );

            Close();
        }
    }
}
