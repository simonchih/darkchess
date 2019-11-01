from darkchess import main
from multiprocessing import set_start_method, freeze_support

if __name__ == "__main__":
    freeze_support()
    set_start_method('spawn')
    main(AI_vs_AI = 0)