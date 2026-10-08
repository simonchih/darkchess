using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
namespace DarkChessUnity
{
    public partial class OriginalEngine
    {
        public object player_win = 0, player_first = 0, first = 1, turn_id = 0;
        public object player_color = 0, com_color = 1, sindex = 0, step = 0, back_num = 32;
        public object AI_min_score = 9000.0, final_score = 9000.0, open_score = null;
        public object gb_m2 = P.L(), com_mv_map = P.L(0, 0);
        public object main_chess = Grid(0), server_main_chess = Grid(0), main_map = Grid(P.L(0, 0));
        public object back_value_num = P.L(P.L(0,0,0,0,0,0,0,0), P.L(0,0,0,0,0,0,0,0));
        public object king_live = P.L(1,1), chess_num = P.L(16,16);
        public object break_long_capture_dest = P.L(), break_long_capture_org = P.L(), com_ban_step = P.L();
        public object move_step = P.L(null,null,null,null);
        public object max_value = 0.0, max_dist = 32, mark = Grid(0), cannon_mark = Grid(0);
        public SourceRandom random = new SourceRandom();
        public Action<string> Sound;
        public int SearchWorkers;
        public bool SequentialSearch;
        public object clean_back_n1_to_0(object pieces)
        {
            foreach (object row in P.Iter(pieces)) foreach (Piece p in P.Iter(row)) if (p.back == -1) p.back = 0;
            return pieces;
        }
        public static object Grid(object initial)
        {
            var result = new PList();
            for (int r = 0; r < 4; r++) { var row = new PList(); for (int c = 0; c < 8; c++) row.append(initial); result.append(row); }
            return result;
        }
        public static int IndexToColor(object index) { int i = P.Int(index); return i >= 0 && i < 16 ? 0 : 1; }
        public static int IndexToValue(object index)
        {
            int i = P.Int(index); if (i >= 16 && i < 32) i -= 16;
            if (i < 5) return 1; if (i < 7) return 2; if (i < 9) return 3;
            if (i < 11) return 4; if (i < 13) return 5; if (i < 15) return 6; if (i == 15) return 7; return 0;
        }
        public static object NewPiece(object index, object rc)
        {
            int r = P.Int(P.Get(rc,0)), c = P.Int(P.Get(rc,1)), i = P.Int(index);
            return new Piece { index = i, row = r, col = c, color = IndexToColor(i), value = IndexToValue(i),
                x = c < 4 ? 34 + c * 57 : 260 + (c - 4) * 57, y = 51 + r * 57 };
        }
        public static object ColorValueToIndex(object color, object value, object counts)
        {
            int c = P.Int(color), v = P.Int(value), n = P.Int(P.Get(P.Get(counts,c),v));
            if (n == 0) return null;
            int[] offsets = {-1,-1,4,6,8,10,12,14};
            return offsets[v] + n + c * 16;
        }
        public sealed class DisplaySound
        {
            readonly Action<string> callback; readonly string name;
            public DisplaySound(Action<string> play, string name) { this.callback = play; this.name = name; }
            public void @play() { callback?.Invoke(name); }
        }
        public object NewSound(object name) { return new DisplaySound(Sound, (string)name); }
        readonly string s_click = "CLICK", s_move2 = "MOVE2", s_capture = "CAPTURE2";
        public sealed class ResultQueue
        {
            public object result;
            public void put(object value) { result = value; }
        }
        public object ParallelSearch(object candidates, object map, object pieces, object alpha, object beta)
        {
            int count = P.Len(candidates);
            var completed = new BlockingCollection<object>();
            var tasks = new Task[count];
            for (int i = 0; i < count; i++)
            {
                int index = i; object candidate = P.Get(candidates,index);
                Action work = () =>
                {
                    // Source spawn workers import a fresh module: all scratch globals start at defaults.
                    var worker = new OriginalEngine();
                    var queue = new ResultQueue();
                    worker.one_turn(queue, P.DeepCopy(map), P.DeepCopy(pieces), P.DeepCopy(candidate),
                        player_color, P.Get(candidate,0), P.Get(candidate,1), P.Get(candidate,2), P.Get(candidate,3),
                        0.90, index, alpha, beta, player_color, com_color, back_num,
                        P.DeepCopy(com_ban_step), P.DeepCopy(king_live), P.Mul(P.L((object)null), count));
                    object row = P.Get(queue.result,0);
                    completed.Add(P.L(P.Get(row,0),P.Get(row,1),P.Get(row,4)));
                };
                SearchWorkers++;
                if (SequentialSearch) { work(); tasks[i] = Task.CompletedTask; }
                else tasks[i] = Task.Run(work);
            }
            // Keep source Queue arrival order, including its tie behavior.
            Task.WaitAll(tasks);
            var results = new PList();
            for (int i = 0; i < count; i++) results.append(completed.Take());
            completed.Dispose();
            return results;
        }
    }
}
