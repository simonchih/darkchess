@echo off
setlocal
cd /d "%~dp0"
"%~dp0.venv\Scripts\python.exe" "%~dp0dchess.py"
if errorlevel 1 pause
endlocal
