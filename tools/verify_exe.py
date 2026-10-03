"""Verify the release executable with no source/resources beside it."""
import json
from pathlib import Path
import shutil
import subprocess
import tempfile

root = Path(__file__).resolve().parents[1]
folder = root / 'build/standalone-check'
output_root = root / 'artifacts/exe-smoke'
folder.mkdir(parents=True, exist_ok=True)
output_root.mkdir(parents=True, exist_ok=True)
output = Path(tempfile.mkdtemp(prefix='run-', dir=output_root))
report = output / 'report.json'
if report.exists():
    report.unlink()
executable = folder / 'DarkChess.exe'
shutil.copy2(root / 'dist/DarkChess.exe', executable)
process = subprocess.Popen([str(executable), '--self-test', str(output)], cwd=folder)
try:
    code = process.wait(timeout=90)
except subprocess.TimeoutExpired:
    subprocess.run(['taskkill', '/PID', str(process.pid), '/T', '/F'], check=False)
    raise RuntimeError('Packaged game self-test timed out')
if code != 0:
    raise RuntimeError('Packaged game self-test failed: exit code ' + str(code))
result = json.loads(report.read_text(encoding='utf-8'))
assert result['complete'] and result['human_reveals'] >= 2 and result['reveals'] >= 4, result
print('Standalone EXE verified: player flip, AI replies, animation, resources and spawned worker.')
if 'screenshot_warning' in result:
    print('Optional screenshot could not be saved: ' + result['screenshot_warning'])
print('Verification report: ' + str(report))
