# 臺灣暗棋

## 執行

Windows 直接執行 `dist/DarkChess.exe`；macOS 執行 `dist/DarkChess`。兩者都是單檔程式，已包含 Cython 模組、pygame、圖片、音效與中文字型；玩家不需要安裝 Python，也不需要另外複製 Image 或 Sound 資料夾。

點擊暗棋翻面；拖曳自己的明棋到合法位置移動或吃子。右上角「新局」沿用原有重新開局流程。

## 一鍵建置

### macOS

雙擊 `build_macos.command`。建置會建立獨立的 `.venv-macos`、安裝指定版本套件、重建美工、編譯 Cython、執行規則與動畫測試，再產生單一可執行檔 `dist/DarkChess`，並將它複製到只有該執行檔的獨立資料夾做遊戲與 spawn 子程序驗證。失敗時停止並保留錯誤訊息。

建置電腦需先具備：

- macOS 與 Python 3.9–3.12（包含 pip）。
- Xcode 或 Apple Command Line Tools；未安裝時先執行 `xcode-select --install`。
- 首次安裝套件需要網路。

命令列／自動化可執行：

```sh
./build_macos.command --no-pause
./dist/DarkChess
```

可使用 `PYTHON=python3.11 ./build_macos.command --no-pause` 指定首次建立虛擬環境的 Python。之後沿用 `.venv-macos`；若要更換 Python 或 CPU 架構，先移除該虛擬環境再建置。Apple Silicon 使用 arm64 Python，Intel Mac 使用 x86_64 Python；產物依建置環境的 Python 架構而定。請在要支援的最舊 macOS 上建置，再於目標系統驗證。

採用 [PyInstaller 單檔模式](https://www.pyinstaller.org/en/stable/usage.html)，產物是 Unix 可執行檔，啟動會解壓內含資源到暫存目錄。從 Finder 開啟時可能會顯示 Terminal 視窗。交付時只需複製 `dist/DarkChess` 並保留可執行權限（必要時執行 `chmod +x DarkChess`）。目前使用本機 ad-hoc 簽章，未做 Apple Developer ID 簽署或公證。

驗證報告寫入 `artifacts/macos-smoke/run-*/report.json`。也可單獨執行：

```sh
.venv-macos/bin/python tools/verify_exe.py
./dist/DarkChess --self-test "$PWD/artifacts/macos-manual-smoke"
```

### Windows

雙擊 `build_windows.bat`。建置會建立 `.venv`、安裝指定版本套件、重建美工、編譯 Cython、執行規則與動畫測試，再產生 `dist/DarkChess.exe`，並將 EXE 複製到獨立資料夾驗證執行。失敗時停止並保留錯誤訊息。

建置電腦需先具備：

- Python 3.9 x64（包含 pip 與 `py` launcher；沿用此專案的 Cython／CPython 3.9 相容環境）。
- Visual Studio 2019 或更新的 C++ Build Tools，包含「使用 C++ 的桌面開發」和 Windows SDK。
- 首次安裝套件需要網路；本工作目錄的 `.venv` 已建好。

命令列／自動化可執行 `build_windows.bat --no-pause`。一般玩家只需要 EXE，以上工具僅供重新建置使用。

## 美工與翻棋

全部既有圖片已重新繪製，包括棋盤、棋背、14 種棋面、選取狀態、新局按鈕與圖示，並同步更新舊 GIF 和 Android 圖片副本。Windows 使用帶透明度的 PNG。`tools/generate_art.py` 使用附帶的中文字型及四倍尺寸繪圖再縮小，固定棋字如下：

- 紅：帥、仕、相、俥、傌、炮、兵。
- 黑：將、士、象、車、馬、包、卒。

玩家與電腦的翻棋都經過約 420 毫秒的轉面動畫，包含背面、側邊與正面；動畫期間不接受重複點擊，但仍可關閉視窗。`presentation.py` 僅讀取棋盤狀態，不修改棋子、合法走法或 AI 決策。

棋背圖案為單一實心大圓。棋盤使用半張象棋圖案，包含 9 條直線、5 條橫線、九宮斜線及炮兵定位記號；暗棋的 8×4 格位置與操作方式維持原樣。

## 驗證

AI 已恢復為 Git 原版 `v1.2.2`（commit `55b68ed`），包含原有搜尋與評分函式。`tests/ai_reference.json` 記錄此版本，`tests/logic_hashes.json` 直接比對原版 AI／移動規則區段及棋子資料原始碼雜湊。

`tools/compare_ai.py` 可將相同棋局的候選搜尋輸入，分別交給 Git 原始 Cython 二進位模組與重新編譯的 AI，檢查搜尋輸出完全相同（需要 Git 歷史）。本次比對結果存於 `artifacts/ai-comparison.json`。

`tests/test_game.py` 將實際 Cython 移動函式抽出編譯，測試所有相鄰吃子階級組合、四邊邊界、禁止斜走、同色與暗棋阻擋、兵卒吃將帥、將帥不能吃兵卒、炮四方向隔一子吃明棋、不能隔兩子吃棋，以及動畫不改變棋子狀態。

額外端到端測試：

```powershell
.venv\Scripts\python.exe dchess.py --self-test C:\simon\darkchess\artifacts\source-smoke
dist\DarkChess.exe --self-test C:\simon\darkchess\artifacts\exe-smoke
```

自我測試使用 SDL 無視窗模式，執行實際遊戲迴圈，模擬玩家點擊翻棋、電腦回應、AI 對弈，並檢查 Windows spawn 子程序。輸出目錄會產生 `report.json` 和單次截圖 `game-<識別碼>.png`。建置驗證每次使用 `artifacts/exe-smoke/run-*` 獨立目錄，報告路徑會顯示在建置輸出。截圖僅供診斷，儲存失敗會記錄警告；遊戲與子程序驗證仍必須成功。此測試選項不影響一般開啟遊戲。

Android 僅同步美工資源；桌面單檔建置支援 Windows x64 與 macOS 原生架構。
