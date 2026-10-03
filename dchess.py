import os
import sys
from pathlib import Path
from multiprocessing import set_start_method, freeze_support

if __name__ == "__main__":
    freeze_support()
    os.chdir(getattr(sys, '_MEIPASS', str(Path(__file__).resolve().parent)))
    set_start_method('spawn')
    if len(sys.argv) == 3 and sys.argv[1] == '--self-test':
        from smoke_test import run
        run(sys.argv[2])
        sys.exit(0)
    from darkchess import main
    main(AI_vs_AI = 0)
