"""Replay the same search inputs through the original and rebuilt Cython AI."""
import json
import os
from pathlib import Path
import pickle
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[1]
FOLDER = ROOT / 'build/ai-reference'


def capture():
    os.environ['SDL_VIDEODRIVER'] = 'dummy'
    os.environ['SDL_AUDIODRIVER'] = 'dummy'
    sys.path.insert(0, str(ROOT))
    import darkchess
    import random
    random.seed(0)
    inputs = []

    class CaptureProcess:
        def __init__(self, target, args):
            inputs.append(args[1:])
            if len(inputs) == len(args[-1]):
                (FOLDER / 'inputs.pickle').write_bytes(pickle.dumps(inputs))
                raise SystemExit

        def start(self):
            pass

    darkchess.Process = CaptureProcess
    darkchess.main(AI_vs_AI=1)


def probe(module_folder, output):
    os.environ['SDL_VIDEODRIVER'] = 'dummy'
    os.environ['SDL_AUDIODRIVER'] = 'dummy'
    sys.path.insert(0, str(ROOT))
    sys.path.insert(0, module_folder)
    import darkchess
    results = []

    class Results:
        def put(self, result):
            results.append(result)

    inputs = pickle.loads((FOLDER / 'inputs.pickle').read_bytes())
    for args in inputs:
        darkchess.one_turn(Results(), *args)
    assert len(results) == len(inputs)
    Path(output).write_text(json.dumps(results, indent=2), encoding='utf-8')


def compare():
    FOLDER.mkdir(parents=True, exist_ok=True)
    reference = FOLDER / 'darkchess.cp39-win_amd64.pyd'
    reference.write_bytes(subprocess.check_output(['git', 'show', '55b68ed:darkchess.cp39-win_amd64.pyd'], cwd=ROOT))
    subprocess.run([sys.executable, __file__, 'capture'], cwd=ROOT, check=True, timeout=30)
    outputs = []
    for label, folder in [('original', FOLDER), ('restored', ROOT)]:
        output = FOLDER / (label+'.json')
        subprocess.run([sys.executable, __file__, 'probe', str(folder), str(output)], cwd=ROOT, check=True, timeout=60)
        outputs.append(json.loads(output.read_text(encoding='utf-8')))
    assert outputs[0] == outputs[1], 'Restored AI differs from original compiled AI'
    report = {'reference': '55b68ed (v1.2.2)', 'candidate_count': len(outputs[0]), 'exact_search_results_match': True}
    (ROOT / 'artifacts/ai-comparison.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
    print(json.dumps(report))


if __name__ == '__main__':
    if len(sys.argv) == 1:
        compare()
    elif sys.argv[1] == 'capture':
        capture()
    elif sys.argv[1] == 'probe':
        probe(sys.argv[2], sys.argv[3])
