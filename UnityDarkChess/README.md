# 臺灣暗棋 — Unity 6000.3.2f1

完整原生 C# Unity 專案，支援桌面與 Android ARM64。執行與建置不需要 Python、Cython、pygame 或外部 AI 引擎。

## 開啟與遊玩

Unity Hub → Add project from disk → 選擇這個 `UnityDarkChess` 資料夾，使用 **6000.3.2f1** 開啟。開啟 `Assets/Scenes/DarkChess.unity`，按 Play。

點擊暗棋翻面；拖曳自己的明棋移動／吃子；右上角圖示重新開局。先手由原版隨機流程決定，第一次翻棋分配紅黑。電腦思考與動畫期間暫停輸入。終局顯示勝負並沿用原版 5 秒後重新開局。

macOS 已建好的獨立程式：`Builds/macOS/DarkChess.app`，可直接開啟，Unity Editor 與 Python 均不需要安裝。

## 建置

選單 **Dark Chess → Build macOS** 或 **Dark Chess → Build Windows**。Windows 建置需要先在 Unity Hub 安裝 Windows Build Support；目前實測平台是 Apple Silicon macOS。

命令列建置：

```sh
/Applications/Unity/Hub/Editor/6000.3.2f1/Unity.app/Contents/MacOS/Unity \
  -batchmode -nographics -quit -projectPath "$PWD" \
  -executeMethod DarkChessBuild.Mac -logFile build.log
```

桌面使用 Unity 的 Mono 與完整 .NET Framework 相容設定。

### Android ARM64 APK

先在 Unity Hub 為 **6000.3.2f1** 安裝 Android Build Support（含 SDK、NDK、OpenJDK）。選單 **Dark Chess → Build Android ARM64 APK** 會設定 IL2CPP、ARM64 與 Release C++ 編譯，產生 `Builds/Android/DarkChess-arm64.apk`。

在此資料夾執行命令列建置前，先關閉此專案的 Unity Editor：

```sh
/Applications/Unity/Hub/Editor/6000.3.2f1/Unity.app/Contents/MacOS/Unity \
  -batchmode -nographics -quit -projectPath "$PWD" -buildTarget Android \
  -executeMethod DarkChessBuild.AndroidArm64 -logFile android-build.log
```

AI 引擎使用 `object` 容器及明確型別轉換，不依賴 C# `dynamic` 的執行期繫結，避免 IL2CPP 產生大量 CallSite 程式碼而在 ARM64 C++ 編譯時耗用大量時間與記憶體。重新產生引擎時，`tools/unity/port_core.py` 也會保留此方式。APK 已驗證 ARM64 建置與簽章；手機上的遊玩仍需實機驗證。WebGL 尚未驗證。

## AI 與规则保留方式

`Assets/Scripts/Core/OriginalEngine.Generated.cs` 逐段移植原版 `darkchess.pyx` 的 45 個規則、翻棋、長捉、評分與搜尋函式。保留原始函式名稱、分支、候選遍歷順序、常數及剪枝條件，註明來源行號。`GameState.cs` 移植原版主迴圈的初始化、玩家操作、吃子數量、移動完成及勝負流程。

原版 multiprocessing 的每個搜尋子程序會載入獨立的模組狀態。Unity 版使用獨立 C# 引擎實例和深複製棋盤，交給 C# 平行工作執行；它們不共享搜尋暫存資料。結果沿用完成佇列順序及嚴格小於的選步比較。原版同分候選會受程序完成順序影響，Unity 版保留這個完成順序的選步方式；不宣稱兩個不同的平行執行每次都選到同一個同分走法。

`PortSemantics.cs` 保留來源的列表順序、座標值比較、整數轉換、深複製和 CPython MT19937 的 `randint` 行為。`SourceFloat.cs` 使用完全 C# 的二進位浮點融合乘加，匹配原版 macOS arm64 Cython 編譯器運算。隨機種子相同時，洗牌與翻棋選擇可以重現；正常遊戲使用新隨機種子。

`DarkChessGame.cs` 僅負責 Unity 的圖像、中文字型、拖曳、翻棋／移動動畫、音效及非同步思考。圖像、音效及中文字型沿用原專案。

## 與原版對照驗證

根目錄 `tools/unity` 的 Python 工具僅供開發時產生原版測試資料，**不會進入 Unity 專案或執行檔**。測試使用原始 Cython 私有函式及原始 `darkchess` 二進位搜尋，並以 IEEE 754 位元形式記錄浮點數，避免 JSON 再捨入。

驗證內容包含所有相鄰吃子階級、所有棋盤邊界、四方向炮隔子／暗棋限制、80 個局面的評分與威脅分析、原版搜尋候選、完整選步、固定種子的洗牌／翻棋，以及 Unity 獨立執行檔中的實際玩家操作與 AI 回應。結果摘要保存在根目錄 `tests/unity/verification.json`；本次詳細執行日誌在 `artifacts/unity-*.log`。

重新產生與執行對照資料：在儲存庫根目錄執行 `tools/unity/verify.sh`。這項開發驗證需要既有 Cython 虛擬環境、C 編譯器和 Mono C# 工具；Unity 遊戲不需要它們。

Unity 執行檔測試：

```sh
"Builds/macOS/DarkChess.app/Contents/MacOS/Taiwan Dark Chess" \
  -darkChessSmoke ../artifacts/unity-manual-smoke \
  -logFile ../artifacts/unity-manual.log
```

成功時輸出 `report.json` 和 Unity 視窗截圖，並自動結束。`-batchmode` 可驗證遊戲流程，但通常無法擷取視窗截圖。
