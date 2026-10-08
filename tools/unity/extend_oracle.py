"""Add complete source candidate selection and cannon/boundary fixtures."""
import json,os,random,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
sys.path[:0]=[str(ROOT),str(ROOT/'build/unity-oracle')]
os.environ['SDL_VIDEODRIVER']='dummy';os.environ['SDL_AUDIODRIVER']='dummy';os.environ['PYGAME_HIDE_SUPPORT_PROMPT']='1'
import _unity_oracle as o
import darkchess
from chess import chess
import struct

def enc(v):
 if isinstance(v,float):return {'float64':struct.pack('<d',v).hex()}
 if isinstance(v,chess):return {'piece':{k:enc(getattr(v,k)) for k in ('index','row','col','back','live','color','value','x','y','possible_move')}}
 if isinstance(v,(list,tuple)):return [enc(x) for x in v]
 return v
class Q:
 def __init__(self):self.values=[]
 def put(self,v):self.values.append(v)
 def get(self):return self.values.pop(0)
class Process:
 def __init__(self,target,args):self.args=args
 def start(self):darkchess.one_turn(*self.args)
o.Queue=Q;o.Process=Process
f=ROOT/'build/unity-oracle/oracle.json';root=json.loads(f.read_text());cases=root['cases'];rng=random.Random(600032)
for scenario in range(40):
 indices=rng.sample(range(32),rng.randint(3,8));positions=rng.sample(range(32),len(indices))
 entries=[[p//8,p%8,index,int(rng.random()<.25)] for p,index in zip(positions,indices)]
 if not any(e[2]>=16 and e[3]==0 for e in entries):entries[0][2]=31;entries[0][3]=0
 if not any(e[2]<16 and e[3]==0 for e in entries):entries[1][2]=15;entries[1][3]=0
 m,ch=o.configure(entries,0,1)
 # Restore source initial final_score and scratch state for every independent call.
 result=o.com_think(m,ch)
 cases.append({'method':'com_think','args':enc([m,ch]),'entries':entries,'pc':0,'cc':1,'sequential':True,'expected':enc(result)})
for r in range(4):
 for c in range(8):
  for dr,dc in ((1,0),(-1,0),(0,1),(0,-1)):
   if not (0<=r+2*dr<4 and 0<=c+2*dc<8):continue
   for screen in (0,1):
    for target in (0,1):
     for color in (0,1):
      entries=[[r,c,5+16*color,0],[r+dr,c+dc,9,screen],[r+2*dr,c+2*dc,15+16*(1-color),target]]
      m,ch=o.configure(entries,0,1)
      cases.append({'method':'collect_possible_move','args':enc([r,c,m,ch]),'entries':entries,'pc':0,'cc':1,'expected':enc(o.collect_possible_move(r,c,m,ch))})
f.write_text(json.dumps(root,separators=(',',':')))
print('Extended oracle:',len(cases),'cases')
