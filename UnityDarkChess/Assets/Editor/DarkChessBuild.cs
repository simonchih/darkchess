using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class DarkChessBuild
{
    [MenuItem("Dark Chess/Create Game Scene")]
    public static void CreateScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var camera = new GameObject("Camera").AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.10f, 0.15f, 0.14f);
        camera.gameObject.AddComponent<AudioListener>();
        new GameObject("DarkChess").AddComponent<DarkChessUnity.DarkChessGame>();
        Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/DarkChess.unity");
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Scenes/DarkChess.unity", true) };
        PlayerSettings.companyName = "DarkChess";
        PlayerSettings.productName = "Taiwan Dark Chess";
        PlayerSettings.defaultScreenWidth = 1042;
        PlayerSettings.defaultScreenHeight = 626;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.runInBackground = true;
        PlayerSettings.SetApiCompatibilityLevel(NamedBuildTarget.Standalone, ApiCompatibilityLevel.NET_Unity_4_8);
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Dark Chess/Build macOS")]
    public static void Mac()
    {
        CreateScene();
        Build("Builds/macOS/DarkChess.app", BuildTarget.StandaloneOSX);
    }

    [MenuItem("Dark Chess/Build Windows")]
    public static void Windows()
    {
        CreateScene();
        Build("Builds/Windows/DarkChess.exe", BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Dark Chess/Build Android ARM64 APK")]
    public static void AndroidArm64()
    {
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.SetIl2CppCompilerConfiguration(NamedBuildTarget.Android, Il2CppCompilerConfiguration.Release);
        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
            throw new Exception("Cannot switch to Android. Install Android Build Support in Unity Hub.");
        AssetDatabase.SaveAssets();
        Build("Builds/Android/DarkChess-arm64.apk", BuildTarget.Android);
    }

    static void Build(string path, BuildTarget target)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/DarkChess.unity" },
            locationPathName = path, target = target, options = BuildOptions.None
        });
        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception("Build failed: " + report.summary.result);
        Debug.Log("DARKCHESS_BUILD_PASS " + Path.GetFullPath(path));
    }
}
