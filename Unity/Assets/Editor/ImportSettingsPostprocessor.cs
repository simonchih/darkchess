using UnityEditor;
using UnityEngine;

public sealed class ImportSettingsPostprocessor : AssetPostprocessor
{
    private const string ImagePrefix = "Assets/Resources/Image/";
    private const string SoundPrefix = "Assets/Resources/Sound/";

    private void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(ImagePrefix))
        {
            return;
        }

        var importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.filterMode = FilterMode.Bilinear;

        // Keep emulator/device rendering stable: avoid unsupported Android texture compression formats.
        var android = importer.GetPlatformTextureSettings("Android");
        android.overridden = true;
        android.maxTextureSize = 2048;
        android.format = TextureImporterFormat.RGBA32;
        android.textureCompression = TextureImporterCompression.Uncompressed;
        android.compressionQuality = 100;
        android.crunchedCompression = false;
        importer.SetPlatformTextureSettings(android);
    }

    private void OnPreprocessAudio()
    {
        if (!assetPath.StartsWith(SoundPrefix))
        {
            return;
        }

        var importer = (AudioImporter)assetImporter;
        var settings = importer.defaultSampleSettings;
        settings.loadType = AudioClipLoadType.DecompressOnLoad;
        settings.compressionFormat = AudioCompressionFormat.Vorbis;
        settings.quality = 0.75f;
        settings.preloadAudioData = true;
        importer.defaultSampleSettings = settings;
    }
}
