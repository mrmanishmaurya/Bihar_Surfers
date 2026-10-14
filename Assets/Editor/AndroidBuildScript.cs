using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BiharSurfers.Editor
{
    public static class AndroidBuildScript
    {
        private static readonly string[] Scenes = new string[]
        {
            "Assets/Scenes/Boot.unity",
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/Game.unity",
            "Assets/Scenes/GameOver.unity"
        };

        [MenuItem("Bihar Surfers/Configure Android Settings")]
        public static void ConfigureAndroidSettings()
        {
            Debug.Log("[Bihar Surfers] Configuring Android PlayerSettings...");

            PlayerSettings.companyName = "Manish Kumar";
            PlayerSettings.productName = "Bihar Surfers";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.manishkumar.biharsurfers");

            // Fixed Portrait for Runner Gameplay
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;

            // Android SDK target configuration (API 26 = Android 8.0 Oreo, modern baseline)
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel34;

            // ARM64 architecture support
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

            // Generate direct .apk file instead of Google Play .aab bundle
            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;

            // Configure local Android SDK if installed
            string localAppData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            string localSdk = Path.Combine(localAppData, "Android", "Sdk");
            if (Directory.Exists(localSdk))
            {
                EditorPrefs.SetString("AndroidSdkRoot", localSdk);
                Debug.Log($"[Bihar Surfers] Linked Android SDK: {localSdk}");
            }

            // Configure JDK
            string studioJbr = @"C:\Program Files\Android\Android Studio\jbr";
            string msJdk = @"C:\Program Files\Microsoft\jdk-17.0.20.1-hotspot";
            if (Directory.Exists(studioJbr))
            {
                EditorPrefs.SetString("JdkPath", studioJbr);
                Debug.Log($"[Bihar Surfers] Linked JDK: {studioJbr}");
            }
            else if (Directory.Exists(msJdk))
            {
                EditorPrefs.SetString("JdkPath", msJdk);
                Debug.Log($"[Bihar Surfers] Linked JDK: {msJdk}");
            }

            AssetDatabase.SaveAssets();
            Debug.Log("[Bihar Surfers] Android PlayerSettings successfully configured!");
        }

        [MenuItem("Bihar Surfers/Build Android APK")]
        public static void BuildAndroidAPK()
        {
            ConfigureAndroidSettings();

            string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "Builds", "Android");
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            string apkPath = Path.Combine(outputDir, "BiharSurfers.apk");
            Debug.Log($"[Bihar Surfers] Starting Android APK build to: {apkPath}");

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = Scenes,
                locationPathName = apkPath,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                FileInfo fi = new FileInfo(apkPath);
                long sizeMB = fi.Exists ? fi.Length / (1024 * 1024) : 0;
                Debug.Log($"[Bihar Surfers] SUCCESS: Android APK built successfully! Size: {sizeMB} MB at: {apkPath}");
            }
            else
            {
                Debug.LogError($"[Bihar Surfers] FAILED: Android APK build failed with result: {summary.result}. Total errors: {summary.totalErrors}");
            }
        }

        [MenuItem("Bihar Surfers/Build Windows Standalone (Validation)")]
        public static void BuildWindowsStandalone()
        {
            string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "Builds", "Windows");
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            string exePath = Path.Combine(outputDir, "BiharSurfers.exe");
            Debug.Log($"[Bihar Surfers] Starting Windows Standalone build to: {exePath}");

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = Scenes,
                locationPathName = exePath,
                target = BuildTarget.StandaloneWindows64,
                targetGroup = BuildTargetGroup.Standalone,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                FileInfo fi = new FileInfo(exePath);
                long sizeMB = fi.Exists ? fi.Length / (1024 * 1024) : 0;
                Debug.Log($"[Bihar Surfers] SUCCESS: Windows Standalone built successfully! Size: {sizeMB} MB at: {exePath}");
            }
            else
            {
                Debug.LogError($"[Bihar Surfers] FAILED: Windows Standalone build failed with result: {summary.result}. Total errors: {summary.totalErrors}");
            }
        }
    }
}
