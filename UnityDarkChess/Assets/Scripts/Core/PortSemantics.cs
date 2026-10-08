using System;
using System.Collections;
using System.Collections.Generic;

namespace DarkChessUnity
{
    // Native C# value containers preserve source list ordering and coordinate equality.
    public sealed class PList : IEnumerable
    {
        public readonly List<object> Items = new List<object>();
        public void append(object value) { Items.Add(value); }
        public void extend(object values) { foreach (object x in P.Iter(values)) Items.Add(x); }
        public int index(object value) { for (int i = 0; i < Items.Count; i++) if (P.Equal(Items[i], value)) return i; throw new ArgumentException("Value absent from list"); }
        public IEnumerator GetEnumerator() { return Items.GetEnumerator(); }
    }

    public static class P
    {
        public static object L(params object[] values) { var list = new PList(); list.Items.AddRange(values); return list; }
        public static int Int(object value) { if (value == null) return 0; return (int)Number(value); }
        public static double Number(object value) { if (value == null) throw new InvalidOperationException("None used as a number"); return Convert.ToDouble(value); }
        public static bool Truth(object value)
        {
            if (value == null) return false;
            if (value is PList list) return list.Items.Count != 0;
            if (value is bool b) return b;
            if (value is string s) return s.Length != 0;
            return Number(value) != 0;
        }
        public static bool Equal(object a, object b)
        {
            if (a == null || b == null) return a == b;
            if (a is PList x && b is PList y)
            {
                if (x.Items.Count != y.Items.Count) return false;
                for (int i = 0; i < x.Items.Count; i++) if (!Equal(x.Items[i], y.Items[i])) return false;
                return true;
            }
            if (a is string || b is string) return a.Equals(b);
            if (a is Piece || b is Piece) return ReferenceEquals(a, b);
            return Number(a) == Number(b);
        }
        public static bool Contains(object list, object value) { foreach (object item in Iter(list)) if (Equal(item, value)) return true; return false; }
        public static int Len(object list) { return ((PList)list).Items.Count; }
        public static IEnumerable Iter(object list) { return (IEnumerable)list; }
        public static object Get(object list, object index)
        {
            var items = ((PList)list).Items;
            int i = Int(index); if (i < 0) i += items.Count;
            return items[i];
        }
        public static void Set(object list, object index, object value)
        {
            var items = ((PList)list).Items;
            int i = Int(index); if (i < 0) i += items.Count;
            items[i] = value;
        }
        public static void Delete(object list, object index) { ((PList)list).Items.RemoveAt(Int(index)); }
        public static object CopyList(object value) { var list = new PList(); list.extend(value); return list; }
        public static void Replace(object list, object value) { ((PList)list).Items.Clear(); ((PList)list).extend(value); }
        public static object DeepCopy(object value) { return Copy(value, new Dictionary<object, object>()); }
        static object Copy(object value, Dictionary<object, object> memo)
        {
            if (value == null || value is string || value.GetType().IsValueType) return value;
            if (memo.TryGetValue(value, out object previous)) return previous;
            if (value is PList source)
            {
                var result = new PList(); memo[value] = result;
                foreach (object v in source.Items) result.Items.Add(Copy(v, memo));
                return result;
            }
            if (value is Piece p)
            {
                var q = (Piece)p.MemberClone(); memo[value] = q;
                q.possible_move = Copy(p.possible_move, memo);
                return q;
            }
            throw new ArgumentException("Unsupported copy: " + value.GetType());
        }
        static bool Integral(object a) { return a is int || a is long || a is bool; }
        public static object Add(object a, object b)
        {
            if (a is PList && b is PList) { var result = (PList)CopyList(a); result.extend(b); return result; }
            if (Integral(a) && Integral(b)) return Int(a) + Int(b);
            return Number(a) + Number(b);
        }
        public static object Sub(object a, object b) { if (Integral(a) && Integral(b)) return Int(a) - Int(b); return Number(a) - Number(b); }
        public static object Mul(object a, object b)
        {
            if (a is PList || b is PList)
            {
                var list = a as PList ?? (PList)b;
                int count = Int(a is PList ? b : a); var result = new PList();
                for (int i = 0; i < count; i++) result.extend(list);
                return result;
            }
            if (Integral(a) && Integral(b)) return Int(a) * Int(b);
            return Number(a) * Number(b);
        }
        public static object Div(object a, object b) { return Number(a) / Number(b); }
        public static object Mod(object a, object b) { int x = Int(a), y = Int(b); return (x % y + y) % y; }
        public static object Neg(object a) { if (Integral(a)) return -Int(a); return -Number(a); }
        public static object Abs(object a) { if (Integral(a)) return Math.Abs(Int(a)); return Math.Abs(Number(a)); }
        public static object Range(object stop) { return Range(0, stop, 1); }
        public static object Range(object start, object stop) { return Range(start, stop, 1); }
        public static object Range(object start, object stop, object step)
        {
            int a = Int(start), b = Int(stop), s = Int(step);
            if (s == 0) throw new ArgumentException("Zero range step");
            var result = new PList();
            for (int i = a; s > 0 ? i < b : i > b; i += s) result.append(i);
            return result;
        }
        public static object Enumerate(object values)
        {
            int i = 0; var result = new PList();
            foreach (object v in Iter(values)) result.append(L(i++, v));
            return result;
        }
        public static object Extreme(object values, bool maximum, Func<object, object> key)
        {
            object result = null; double score = 0; bool first = true;
            foreach (object v in Iter(values))
            {
                double next = Number(key == null ? v : key(v));
                if (first || (maximum ? next > score : next < score)) { result = v; score = next; first = false; }
            }
            if (first) throw new ArgumentException("Empty min/max");
            return result;
        }
    }

