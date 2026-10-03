import hashlib
import json
import os
from pathlib import Path
import re
import subprocess
import sys
import unittest

os.environ['SDL_VIDEODRIVER'] = 'dummy'
os.environ['SDL_AUDIODRIVER'] = 'dummy'
os.environ['PYGAME_HIDE_SUPPORT_PROMPT'] = '1'
ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT))
import pygame
from chess import chess
from presentation import FlipAnimator, flip_surface


def logic(source):
    return source[source.index('cdef int can_be_ate_equal'):source.index('cdef void display_font')]


def compiled_rules():
    """Compile the actual private movement functions without exposing a game API."""
    source = (ROOT / 'darkchess.pyx').read_text(encoding='utf-8')
    functions = []
    for name in ('near', 'collect_possible_move'):
        match = re.search(r'^cdef list '+name+r'\(.*?(?=^(?:cdef |cpdef |def ))', source, re.M | re.S)
        functions.append(match.group(0).replace('cdef list '+name, 'cpdef list '+name, 1))
    folder = ROOT / 'build/rule_tests'
    folder.mkdir(parents=True, exist_ok=True)
    (folder / '_rule_probe.pyx').write_text('\n'.join(functions), encoding='utf-8')
    # Reuse the same compiler environment as the production build.
    setup = (ROOT / 'setup.py').read_text()
    setup = setup[:setup.index("setup(name=")]
    setup += 'setup(ext_modules=cythonize("_rule_probe.pyx", language_level="3"))\n'
    (folder / 'setup.py').write_text(setup)
    result = subprocess.run([sys.executable, 'setup.py', 'build_ext', '--inplace'], cwd=folder, capture_output=True, text=True, errors='replace')
    if result.returncode:
        raise RuntimeError(result.stdout + result.stderr)
    sys.path.insert(0, str(folder))
    from _rule_probe import collect_possible_move
    return collect_possible_move


class Rules(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.moves = staticmethod(compiled_rules())

    def board(self, entries):
        board = [[None]*8 for _ in range(4)]
        mapping = [[None]*8 for _ in range(4)]
        for r, c, index, back in entries:
            p = chess(index, (r, c)); p.back = back
            board[r][c] = p; mapping[r][c] = (r, c)
        return mapping, board

    def test_all_adjacent_capture_pairs(self):
        for color in (0, 1):
            for index in (0, 5, 7, 9, 11, 13, 15):
                for other in (0, 5, 7, 9, 11, 13, 15):
                    m, b = self.board([(1, 2, index+16*color, 0), (1, 3, other+16*(1-color), 0)])
                    v, target = b[1][2].value, b[1][3].value
                    expected = v != 2 and ((v == 1 and target == 7) or (v >= target and not (v == 7 and target == 1)))
                    self.assertEqual((1, 3) in self.moves(1, 2, m, b), expected)

    def test_board_edges_and_no_diagonal(self):
        for r in range(4):
            for c in range(8):
                m, b = self.board([(r, c, 15, 0)])
                expected = {(rr, cc) for rr in range(4) for cc in range(8) if abs(rr-r)+abs(cc-c) == 1}
                self.assertEqual(set(self.moves(r, c, m, b)), expected)

    def test_friendly_and_hidden_blocks(self):
        for index, back in ((9, 0), (25, 1)):
            m, b = self.board([(1, 2, 15, 0), (1, 3, index, back)])
            self.assertNotIn((1, 3), self.moves(1, 2, m, b))

    def test_cannon_screen_in_four_directions(self):
        for dr, dc in ((1,0), (-1,0), (0,1), (0,-1)):
            r, c = (0 if dr == 1 else 3 if dr == -1 else 1), (0 if dc == 1 else 7 if dc == -1 else 3)
            middle = (r+dr, c+dc)
            dest = (r+2*dr, c+2*dc)
            for screen_back in (0, 1):
                m, b = self.board([(r,c,5,0), (*middle,9,screen_back), (*dest,31,0)])
                self.assertIn(dest, self.moves(r,c,m,b))
                b[dest[0]][dest[1]].back = 1
                self.assertNotIn(dest, self.moves(r,c,m,b))
            m, b = self.board([(r,c,5,0), (*dest,31,0)])
            self.assertNotIn(dest, self.moves(r,c,m,b))
        m,b = self.board([(1,0,5,0), (1,1,9,1), (1,2,11,1), (1,3,31,0)])
        self.assertNotIn((1,3), self.moves(1,0,m,b))

    def test_ai_and_rules_unchanged(self):
        manifest = json.loads((ROOT / 'tests/logic_hashes.json').read_text())
        for path, digest in manifest.items():
            content = (ROOT / path).read_text(encoding='utf-8')
            if path == 'darkchess.pyx':
                content = logic(content)
            self.assertEqual(hashlib.sha256(content.encode()).hexdigest(), digest, path)


class Presentation(unittest.TestCase):
    def test_solid_back_and_complete_half_board_grid(self):
        from PIL import Image
        with Image.open(ROOT / 'Image/back.png') as back:
            for x in range(17, 40):
                for y in range(14, 37):
                    self.assertEqual(back.getpixel((x,y)), (35,77,71,255))
        with Image.open(ROOT / 'Image/SHEET.png') as board:
            xs = [34,91,148,205,260,317,374,431,488]
            ys = [51,108,165,222,279]
            for x in xs:
                for y in (80,136,193,250):
                    self.assertLess(board.getpixel((x,y))[0], 180, ('vertical',x,y))
            for y in ys:
                for x1,x2 in zip(xs,xs[1:]):
                    x = (x1+x2)//2
                    self.assertLess(board.getpixel((x,y))[0], 180, ('horizontal',x,y))

    def test_flip_has_back_edge_and_face(self):
        back = pygame.image.load(str(ROOT / 'Image/back.png'))
        face = pygame.image.load(str(ROOT / 'Image/RK.png'))
        self.assertEqual(flip_surface(back, face, 0).get_size(), (57,57))
        self.assertEqual(flip_surface(back, face, .5).get_width(), 2)
        self.assertEqual(pygame.image.tostring(flip_surface(back, face, 1), 'RGBA'), pygame.image.tostring(face, 'RGBA'))

    def test_animation_does_not_mutate_piece(self):
        pygame.init()
        screen = pygame.display.set_mode((521,313))
        from tools.generate_art import LABELS
        keys = ['BP','BC','BN','BR','BB','BA','BK','RP','RC','RN','RR','RB','RA','RK']
        images = [pygame.image.load(str(ROOT / 'Image' / (k+'.png'))) for k in keys+['back']]
        selected = [pygame.image.load(str(ROOT / 'Image' / (k+'S.png'))) for k in keys]
        p = chess(31, (0,0)); p.back = 0
        before = (p.index,p.row,p.col,p.x,p.y,p.back,p.live,p.color,p.value)
        FlipAnimator().present(screen, [[p]], selected, images, pygame.image.load(str(ROOT/'Image/SHEET.png')), pygame.image.load(str(ROOT/'Image/shield-and-swords.png')), lambda s: None)
        self.assertEqual(before, (p.index,p.row,p.col,p.x,p.y,p.back,p.live,p.color,p.value))
        self.assertEqual(''.join(LABELS[k] for k in keys[7:][::-1]), '帥仕相俥傌炮兵')
        self.assertEqual(''.join(LABELS[k] for k in keys[:7][::-1]), '將士象車馬包卒')

if __name__ == '__main__':
    unittest.main()
