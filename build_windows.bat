@echo off
setlocal
cd /d "%~dp0"
echo Building Taiwan Dark Chess...
py -3.9 -c "import struct; assert struct.calcsize('P') == 8" >nul 2>&1
if errorlevel 1 (
  echo Install Python 3.9 x64 with pip and the Python launcher first.
  goto fail
)
if not exist ".venv\Scripts\python.exe" py -3.9 -m venv .venv
if errorlevel 1 goto fail
set "PY=.venv\Scripts\python.exe"
"%PY%" -m pip install -r requirements.txt
if errorlevel 1 goto fail
"%PY%" tools\generate_art.py
if errorlevel 1 goto fail
"%PY%" setup.py build_ext --inplace
if errorlevel 1 (
  echo C compilation requires Visual Studio Build Tools: Desktop development with C++ and Windows SDK.
  goto fail
)
"%PY%" -m unittest discover -s tests -v
if errorlevel 1 goto fail
"%PY%" -m PyInstaller --noconfirm --clean dchess.spec
if errorlevel 1 goto fail
"%PY%" tools\verify_exe.py
if errorlevel 1 goto fail
echo SUCCESS: %CD%\dist\DarkChess.exe
if /i not "%~1"=="--no-pause" pause
exit /b 0
:fail
echo BUILD FAILED. Review the error above.
if /i not "%~1"=="--no-pause" pause
exit /b 1
