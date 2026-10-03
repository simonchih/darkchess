"""Reproducible, supersampled vector artwork and exact Traditional Chinese glyphs."""
from pathlib import Path
import io
import os
import time
from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parents[1]
S = 4
FONT = ROOT / 'wqy-zenhei.ttf'
LABELS = {'BK': '將', 'BA': '士', 'BB': '象', 'BR': '車', 'BN': '馬', 'BC': '包', 'BP': '卒',
          'RK': '帥', 'RA': '仕', 'RB': '相', 'RR': '俥', 'RN': '傌', 'RC': '炮', 'RP': '兵'}

def save(im, path, **options):
    """Skip identical files and atomically replace artwork on Windows."""
    path = Path(path)
    buffer = io.BytesIO()
    im.save(buffer, format=Image.registered_extensions()[path.suffix.lower()], **options)
    data = buffer.getvalue()
    if path.exists() and path.read_bytes() == data:
        return
    temporary = path.with_name(path.name + '.tmp')
    for attempt in range(5):
        try:
            temporary.write_bytes(data)
            os.replace(temporary, path)
            return
        except OSError:
            if attempt == 4:
                raise
            time.sleep(0.1 * (attempt+1))

def canvas(w, h):
    return Image.new('RGBA', (w*S, h*S))

def ellipse(d, box, fill, outline=None, width=1):
    d.ellipse(tuple(round(v*S) for v in box), fill, outline, width*S)

def rect(d, box, fill, radius=0, outline=None, width=1):
    d.rounded_rectangle(tuple(round(v*S) for v in box), radius*S, fill, outline, width*S)

def text(d, xy, label, size, fill, anchor='mm'):
    d.text(tuple(v*S for v in xy), label, font=ImageFont.truetype(str(FONT), size*S), fill=fill, anchor=anchor)

def finish(im):
    return im.resize((im.width//S, im.height//S), Image.Resampling.LANCZOS)

def piece(label='', red=False, selected=False, back=False):
    im = canvas(57, 57)
    d = ImageDraw.Draw(im)
    ellipse(d, (3, 5, 55, 56), '#00000045')
    if selected:
        ellipse(d, (0, 0, 56, 56), '#edbb53')
    ellipse(d, (3, 2, 53, 53), '#704629')
    ellipse(d, (3, 1, 53, 50), '#c59959')
    ellipse(d, (5, 2, 51, 48), '#f4dfac')
    ellipse(d, (7, 4, 49, 46), '#e9cb90')
    ellipse(d, (8, 4, 48, 44), '#f5e6c6')
    ink = '#a32d2b' if red else '#233736'
    if back:
        ellipse(d, (6, 3, 50, 47), '#234d47')
    else:
        ellipse(d, (7, 4, 49, 46), None, ink)
        text(d, (28, 24), label, 34, ink)
    return finish(im)

def board():
    im = canvas(521, 313)
    d = ImageDraw.Draw(im)
    rect(d, (0, 0, 521, 313), '#142f2d')
    rect(d, (9, 8, 512, 305), '#21433c', 13, '#688167')
    rect(d, (20, 42, 500, 287), '#aa7947', 9, '#dbc38e')
    rect(d, (27, 47, 493, 281), '#d9b780', 5)
    # One half of a Xiangqi board: 9 files x 5 ranks, 8 x 4 cells.
    # Keep the original game's piece origins and mouse hit areas.
    xs = [34, 91, 148, 205, 260, 317, 374, 431, 488]
    ys = [51, 108, 165, 222, 279]
    ink = '#65442a'

    def line(points, width=1):
        d.line([(x*S, y*S) for x, y in points], fill=ink, width=width*S)

    for x in xs:
        line([(x, ys[0]), (x, ys[-1])])
    for y in ys:
        line([(xs[0], y), (xs[-1], y)])
    # Palace of the lower half; retain the grid through the diagonals.
    line([(xs[3], ys[2]), (xs[5], ys[4])])
    line([(xs[5], ys[2]), (xs[3], ys[4])])
    # Traditional L-shaped placement marks for cannons and soldiers.
    for col, row in [(1, 2), (7, 2), (0, 1), (2, 1), (4, 1), (6, 1), (8, 1)]:
        x, y = xs[col], ys[row]
        for dx in (-1, 1):
            if (col == 0 and dx == -1) or (col == 8 and dx == 1):
                continue
            for dy in (-1, 1):
                line([(x+dx*10, y+dy*4), (x+dx*4, y+dy*4), (x+dx*4, y+dy*10)])
    text(d, (34, 22), '臺灣暗棋', 20, '#f1dfb7', 'lm')
    rect(d, (225, 10, 339, 36), '#f3e3bf', 7)
    text(d, (260, 296), '翻棋見招 · 步步為營', 10, '#c7c6a6')
    return finish(im)

def generate():
    out = ROOT / 'Image'
    out.mkdir(exist_ok=True)
    images = {}
    for key, label in LABELS.items():
        images[key+'.GIF'] = piece(label, key.startswith('R'))
        images[key+'S.GIF'] = piece(label, key.startswith('R'), True)
    images['back.gif'] = piece(back=True)
    images['SHEET.gif'] = board()
    button = canvas(60, 26)
    d = ImageDraw.Draw(button)
    rect(d, (0, 0, 59, 25), '#31594d', 7, '#bda16a')
    text(d, (30, 12), '新局', 14, '#f1dfb7')
    images['shield-and-swords.gif'] = finish(button)
    images['BKM.GIF'] = images['BKS.GIF']
    images['RKM.GIF'] = images['RKS.GIF']
    images['OO.GIF'] = piece()
    images['OOS.GIF'] = piece(selected=True)
    for name, im in images.items():
        # Native PNG counterparts retain smooth alpha in the Windows game.
        save(im, out / (Path(name).stem+'.png'))
        # Refresh legacy assets too, including the Android resource mirror.
        palette = im.convert('RGB').quantize(colors=255)
        alpha = im.getchannel('A')
        palette.paste(255, mask=alpha.point(lambda a: 255 if a < 128 else 0))
        save(palette, out / name, transparency=255)
        mirror = ROOT / 'android' / 'Image' / name
        if mirror.exists():
            save(palette, mirror, transparency=255)
    icon = piece('帥', True).resize((256, 256), Image.Resampling.LANCZOS)
    save(icon, out / 'darkchess_default.png')
    for dest in (ROOT / 'darkchess_default.ico', out / 'darkchess_default.ico', ROOT / 'android/Image/darkchess_default.ico'):
        save(icon, dest, sizes=[(16,16), (32,32), (48,48), (256,256)])
    save(icon, ROOT / 'android/Image/darkchess_default.png')
    preview = Image.new('RGB', (521, 155), '#21433c')
    for n, (key, label) in enumerate(LABELS.items()):
        im = piece(label, key.startswith('R'))
        preview.paste(im, (58+(n%7)*58, 15+(n//7)*66), im)
    (ROOT / 'artifacts').mkdir(exist_ok=True)
    save(preview, ROOT / 'artifacts/pieces.png')

if __name__ == '__main__':
    generate()
