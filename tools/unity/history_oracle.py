"""Check long-capture history side effects against the unmodified source function."""
import json,os,random,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
sys.path[:0]=[str(ROOT),str(ROOT/'build/unity-oracle')]
os.environ['SDL_VIDEODRIVER']='dummy';os.environ['SDL_AUDIODRIVER']='dummy';os.environ['PYGAME_HIDE_SUPPORT_PROMPT']='1'
import _unity_oracle as o
from chess import chess
src=(ROOT/'darkchess.pyx').read_text(encoding='utf-8-sig')
function=re.search(r'^cdef void save_step_and_break_long_capture\(.*?(?=^(?:cdef |cpdef |def ))',src,re.M|re.S).group()
function=function.replace('cdef void','def',1)
def enc(v):
 if isinstance(v,chess):return {'piece':{k:enc(getattr(v,k)) for k in ('index','row','col','back','live','color','value','x','y','possible_move')}}
 if isinstance(v,(list,tuple)):return [enc(x) for x in v]
 return v
rng=random.Random(721);cases=[]
for scenario in range(100):
 org=(rng.randrange(4),rng.randrange(8));dest=(rng.randrange(4),rng.randrange(8))
 entries=[[*dest,15,0]]
 m,ch=o.configure(entries,0,1)
 bans=[];orgs=[];dests=[]
 for _ in range(4):
  d=[(rng.randrange(4),rng.randrange(8)) for _ in range(4)]
  p=[(rng.randrange(4),rng.randrange(8)) for _ in range(2)]
  dests.append(d);orgs.append(p);bans.append(p[1])
 history=[None]*4;index=rng.randrange(4)
 namespace=dict(collect_possible_move=o.collect_possible_move,move_step=history,sindex=index,break_long_capture_dest=dests,break_long_capture_org=orgs,com_ban_step=bans,com_color=1)
 exec(function,namespace)
 before=enc([history,index,dests,orgs,bans])
 namespace['save_step_and_break_long_capture'](org,dest,m,ch)
 after=enc([namespace[k] for k in ('move_step','sindex','break_long_capture_dest','break_long_capture_org','com_ban_step')])
 cases.append(dict(method='save_step_and_break_long_capture',args=enc([org,dest,m,ch]),entries=entries,pc=0,cc=1,history=before,expected_history=after,expected=None))
f=ROOT/'build/unity-oracle/oracle.json';root=json.loads(f.read_text());root['cases']+=cases;f.write_text(json.dumps(root,separators=(',',':')))
(ROOT/'build/unity-oracle/history.json').write_text(json.dumps(dict(cases=cases)))
print('Long-capture history cases:',len(cases))