    [Serializable]
    public class Piece
    {
        public int index, color, value, row, col, x, y, back = 1, live = 1;
        public object possible_move = P.L();
        public object MemberClone() { return MemberwiseClone(); }
    }

    // CPython Random's MT19937 integer seeding/getrandbits/randbelow algorithm.
    public sealed class SourceRandom
    {
        readonly uint[] state = new uint[624]; int position = 624;
        public SourceRandom() : this((uint)Guid.NewGuid().GetHashCode()) { }
        public SourceRandom(uint seed)
        {
            unchecked
            {
                state[0] = 19650218;
                for (uint k = 1; k < 624; k++) state[k] = 1812433253U * (state[k - 1] ^ (state[k - 1] >> 30)) + k;
                int i = 1;
                for (int k = 624; k > 0; k--)
                {
                    state[i] = (state[i] ^ ((state[i - 1] ^ (state[i - 1] >> 30)) * 1664525U)) + seed;
                    if (++i >= 624) { state[0] = state[623]; i = 1; }
                }
                for (int k = 623; k > 0; k--)
                {
                    state[i] = (state[i] ^ ((state[i - 1] ^ (state[i - 1] >> 30)) * 1566083941U)) - (uint)i;
                    if (++i >= 624) { state[0] = state[623]; i = 1; }
                }
                state[0] = 0x80000000;
            }
        }
        uint NextUInt()
        {
            if (position >= 624)
            {
                for (int i = 0; i < 624; i++)
                {
                    uint y = (state[i] & 0x80000000) | (state[(i + 1) % 624] & 0x7fffffff);
                    state[i] = state[(i + 397) % 624] ^ (y >> 1) ^ ((y & 1) == 0 ? 0U : 0x9908b0dfU);
                }
                position = 0;
            }
            uint v = state[position++]; v ^= v >> 11; v ^= (v << 7) & 0x9d2c5680U; v ^= (v << 15) & 0xefc60000U; return v ^ (v >> 18);
        }
        public int RandInt(object start, object end)
        {
            int a = P.Int(start), n = P.Int(end) - a + 1;
            if (n <= 0) throw new ArgumentException("Empty randint range");
            int bits = 0; for (int v = n; v != 0; v >>= 1) bits++;
            uint result; do { result = NextUInt() >> (32 - bits); } while (result >= n);
            return a + (int)result;
        }
    }
}
