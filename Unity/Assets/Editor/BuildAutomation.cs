using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public static class BuildAutomation
{
    private static readonly string[] BuildScenes =
    {
        "Assets/Scenes/Main.unity"
    };

    private static bool _verifyPlayEntered;
    private static double _verifyPlayDeadline;

    [MenuItem("Build/Build Android APK")]
    public static void BuildAndroidApk()
    {
        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
        {
            throw new Exception("Failed to switch build target to Android.");
        }

        PrepareAndroidCompatibilitySettings();

        var projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
        if (string.IsNullOrEmpty(projectRoot))
        {
            throw new Exception("Unable to resolve project root path.");
        }

        var outputDir = Path.Combine(projectRoot, "Builds", "Android");
        Directory.CreateDirectory(outputDir);

        var outputPath = Path.Combine(outputDir, "DarkChess.apk");

        var options = new BuildPlayerOptions
        {
            scenes = BuildScenes,
            locationPathName = outputPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new Exception($"Android build failed: {report.summary.result}");
        }

        Debug.Log($"Android build succeeded: {outputPath}");
    }

    [MenuItem("Build/Build Android APK (DarkChess_086d)")]
    public static void BuildAndroidApk086d()
    {
        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
        {
            throw new Exception("Failed to switch build target to Android.");
        }

        PrepareAndroidCompatibilitySettings();

        var projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
        if (string.IsNullOrEmpty(projectRoot))
        {
            throw new Exception("Unable to resolve project root path.");
        }

        var outputPath = Path.Combine(projectRoot, "DarkChess_086d.apk");
        var options = new BuildPlayerOptions
        {
            scenes = BuildScenes,
            locationPathName = outputPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new Exception($"Android build failed: {report.summary.result}");
        }

        Debug.Log($"Android build succeeded: {outputPath}");
    }

    [MenuItem("Build/Verify PlayMode Startup")]
    public static void VerifyPlayModeStartup()
    {
        _verifyPlayEntered = false;
        _verifyPlayDeadline = EditorApplication.timeSinceStartup + 45.0d;

        EditorApplication.playModeStateChanged -= OnVerifyPlayModeStateChanged;
        EditorApplication.update -= OnVerifyPlayModeUpdate;

        EditorApplication.playModeStateChanged += OnVerifyPlayModeStateChanged;
        EditorApplication.update += OnVerifyPlayModeUpdate;

        EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
        EditorApplication.EnterPlaymode();
    }

    private static void OnVerifyPlayModeStateChanged(PlayModeStateChange state)
    {
        Debug.Log($"PlayMode smoke state: {state}");

        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            _verifyPlayEntered = true;
            EditorApplication.ExitPlaymode();
            return;
        }

        if (state == PlayModeStateChange.EnteredEditMode && _verifyPlayEntered)
        {
            CleanupVerifyCallbacks();
            Debug.Log("PlayMode smoke check passed.");
            EditorApplication.Exit(0);
        }
    }

    private static void OnVerifyPlayModeUpdate()
    {
        if (EditorApplication.timeSinceStartup <= _verifyPlayDeadline)
        {
            return;
        }

        CleanupVerifyCallbacks();
        Debug.LogError("PlayMode smoke check timed out.");
        EditorApplication.Exit(1);
    }

    private static void CleanupVerifyCallbacks()
    {
        EditorApplication.playModeStateChanged -= OnVerifyPlayModeStateChanged;
        EditorApplication.update -= OnVerifyPlayModeUpdate;
    }

    private static void PrepareAndroidCompatibilitySettings()
    {
        // BlueStacks and some Android emulators are sensitive to shader stripping and texture compression.
        EnsureAndroidGraphicsApi();
        EnsureAlwaysIncludedShaders();
        ForceAndroidSpriteTexturesToRgba32();
        PlayerSettings.stripEngineCode = false;
        AssetDatabase.SaveAssets();
    }

    private static void EnsureAndroidGraphicsApi()
    {
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
    }

    private static void EnsureAlwaysIncludedShaders()
    {
        var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset");
        if (assets == null || assets.Length == 0)
        {
            Debug.LogWarning("Unable to load ProjectSettings/GraphicsSettings.asset.");
            return;
        }

        var serialized = new SerializedObject(assets[0]);
        var alwaysIncluded = serialized.FindProperty("m_AlwaysIncludedShaders");
        if (alwaysIncluded == null || !alwaysIncluded.isArray)
        {
            Debug.LogWarning("m_AlwaysIncludedShaders was not found in GraphicsSettings.asset.");
            return;
        }

        EnsureShaderInList(alwaysIncluded, "UI/Default");
        EnsureShaderInList(alwaysIncluded, "UI/Default Font");
        EnsureShaderInList(alwaysIncluded, "Sprites/Default");
        EnsureShaderInList(alwaysIncluded, "GUI/Text Shader");
        EnsureShaderInList(alwaysIncluded, "Unlit/Texture");
        EnsureShaderInList(alwaysIncluded, "Unlit/Transparent");
        EnsureShaderInList(alwaysIncluded, "DarkChess/UnlitSprite");

        serialized.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(assets[0]);
    }

    private static void EnsureShaderInList(SerializedProperty shaderArray, string shaderName)
    {
        var shader = Shader.Find(shaderName);
        if (shader == null)
        {
            Debug.LogWarning($"Shader not found and could not be forced into Always Included list: {shaderName}");
            return;
        }

        for (var i = 0; i < shaderArray.arraySize; i++)
        {
            var item = shaderArray.GetArrayElementAtIndex(i);
            if (item.objectReferenceValue == shader)
            {
                return;
            }
        }

        var index = shaderArray.arraySize;
        shaderArray.InsertArrayElementAtIndex(index);
        shaderArray.GetArrayElementAtIndex(index).objectReferenceValue = shader;
    }

    private static void ForceAndroidSpriteTexturesToRgba32()
    {
        var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Resources/Image" });
        var changed = 0;

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                continue;
            }

            var dirty = false;
            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                dirty = true;
            }

            if (importer.spriteImportMode != SpriteImportMode.Single)
            {
                importer.spriteImportMode = SpriteImportMode.Single;
                dirty = true;
            }

            var android = importer.GetPlatformTextureSettings("Android");
            if (!android.overridden ||
                android.format != TextureImporterFormat.RGBA32 ||
                android.textureCompression != TextureImporterCompression.Uncompressed)
            {
                android.overridden = true;
                android.maxTextureSize = 2048;
                android.format = TextureImporterFormat.RGBA32;
                android.textureCompression = TextureImporterCompression.Uncompressed;
                android.compressionQuality = 100;
                android.crunchedCompression = false;
                importer.SetPlatformTextureSettings(android);
                dirty = true;
            }

            if (dirty)
            {
                importer.SaveAndReimport();
                changed++;
            }
        }

        Debug.Log($"Android texture compatibility pass complete. Updated textures: {changed}");
    }
}
