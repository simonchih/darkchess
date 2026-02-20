# DarkChess Unity 6000.3 Project

This project is converted to Unity `6000.3.2f1`.

## Folder layout

- `Assets/Resources/Image`: converted image assets (GIF -> PNG).
- `Assets/Resources/Sound`: original WAV sound assets.
- `Assets/Scripts/DarkChessGameController.cs`: core game logic, UI, and Monte Carlo AI.
- `Assets/Scenes/Main.unity`: startup scene.
- `Assets/Editor/BuildAutomation.cs`: Android build entry.

## Android landscape settings

The project is configured for Android landscape gameplay:

- Portrait autorotate is disabled.
- Landscape left/right autorotate is enabled.
- Runtime code also enforces landscape-only orientation.

## Build Android APK

### Unity Editor

1. Open this project with Unity `6000.3.2f1`.
2. Run `Build > Build Android APK`.
3. Output: `Builds/Android/DarkChess.apk`.

### Command line

```bash
"/Applications/Unity/Hub/Editor/6000.3.2f1/Unity.app/Contents/MacOS/Unity" \
  -batchmode -nographics -quit \
  -projectPath "/Users/fit0721/Documents/GitHub/darkchess_unity_v086/Unity" \
  -executeMethod BuildAutomation.BuildAndroidApk \
  -logFile "/Users/fit0721/Documents/GitHub/darkchess_unity_v086/Unity/unity_android_build.log"
```
