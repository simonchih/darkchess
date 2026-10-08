"""Compile private ORIGINAL Cython functions and record independent C# parity fixtures."""
import struct
import hashlib
import json
import os
from pathlib import Path
import random
import re
import subprocess
import sys
ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0,str(ROOT))
os.environ['SDL_VIDEODRIVER']='dummy'
os.environ['SDL_AUDIODRIVER']='dummy'
os.environ['PYGAME_HIDE_SUPPORT_PROMPT']='1'
FOLDER=ROOT/'build/unity-oracle'
FOLDER.mkdir(parents=True,exist_ok=True)
src=(ROOT/'darkchess.pyx').read_text(encoding='utf-8-sig')
probe=src[:src.index('cdef void display_font')]
probe=re.sub(r'^cdef (int|double|bint|void|list) (\w+)\(',r'cpdef \1 \2(',probe,flags=re.M)
probe+='\n'+src[src.index('def clean_back_n1_to_0'):src.index('def main(')]
probe+='''
def configure(entries, pc, cc):
    global main_chess, main_map, player_color, com_color, back_num
    global back_value_num, king_live, com_ban_step
    main_chess = [[chess(32,(r,c)) for c in range(8)] for r in range(4)]
    main_map = [[None]*8 for r in range(4)]
    player_color, com_color = pc, cc
    back_num = 0
    back_value_num[0][:] = [0,0,0,0,0,0,0,0]
    back_value_num[1][:] = [0,0,0,0,0,0,0,0]
    king_live[:] = [0,0]
    com_ban_step = []
    for row in main_chess:
        for p in row: p.live = 0
    for r,c,index,back in entries:
        p = chess(index,(r,c)); p.back = back
        main_chess[r][c] = p; main_map[r][c] = (r,c)
        if back == 1:
            back_num += 1
            back_value_num[p.color][p.value] += 1
    scan_king(main_chess)
    return main_map, main_chess
'''
(FOLDER/'_unity_oracle.pyx').write_text(probe)
(FOLDER/'setup.py').write_text('from setuptools import setup\nfrom Cython.Build import cythonize\nsetup(ext_modules=cythonize("_unity_oracle.pyx", include_path=['+repr(str(ROOT))+'], language_level=3))\n')
env=dict(os.environ,DEVELOPER_DIR='/Library/Developer/CommandLineTools')
subprocess.run([sys.executable,'setup.py','build_ext','--inplace'],cwd=FOLDER,env=env,check=True,stdout=subprocess.DEVNULL)
sys.path.insert(0,str(FOLDER))
import _unity_oracle as oracle
from chess import chess

def encode(x):
    if isinstance(x,float):return {"float64":struct.pack("<d",x).hex()}
    if isinstance(x,chess):return {'piece':{'index':x.index,'row':x.row,'col':x.col,'back':x.back,'live':x.live,'color':x.color,'value':x.value,'x':x.x,'y':x.y,'possible_move':encode(x.possible_move)}}
    if isinstance(x,(list,tuple)):return [encode(v) for v in x]
    return x
cases=[]
def record(method,args, entries=None,pc=0,cc=1):
    try:
        result=getattr(oracle,method)(*args)
    except Exception as e: raise RuntimeError((method,entries,args)) from e
    cases.append({'method':method,'args':encode(args),'entries':entries,'pc':pc,'cc':cc,'expected':encode(result)})

for a in range(1,8):
    for b in range(1,8):
        record('can_be_ate',[a,b]);record('can_be_ate_equal',[a,b])
for color in (0,1):
    for a in (0,5,7,9,11,13,15):
        for b in (0,5,7,9,11,13,15):
            entries=[[1,2,a+16*color,0],[1,3,b+16*(1-color),0]]
            m,ch=oracle.configure(entries,color,1-color)
            record('collect_possible_move',[1,2,m,ch],entries,color,1-color)
for r in range(4):
    for c in range(8):
        entries=[[r,c,15,0]];m,ch=oracle.configure(entries,0,1)
        record('near',[r,c]);record('collect_possible_move',[r,c,m,ch],entries)

