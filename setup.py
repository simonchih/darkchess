from distutils.core import setup
from Cython.Build import cythonize

setup(name='chess app',
      ext_modules=cythonize("*.pyx", language_level = "3"))