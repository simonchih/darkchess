using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;
using DarkChessUnity;
class OracleRunner
{
    static dynamic ConvertValue(object value)
    {
        if(value is IDictionary<string,object> dict)
        {
            if(dict.ContainsKey("float64"))
            {
                string hex=(string)dict["float64"];byte[] bytes=new byte[8];
                for(int i=0;i<8;i++)bytes[i]=System.Convert.ToByte(hex.Substring(2*i,2),16);
                return BitConverter.ToDouble(bytes,0);
            }
            var data=(IDictionary<string,object>)dict["piece"];
            Piece p=(Piece)OriginalEngine.NewPiece(data["index"],P.L(data["row"],data["col"]));
            p.back=P.Int(data["back"]);p.live=P.Int(data["live"]);p.x=P.Int(data["x"]);p.y=P.Int(data["y"]);
            p.possible_move=ConvertValue(data["possible_move"]);return p;
        }
        if(value is IList items){var list=new PList();foreach(object item in items)list.append(ConvertValue(item));return list;}
        if(value is decimal)return (double)(decimal)value;
        return value;
    }
    static void Configure(OriginalEngine e,object entries,int pc,int cc)
    {
        e.main_chess=OriginalEngine.Grid(null);e.main_map=OriginalEngine.Grid(null);
        e.player_color=pc;e.com_color=cc;e.back_num=0;
        e.back_value_num=P.L(P.L(0,0,0,0,0,0,0,0),P.L(0,0,0,0,0,0,0,0));
        e.king_live=P.L(0,0);
        for(int r=0;r<4;r++)for(int c=0;c<8;c++) {Piece p=(Piece)OriginalEngine.NewPiece(32,P.L(r,c));p.live=0;P.Set(P.Get(e.main_chess,r),c,p);}
        foreach(dynamic row in P.Iter(ConvertValue(entries)))
        {
            int r=P.Int(P.Get(row,0)),c=P.Int(P.Get(row,1));
            Piece p=(Piece)OriginalEngine.NewPiece(P.Get(row,2),P.L(r,c));p.back=P.Int(P.Get(row,3));
            P.Set(P.Get(e.main_chess,r),c,p);P.Set(P.Get(e.main_map,r),c,P.L(r,c));
            if(p.back==1){e.back_num=P.Int(e.back_num)+1;dynamic counts=P.Get(e.back_value_num,p.color);P.Set(counts,p.value,P.Add(P.Get(counts,p.value),1));}
        }
        e.scan_king(e.main_chess);
    }
    static object Normalize(object v)
    {
        if(v is PList list){var result=new List<object>();foreach(object x in list.Items)result.Add(Normalize(x));return result;}
        return v;
    }
    static int Main(string[] args)
    {
        var json=new JavaScriptSerializer {MaxJsonLength=100000000};
        var root=(Dictionary<string,object>)json.DeserializeObject(File.ReadAllText(args[0]));
        int cases=0,failed=0;var methods=new Dictionary<string,int>();
        foreach(Dictionary<string,object> test in (object[])root["cases"])
        {
            string method=(string)test["method"];cases++;
            if(!methods.ContainsKey(method))methods[method]=0;methods[method]++;
            try
            {
                var engine=new OriginalEngine();
                if(test.ContainsKey("sequential"))engine.SequentialSearch=true;
                if(test.ContainsKey("entries")&&test["entries"]!=null) Configure(engine,test["entries"],P.Int(test["pc"]),P.Int(test["cc"]));
                if(test.ContainsKey("seed"))engine.random=new SourceRandom((uint)P.Int(test["seed"]));
                if(test.ContainsKey("history"))
                {
                    dynamic h=ConvertValue(test["history"]);
                    engine.move_step=P.Get(h,0);engine.sindex=P.Get(h,1);
                    engine.break_long_capture_dest=P.Get(h,2);engine.break_long_capture_org=P.Get(h,3);engine.com_ban_step=P.Get(h,4);
                }
                var supplied=(PList)ConvertValue(test["args"]);
                if(test.ContainsKey("entries")&&test["entries"]!=null)
                {
                    // configure() returns the very same board/map held in source globals.
                    // Preserve that alias when deserializing function arguments.
                    foreach(object argument in supplied.Items)
                    {
                        if(!(argument is PList grid)||grid.Items.Count!=4||!(grid.Items[0] is PList row)||row.Items.Count!=8)continue;
                        if(row.Items[0] is Piece)engine.main_chess=grid;
                        else if(row.Items[0]==null||row.Items[0] is PList)engine.main_map=grid;
                    }
                }
                var parameters=new List<object>(supplied.Items);
                var q=new OriginalEngine.ResultQueue();if(method=="one_turn")parameters.Insert(0,q);
                MethodInfo info=typeof(OriginalEngine).GetMethod(method);
                ParameterInfo[] signature=info.GetParameters();
                while(parameters.Count<signature.Length)parameters.Add(signature[parameters.Count].DefaultValue);
                object actual=info.Invoke(engine,parameters.ToArray());if(method=="one_turn")actual=q.result;
                object expected=ConvertValue(test["expected"]);
                if(test.ContainsKey("expected_history"))
                {
                    actual=P.L(engine.move_step,engine.sindex,engine.break_long_capture_dest,engine.break_long_capture_org,engine.com_ban_step);
                    expected=ConvertValue(test["expected_history"]);
                }
                if(!P.Equal(actual,expected))
                {
                    failed++;Console.WriteLine("FAIL "+cases+" "+method+" expected "+json.Serialize(Normalize(expected))+" actual "+json.Serialize(Normalize(actual)));
                    if(failed>=15)break;
                }
            }
            catch(Exception error){failed++;Console.WriteLine("ERROR "+cases+" "+method+" "+error);if(failed>=15)break;}
        }
        string report=json.Serialize(new{success=failed==0,cases,failed,methods});Console.WriteLine(report);
        if(args.Length>1)File.WriteAllText(args[1],report);
        return failed==0?0:1;
    }
}
