#!/bin/sh
set -eu
repo=$(CDPATH= cd -- "$(dirname -- "$0")/../.." && pwd)
cd "$repo"
mkdir -p artifacts build/unity-oracle
export DEVELOPER_DIR=/Library/Developer/CommandLineTools
.venv-macos/bin/python -m unittest discover -s tests
.venv-macos/bin/python tools/unity/generate_oracle.py > artifacts/unity-oracle-build.log 2>&1
.venv-macos/bin/python tools/unity/extend_oracle.py > artifacts/unity-selection-oracle.log 2>&1
.venv-macos/bin/python tools/unity/history_oracle.py > artifacts/unity-history-oracle.log 2>&1
mcs -r:Microsoft.CSharp -r:System.Numerics -r:System.Web.Extensions \
  -out:build/unity-oracle/OracleRunner.exe UnityDarkChess/Assets/Scripts/Core/*.cs tools/unity/OracleRunner.cs
mono build/unity-oracle/OracleRunner.exe build/unity-oracle/oracle.json artifacts/unity-parity.json
mono build/unity-oracle/OracleRunner.exe build/unity-oracle/history.json artifacts/unity-history-parity.json
mcs -r:Microsoft.CSharp -r:System.Numerics \
  -out:build/unity-oracle/GameFlowRunner.exe UnityDarkChess/Assets/Scripts/Core/*.cs tools/unity/GameFlowRunner.cs
mono build/unity-oracle/GameFlowRunner.exe > artifacts/unity-game-flow.json
cat artifacts/unity-game-flow.json
