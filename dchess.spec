from pathlib import Path
root = Path(SPECPATH)
a = Analysis([str(root / 'dchess.py')], pathex=[str(root)],
    binaries=[], datas=[(str(root / 'Image'), 'Image'), (str(root / 'Sound'), 'Sound'), (str(root / 'wqy-zenhei.ttf'), '.')],
    hiddenimports=['darkchess', 'chess', 'chess_data', 'presentation', 'pygame', 'pygame.locals', 'random', 'math', 'copy', 'threading', 'multiprocessing'],
    hookspath=[], runtime_hooks=[], excludes=[], noarchive=False)
pyz = PYZ(a.pure)
exe = EXE(pyz, a.scripts, a.binaries, a.datas, [], name='DarkChess',
          debug=False, strip=False, upx=False, console=False, icon=str(root / 'darkchess_default.ico'))
