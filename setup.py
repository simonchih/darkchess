from setuptools import setup
from Cython.Build import cythonize
import os
from pathlib import Path

# Ensure compiler subprocesses can also locate the Windows resource compiler.
if os.name == 'nt':
    from setuptools._distutils._msvccompiler import _get_vc_env
    vc = _get_vc_env('x64')
    os.environ.update({key.upper(): value for key, value in vc.items()})
    tool_bin = Path(vc['vctoolsinstalldir']) / 'bin/Hostx64/x64'
    sdk_bin = Path(vc['windowssdkdir']) / 'bin' / vc['windowssdkversion'].rstrip('\\') / 'x64'
    os.environ['PATH'] = str(tool_bin) + os.pathsep + str(sdk_bin) + os.pathsep + os.environ['PATH']
    os.environ['DISTUTILS_USE_SDK'] = '1'

setup(name='chess app',
      ext_modules=cythonize(["chess_data.pyx", "chess.pyx", "darkchess.pyx"], build_dir="build/cython", language_level="3"))
