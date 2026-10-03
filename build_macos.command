#!/bin/bash
# Double-click in Finder, or run ./build_macos.command --no-pause.
set -euo pipefail
cd "$(dirname "$0")"

finish() {
  local status=$?
  if [ "$status" -ne 0 ]; then
    echo "BUILD FAILED. Review the error above."
  fi
  if [ "${1:-}" != "--no-pause" ] && [ -t 0 ]; then
    read -r -p "Press Return to close... " || true
  fi
  exit "$status"
}
trap 'finish "${1:-}"' EXIT
if [ "${1:-}" != "" ] && [ "${1:-}" != "--no-pause" ]; then
  echo "Usage: $0 [--no-pause]"
  exit 1
fi
if [ "$(uname -s)" != "Darwin" ]; then
  echo "This build must run on macOS."
  exit 1
fi
PYTHON_BIN="${PYTHON:-python3}"
"$PYTHON_BIN" -c 'import sys; assert (3, 9) <= sys.version_info[:2] <= (3, 12), "Use Python 3.9–3.12 with pip."'
if ! xcode-select -p >/dev/null 2>&1; then
  echo "Install Apple Command Line Tools first: xcode-select --install"
  exit 1
fi
echo "Building Taiwan Dark Chess for macOS..."
if [ ! -x .venv-macos/bin/python ]; then
  "$PYTHON_BIN" -m venv .venv-macos
fi
PY="$PWD/.venv-macos/bin/python"
# Keep build caches inside the project, including when launched from Finder.
export PYINSTALLER_CONFIG_DIR="$PWD/build/pyinstaller-config"
"$PY" -m pip install --no-cache-dir -r requirements.txt
"$PY" tools/generate_art.py
"$PY" setup.py build_ext --inplace --force
"$PY" -m unittest discover -s tests -v
"$PY" -m PyInstaller --noconfirm --clean dchess.spec
"$PY" tools/verify_exe.py
echo "SUCCESS: $PWD/dist/DarkChess"
"$PY" -c 'import platform; print("Architecture: " + platform.machine())'
