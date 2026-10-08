"""Development-time Cython-to-C# mechanical port. Never used by Unity at runtime."""
import ast
import collections
import hashlib
import json
from pathlib import Path
import re

ROOT = Path(__file__).resolve().parents[2]
source = (ROOT/'darkchess.pyx').read_text(encoding='utf-8-sig')
core = source[source.index('cdef int can_be_ate_equal'):source.index('cdef void display_font')]
# Preserve calculation/control flow; presentation is handled by Unity.
core = re.sub(r'^c(?:p)?def\s+(?:\([^\n]*?\)|\w+)\s+(\w+)\(', r'def \1(', core, flags=re.M)
core = re.sub(r'\((?:int *,?\s*)+\)(\w+)', r'\1', core)
core = re.sub(r'\b(?:int|double|list|bint)\s+(?=\w+)', '', core)
core = re.sub(r'^(\s*)cdef\s+', r'\1', core, flags=re.M)
# Zero-initialized C declarations (none currently without an initializer).
tree = ast.parse(core)
print(collections.Counter(type(n).__name__ for n in ast.walk(tree)))
(ROOT/'build/unity-oracle').mkdir(parents=True,exist_ok=True)
(ROOT/'build/unity-oracle/normalized_core.py.txt').write_text(core)
# Replace only OS process orchestration with isolated C# candidate workers.
start = core.index('    if len(m) > 1:', core.index('def com_think'))
end = core.index('    elif 1 == len(m):', start)
core = core[:start] + '''    if len(m) > 1:
        mf = parallel_search(m, a_map, a_ch, alpha, beta)
        min_index = None
        for result in mf:
            if result[0] == result[1]:
                open_score = result[2]
            if final_score > result[2]:
                final_score = result[2]
                min_index = mf.index(result)
        if mf:
            return mf[min_index][0], mf[min_index][1], mf[min_index][2]
        else:
            return org, dest, min_score
''' + core[end:]
tree = ast.parse(core)
headers = {m.group(2): m.group(1) for m in re.finditer(r'^c(?:p)?def\s+(int|double|bint|void|list)\s+(\w+)\(', source, re.M)}
functions = {n.name: n for n in tree.body if isinstance(n, ast.FunctionDef)}