rng=random.Random(721)
searches=0
class Queue:
    def put(self,v):self.result=v
for scenario in range(80):
    slots=rng.sample(range(32),rng.randint(3,9))
    entries=[[pos//8,pos%8,rng.randrange(32),int(rng.random()<.25)] for pos in slots]
    # Ensure both sides have an open piece and keep inventories physically valid.
    indices=rng.sample(range(32),len(slots))
    for e,i in zip(entries,indices):e[2]=i
    entries[0][2]=rng.randrange(16);entries[0][3]=0
    entries[1][2]=rng.randrange(16,32);entries[1][3]=0
    m,ch=oracle.configure(entries,0,1)
    for e in entries:
        r,c,_,back=e
        record('collect_possible_move',[r,c,m,ch],entries)
        if back:continue
        for method in ('eat_by_bomb','if_cannon_can_eat','eat_by_player_bomb','stand_will_dead_pity'):
            args=[(r,c),m,ch] if method=='eat_by_bomb' else [(r,c),m,ch,0] if method in ('if_cannon_can_eat','eat_by_player_bomb') else [(r,c),ch,m,0]
            record(method,args,entries)
        for dest in oracle.collect_possible_move(r,c,m,ch):
            for owner in (0,1):
                record('will_dead_pity',[(r,c),dest,ch,m,owner],entries)
                record('will_dead_pity_even_equal',[(r,c),dest,ch,m,owner],entries)
                record('escape_way_to_run',[(r,c),dest,ch,m,owner],entries)
                record('caca',[(r,c),dest,ch,m,owner],entries)
                record('near2_have_same_value',[(r,c),ch,m,owner],entries)
                record('first_move_score',[(r,c),dest,ch,m,owner,0,1,[],[1,1]],entries)
                record('move_score',[(r,c),dest,ch,m,owner,0,1,[],[1,1],3],entries)
    # Exercise exactly the public original one_turn search on sparse boards.
    if scenario < 30:
        import darkchess
        oracle.all_chess_move(m,ch)
        moves=[]
        hidden=sum(e[3] for e in entries)
        if hidden:moves.append((None,None,.01,0))
        for row in ch:
            for p in row:
                if p.live and p.back<1 and p.color==1:
                    for dest in p.possible_move:
                        score=-oracle.first_move_score((p.row,p.col),dest,ch,m,1,0,1,[],[1,1])
                        moves.append(((p.row,p.col),dest,score,0))
        for index,mm in enumerate(moves[:4]):
            q=Queue()
            args=[m,ch,mm,0,mm[0],mm[1],mm[2],mm[3],.90,index,9000.,-9000.,0,1,hidden,[],[1,1],[None]*len(moves)]
            darkchess.one_turn(q,*args)
            cases.append({'method':'one_turn','args':encode(args),'expected':encode(q.result),'fresh':True})
            searches+=1
# Reproducible shuffle and reveal selectors include source RNG behavior.
for seed in (0,1,7,721,123456):
    random.seed(seed)
    cases.append({'method':'ini_random_chess','args':[[0]*32],'seed':seed,'expected':oracle.ini_random_chess([0]*32)})
    entries=[[0,0,5,0],[0,2,31,1],[1,1,16,0],[2,3,13,1],[3,7,9,1]]
    m,ch=oracle.configure(entries,0,1)
    random.seed(seed)
    cases.append({'method':'select_back_chess','args':encode([m,ch,None]),'entries':entries,'pc':0,'cc':1,'seed':seed,'expected':encode(oracle.select_back_chess(m,ch))})
output=FOLDER/'oracle.json'
output.parent.mkdir(parents=True,exist_ok=True)
output.write_text(json.dumps({'source_sha256':hashlib.sha256(src.encode()).hexdigest(),'cases':cases},separators=(',',':')))
print(json.dumps({'cases':len(cases),'original_binary_searches':searches,'fixture':str(output)}))
