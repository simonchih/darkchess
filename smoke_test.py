"""Opt-in end-to-end check, also runnable inside the single-file executable."""
import json
import io
import os
from pathlib import Path
import time
import uuid


def save_snapshot(screen, output, report):
    """A diagnostic screenshot must not fail the actual game verification."""
    import pygame
    try:
        buffer = io.BytesIO()
        pygame.image.save(screen, buffer, 'game.png')
        path = Path(output) / ('game-' + uuid.uuid4().hex + '.png')
        path.write_bytes(buffer.getvalue())
        report['screenshot'] = path.name
    except (OSError, pygame.error) as error:
        report['screenshot_warning'] = str(error)


def run(output):
    output = Path(output).resolve()
    output.mkdir(parents=True, exist_ok=True)
    os.environ['SDL_VIDEODRIVER'] = 'dummy'
    os.environ['SDL_AUDIODRIVER'] = 'dummy'
    import pygame
    import presentation
    import darkchess
    import random
    report = {'frames': 0, 'reveals': 0, 'human_click': False, 'human_reveals': 0, 'complete': False}
    phase = 'human'
    original = presentation.FlipAnimator.present
    start = time.monotonic()

    def observe(self, screen, board, *args):
        report['reveals'] += sum(self.previous.get((p.row,p.col), False) and p.back != 1 for row in board for p in row if p.live)
        original(self, screen, board, *args)
        report['frames'] += 1
        if phase == 'human' and not report['human_click']:
            pygame.event.post(pygame.event.Event(pygame.MOUSEBUTTONDOWN, button=1, pos=(62,79)))
            report['human_click'] = True
        if phase == 'human' and report['reveals'] >= 2:
            pygame.event.post(pygame.event.Event(pygame.QUIT))
        if report['reveals'] >= 8 or time.monotonic()-start > 35:
            if phase == 'ai' and not any(key in report for key in ('screenshot', 'screenshot_warning')):
                save_snapshot(screen, output, report)
            pygame.event.post(pygame.event.Event(pygame.QUIT))
    presentation.FlipAnimator.present = observe

    # A real spawn/Queue round trip validates the frozen worker bootstrap.
    from multiprocessing import get_context
    context = get_context('spawn')
    q = context.Queue()
    worker = context.Process(target=worker_probe, args=(q,))
    worker.start()
    report['worker'] = q.get(timeout=20)
    worker.join(20)
    if worker.is_alive():
        worker.terminate(); worker.join()
        raise RuntimeError('Worker did not exit')
    assert worker.exitcode == 0
    q.close()
    q.join_thread()
    original_mouse = pygame.mouse.get_pos
    pygame.mouse.get_pos = lambda: (62,79)
    random.seed(0)  # Reproducible player-first smoke scenario only.
    try:
        darkchess.main(AI_vs_AI=0)
    except SystemExit:
        report['human_reveals'] = report['reveals']
    finally:
        pygame.mouse.get_pos = original_mouse
    assert report['human_reveals'] >= 2, 'Player click and AI reply did not complete'
    phase = 'ai'
    report['reveals'] = 0
    try:
        darkchess.main(AI_vs_AI=1)
    except SystemExit:
        report['complete'] = report['reveals'] >= 4 and report['human_reveals'] >= 2
    finally:
        (output/'report.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
        pygame.quit()
    if not report['complete']:
        raise RuntimeError('Game smoke test did not complete')


def worker_probe(queue):
    import darkchess
    queue.put('compiled darkchess imported in spawned worker')
