"""Verify the release executable with no source/resources beside it."""
import json
import os
from pathlib import Path
import shutil
import signal
import subprocess
import sys
import tempfile

root = Path(__file__).resolve().parents[1]
check_root = root / 'build/standalone-check'
check_root.mkdir(parents=True, exist_ok=True)
folder = Path(tempfile.mkdtemp(prefix='run-', dir=check_root))
output_root = root / ('artifacts/macos-smoke' if sys.platform == 'darwin' else 'artifacts/exe-smoke')
output_root.mkdir(parents=True, exist_ok=True)
output = Path(tempfile.mkdtemp(prefix='run-', dir=output_root))
report = output / 'report.json'
if report.exists():
    report.unlink()
name = 'DarkChess.exe' if sys.platform == 'win32' else 'DarkChess'
executable = folder / name
shutil.copy2(root / 'dist' / name, executable)
process = subprocess.Popen([str(executable), '--self-test', str(output)], cwd=folder,
                           start_new_session=os.name != 'nt')
try:
    code = process.wait(timeout=90)
except subprocess.TimeoutExpired:
    if os.name == 'nt':
        subprocess.run(['taskkill', '/PID', str(process.pid), '/T', '/F'], check=False)
    else:
        os.killpg(process.pid, signal.SIGKILL)
    process.wait()
    raise RuntimeError('Packaged game self-test timed out')
if code != 0:
    raise RuntimeError('Packaged game self-test failed: exit code ' + str(code))
result = json.loads(report.read_text(encoding='utf-8'))
assert (result['complete'] and result['human_reveals'] >= 2 and result['reveals'] >= 4
        and result['worker'] == 'compiled darkchess imported in spawned worker'), result
print('Standalone executable verified: player flip, AI replies, animation, resources and spawned worker.')
if 'screenshot_warning' in result:
    print('Optional screenshot could not be saved: ' + result['screenshot_warning'])
print('Verification report: ' + str(report))