class Emitter:
    def __init__(self): self.serial = 0; self.lines = []; self.indent = 2; self.typed = {}; self.ret = None; self.current = None
    def w(self, text): self.lines.append('    '*self.indent + text)
    def name(self, s): return '@'+s
    def temp(self): self.serial += 1; return '__t'+str(self.serial)
    def expr(self,n):
        if isinstance(n, ast.Name): return self.name(n.id)
        if isinstance(n, ast.Constant):
            if n.value is None: return 'null'
            if isinstance(n.value,bool): return str(n.value).lower()
            return json.dumps(n.value,ensure_ascii=False)
        if isinstance(n, (ast.List,ast.Tuple)): return 'P.L('+', '.join(self.expr(x) for x in n.elts)+')'
        if isinstance(n, ast.Attribute): return self.expr(n.value)+'.'+self.name(n.attr)
        if isinstance(n, ast.Subscript):
            if isinstance(n.slice,ast.Slice): assert n.slice.lower is None and n.slice.upper is None; return 'P.CopyList('+self.expr(n.value)+')'
            return 'P.Get('+self.expr(n.value)+', '+self.expr(n.slice)+')'
        if isinstance(n, ast.BinOp):
            # The source's C double expressions contract multiply + add/sub on arm64.
            # Python object arithmetic remains separately rounded.
            if self.current == 'f_calc_move_score' and isinstance(n.op,ast.Sub) and isinstance(n.right,ast.BinOp) and isinstance(n.right.op,ast.Mult):
                return 'SourceFloat.Fma(P.Neg('+self.expr(n.right.left)+'), '+self.expr(n.right.right)+', '+self.expr(n.left)+')'
            if self.current == 'one_turn' and isinstance(n.op,(ast.Add,ast.Sub)) and isinstance(n.right,ast.BinOp) and isinstance(n.right.op,ast.Mult):
                a=self.expr(n.right.left)
                if isinstance(n.op,ast.Sub):a='P.Neg('+a+')'
                return 'SourceFloat.Fma('+a+', '+self.expr(n.right.right)+', '+self.expr(n.left)+')'
            op = {ast.Add:'Add',ast.Sub:'Sub',ast.Mult:'Mul',ast.Div:'Div',ast.Mod:'Mod'}[type(n.op)]
            return 'P.'+op+'('+self.expr(n.left)+', '+self.expr(n.right)+')'
        if isinstance(n, ast.UnaryOp):
            if isinstance(n.op,ast.USub): return 'P.Neg('+self.expr(n.operand)+')'
            raise ValueError(ast.dump(n))
        if isinstance(n, ast.BoolOp):
            return '('+(' && ' if isinstance(n.op,ast.And) else ' || ').join('P.Truth('+self.expr(v)+')' for v in n.values)+')'
        if isinstance(n, ast.Compare):
            parts=[]; left=n.left
            for op,right in zip(n.ops,n.comparators):
                a,b=self.expr(left),self.expr(right)
                if isinstance(op,(ast.Eq,ast.NotEq,ast.IsNot)): term=('!' if not isinstance(op,ast.Eq) else '')+'P.Equal('+a+', '+b+')'
                elif isinstance(op,ast.In): term='P.Contains('+b+', '+a+')'
                else: term='(P.Number('+a+') '+{ast.Lt:'<',ast.Gt:'>',ast.LtE:'<=',ast.GtE:'>='}[type(op)]+' P.Number('+b+'))'
                parts.append(term); left=right
            return '('+' && '.join(parts)+')'
        if isinstance(n, ast.Lambda):
            return 'new Func<dynamic, dynamic>(('+', '.join(self.name(a.arg) for a in n.args.args)+') => '+self.expr(n.body)+')'
        if isinstance(n, ast.Call):
            fname=ast.unparse(n.func)
            args=[self.expr(x) for x in n.args]
            if fname in ('min','max'):
                key=next((self.expr(k.value) for k in n.keywords if k.arg=='key'),None)
                return 'P.Extreme('+args[0]+', '+('true' if fname=='max' else 'false')+', '+(key or 'null')+')'
            mapped={'range':'P.Range','len':'P.Len','enumerate':'P.Enumerate','list':'P.CopyList','abs':'P.Abs',
                    'copy.deepcopy':'P.DeepCopy','random.randint':'random.RandInt',
                    'chess':'NewPiece','color_value_to_index':'ColorValueToIndex',
                    'index_to_color':'IndexToColor','index_to_chess_value':'IndexToValue',
                    'pygame.mixer.Sound':'NewSound','parallel_search':'ParallelSearch'}
            call=mapped.get(fname,self.expr(n.func))
            args += [self.name(k.arg)+': '+self.expr(k.value) for k in n.keywords]
            return call+'('+', '.join(args)+')'
        raise ValueError(ast.dump(n))
    def cast(self, name, expression):
        typ=self.typed.get(name)
        return ('P.Int('+expression+')' if typ=='int' else 'P.Number('+expression+')' if typ=='double' else expression)
    def assign(self,n,value):
        if isinstance(n,ast.Name): self.w(self.name(n.id)+' = '+self.cast(n.id,value)+';')
        elif isinstance(n,ast.Attribute): self.w(self.expr(n)+' = '+value+';')
        elif isinstance(n,ast.Subscript):
            if isinstance(n.slice,ast.Slice): self.w('P.Replace('+self.expr(n.value)+', '+value+');')
            else:self.w('P.Set('+self.expr(n.value)+', '+self.expr(n.slice)+', '+value+');')
        elif isinstance(n,(ast.Tuple,ast.List)):
            temp=self.temp(); self.w('dynamic '+temp+' = '+value+';')
            for i,el in enumerate(n.elts): self.assign(el,'P.Get('+temp+', '+str(i)+')')
        else: raise ValueError(ast.dump(n))
    def block(self,nodes):
        self.w('{'); self.indent+=1
        for n in nodes:self.stmt(n)
        self.indent-=1; self.w('}')
    def stmt(self,n):
        if isinstance(n,ast.Assign):
            value=self.expr(n.value)
            for target in n.targets:self.assign(target,value)
        elif isinstance(n,ast.AugAssign):
            op={ast.Add:'Add',ast.Sub:'Sub',ast.Mod:'Mod',ast.Mult:'Mul'}[type(n.op)]
            self.assign(n.target,'P.'+op+'('+self.expr(n.target)+', '+self.expr(n.value)+')')
        elif isinstance(n,ast.If):
            self.w('if (P.Truth('+self.expr(n.test)+'))');self.block(n.body)
            if n.orelse:self.w('else');self.block(n.orelse)
        elif isinstance(n,ast.For):
            if ast.unparse(n.iter).startswith('pygame.event.get'): return
            t=self.temp(); self.w('foreach (dynamic '+t+' in P.Iter('+self.expr(n.iter)+'))');self.w('{');self.indent+=1
            self.assign(n.target,t)
            for child in n.body:self.stmt(child)
            self.indent-=1;self.w('}')
            assert not n.orelse
        elif isinstance(n,ast.While):
            self.w('while (P.Truth('+self.expr(n.test)+'))');self.block(n.body);assert not n.orelse
        elif isinstance(n,ast.Return):
            val=self.expr(n.value) if n.value else 'null'
            if self.ret in ('int','bint'):val='P.Int('+val+')' if n.value else '0'
            if self.ret=='double':val='P.Number('+val+')' if n.value else '0.0'
            self.w('return '+val+';')
        elif isinstance(n,ast.Expr):
            text=ast.unparse(n.value)
            if text.startswith(('pygame.display.update','print(')):return
            self.w(self.expr(n.value)+';')
        elif isinstance(n,ast.Delete):
            for target in n.targets: self.w('P.Delete('+self.expr(target.value)+', '+self.expr(target.slice)+');')
        elif isinstance(n,ast.Break):self.w('break;')
        elif isinstance(n,ast.Continue):self.w('continue;')
        elif isinstance(n,(ast.Pass,ast.Global)): pass
        else:raise ValueError(ast.dump(n))
    def function(self,n):
        if n.name=='mouse_position_to_block':return
        globals_={x for node in ast.walk(n) if isinstance(node,ast.Global) for x in node.names}
        params={p.arg for p in n.args.args}
        stores={x.id for x in ast.walk(n) if isinstance(x,ast.Name) and isinstance(x.ctx,ast.Store)}
        original=re.search(r'^(?:cdef|cpdef|def) [^\n]*\b'+n.name+r'\(.*?(?=^(?:cdef|cpdef|def) |\Z)',source,re.M|re.S).group()
        self.typed={name:typ for typ,name in re.findall(r'\b(int|double)\s+(\w+)',original.split(':',1)[0])}
        self.typed.update({name:typ for typ,name in re.findall(r'\bcdef\s+(int|double)\s+(\w+)',original)})
        global_types={'turn_id':'int','first':'int','player_color':'int','com_color':'int','player_first':'int','player_win':'int','back_num':'int','step':'int','sindex':'int','max_value':'double','max_dist':'int','final_score':'double','AI_min_score':'double'}
        self.typed.update({k:v for k,v in global_types.items() if k in globals_})
        self.ret=headers.get(n.name)
        self.current=n.name
        defaults=[None]*(len(n.args.args)-len(n.args.defaults))+n.args.defaults
        signature=[]
        for arg,default in zip(n.args.args,defaults):signature.append(('int ' if default is not None and not (isinstance(default,ast.Constant) and default.value is None) else 'dynamic ')+self.name(arg.arg)+(' = '+(self.expr(default) if isinstance(default,ast.Constant) else ast.unparse(default)) if default else ''))
        self.w('// Original darkchess.pyx: '+str(source[:source.index(original)].count('\n')+1))
        self.w('public dynamic '+self.name(n.name)+'('+', '.join(signature)+')');self.w('{');self.indent+=1
        for name in sorted(stores-params-globals_): self.w('dynamic '+self.name(name)+' = null;')
        for name in params:
            if name in self.typed:self.w(self.name(name)+' = '+self.cast(name,self.name(name))+';')
        for child in n.body:self.stmt(child)
        self.w('return '+('0' if self.ret in ('int','bint','double') else 'null')+';')
        self.indent-=1;self.w('}')

em=Emitter()
for n in tree.body:
    if isinstance(n,ast.FunctionDef):em.function(n)
output='''#pragma warning disable 0162, 0219
// Generated mechanical C# port of original Cython calculations. Regenerate with tools/unity/port_core.py.
// Only presentation and candidate orchestration are provided by Unity adapters.
using System;
namespace DarkChessUnity
{
    public partial class OriginalEngine
    {
'''+ '\n'.join(em.lines)+'\n    }\n}\n'
(ROOT/'UnityDarkChess/Assets/Scripts/Core/OriginalEngine.Generated.cs').write_text(output)
print('Generated',len(em.lines),'lines /',len(functions)-1,'functions')
