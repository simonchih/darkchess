#pragma warning disable 0162, 0219
// Generated mechanical C# port of original Cython calculations. Regenerate with tools/unity/port_core.py.
// Only presentation and candidate orchestration are provided by Unity adapters.
using System;
namespace DarkChessUnity
{
    public partial class OriginalEngine
    {
        // Original darkchess.pyx: 131
        public object @can_be_ate_equal(object @small_value, object @big_value)
        {
            @small_value = P.Int(@small_value);
            @big_value = P.Int(@big_value);
            if (P.Truth((P.Equal(2, @big_value))))
            {
                return P.Int(0);
            }
            else
            {
                if (P.Truth((P.Truth((P.Equal(1, @big_value))) && P.Truth((P.Equal(7, @small_value))))))
                {
                    return P.Int(1);
                }
                else
                {
                    if (P.Truth((P.Truth((P.Equal(7, @big_value))) && P.Truth((P.Equal(1, @small_value))))))
                    {
                        return P.Int(0);
                    }
                    else
                    {
                        if (P.Truth(((P.Number(@big_value) > P.Number(@small_value)))))
                        {
                            return P.Int(1);
                        }
                        else
                        {
                            if (P.Truth((P.Equal(@big_value, @small_value))))
                            {
                                return P.Int(2);
                            }
                            else
                            {
                                return P.Int(0);
                            }
                        }
                    }
                }
            }
            return 0;
        }
        // Original darkchess.pyx: 145
        public object @can_be_ate(object @small_value, object @big_value)
        {
            @small_value = P.Int(@small_value);
            @big_value = P.Int(@big_value);
            if (P.Truth((P.Equal(2, @big_value))))
            {
                return P.Int(0);
            }
            else
            {
                if (P.Truth((P.Truth((P.Equal(1, @big_value))) && P.Truth((P.Equal(7, @small_value))))))
                {
                    return P.Int(1);
                }
                else
                {
                    if (P.Truth((P.Truth((P.Equal(7, @big_value))) && P.Truth((P.Equal(1, @small_value))))))
                    {
                        return P.Int(0);
                    }
                    else
                    {
                        if (P.Truth(((P.Number(@big_value) > P.Number(@small_value)))))
                        {
                            return P.Int(1);
                        }
                        else
                        {
                            return P.Int(0);
                        }
                    }
                }
            }
            return 0;
        }
        // Original darkchess.pyx: 157
        public object @ini_random_chess(object @list)
        {
            object @all_list = null;
            object @end = null;
            object @i = null;
            object @start = null;
            @all_list = P.Mul(P.L(0), 32);
            foreach (object __t1 in P.Iter(P.Range(31, P.Neg(1), P.Neg(1))))
            {
                @end = __t1;
                @start = random.RandInt(0, @end);
                @i = @start;
                while (P.Truth((!P.Equal(@i, P.Neg(1)))))
                {
                    if (P.Truth((P.Equal(0, P.Get(@all_list, @start)))))
                    {
                        if (P.Truth((!P.Equal(@i, 0))))
                        {
                            @start = P.Add(@start, 1);
                            @start = P.Mod(@start, 32);
                            @i = P.Sub(@i, 1);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        @start = P.Add(@start, 1);
                        @start = P.Mod(@start, 32);
                    }
                }
                P.Set(@all_list, @start, 1);
                P.Set(@list, P.Sub(31, @end), @start);
            }
            return @list;
            return null;
        }
        // Original darkchess.pyx: 184
        public object @all_chess_move(object @a_map, object @my_chess)
        {
            object @ch = null;
            object @chr = null;
            foreach (object __t2 in P.Iter(@my_chess))
            {
                @chr = __t2;
                foreach (object __t3 in P.Iter(@chr))
                {
                    @ch = __t3;
                    if (P.Truth((P.Truth(((P.Number(((Piece)@ch).@back) < P.Number(1)))) && P.Truth((P.Equal(1, ((Piece)@ch).@live))))))
                    {
                        ((Piece)@ch).@possible_move = @collect_possible_move(((Piece)@ch).@row, ((Piece)@ch).@col, @a_map, @my_chess);
                    }
                }
            }
            return null;
        }
        // Original darkchess.pyx: 190
        public object @collect_possible_move(object @i, object @j, object @a_map, object @my_chess)
        {
            object @ii = null;
            object @jj = null;
            object @jump = null;
            object @nc = null;
            object @ncor = null;
            object @pm = null;
            @i = P.Int(@i);
            @j = P.Int(@j);
            @pm = P.L();
            @ncor = @near(@i, @j);
            foreach (object __t4 in P.Iter(@ncor))
            {
                @nc = __t4;
                if (P.Truth((P.Equal(null, P.Get(P.Get(@a_map, P.Get(@nc, 0)), P.Get(@nc, 1))))))
                {
                    ((PList)@pm).@append(@nc);
                }
                else
                {
                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @j), null))))
                    {
                        if (P.Truth((P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, P.Get(@nc, 0)), P.Get(@nc, 1)), 0)), P.Get(P.Get(P.Get(@a_map, P.Get(@nc, 0)), P.Get(@nc, 1)), 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@color))) && P.Truth(((P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, P.Get(@nc, 0)), P.Get(@nc, 1)), 0)), P.Get(P.Get(P.Get(@a_map, P.Get(@nc, 0)), P.Get(@nc, 1)), 1))).@back) < P.Number(1)))))))
                        {
                            if (P.Truth((P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@value))) && P.Truth((P.Equal(7, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, P.Get(@nc, 0)), P.Get(@nc, 1)), 0)), P.Get(P.Get(P.Get(@a_map, P.Get(@nc, 0)), P.Get(@nc, 1)), 1))).@value))))))
                            {
                                ((PList)@pm).@append(@nc);
                            }
                            else
                            {
                                if (P.Truth((P.Truth((P.Equal(7, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@value))) && P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, P.Get(@nc, 0)), P.Get(@nc, 1)), 0)), P.Get(P.Get(P.Get(@a_map, P.Get(@nc, 0)), P.Get(@nc, 1)), 1))).@value))))))
                                {
                                }
                                else
                                {
                                    if (P.Truth((P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@value, 2))) && P.Truth(((P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@value) >= P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, P.Get(@nc, 0)), P.Get(@nc, 1)), 0)), P.Get(P.Get(P.Get(@a_map, P.Get(@nc, 0)), P.Get(@nc, 1)), 1))).@value)))))))
                                    {
                                        ((PList)@pm).@append(@nc);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @j), null))))
            {
                if (P.Truth((P.Equal(2, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@value))))
                {
                    @jump = 0;
                    foreach (object __t5 in P.Iter(P.Range(P.Sub(@i, 1), P.Neg(1), P.Neg(1))))
                    {
                        @ii = __t5;
                        if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))))
                        {
                            if (P.Truth((P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@back))) || P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@color))))))
                            {
                                break;
                            }
                            else
                            {
                                ((PList)@pm).@append(P.L(@ii, @j));
                                break;
                            }
                        }
                        if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))
                        {
                            @jump = 1;
                        }
                    }
                    @jump = 0;
                    foreach (object __t6 in P.Iter(P.Range(P.Add(@i, 1), 4, 1)))
                    {
                        @ii = __t6;
                        if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))))
                        {
                            if (P.Truth((P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@back))) || P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@color))))))
                            {
                                break;
                            }
                            else
                            {
                                ((PList)@pm).@append(P.L(@ii, @j));
                                break;
                            }
                        }
                        if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))
                        {
                            @jump = 1;
                        }
                    }
                    @jump = 0;
                    foreach (object __t7 in P.Iter(P.Range(P.Sub(@j, 1), P.Neg(1), P.Neg(1))))
                    {
                        @jj = __t7;
                        if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))))
                        {
                            if (P.Truth((P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@back))) || P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@color))))))
                            {
                                break;
                            }
                            else
                            {
                                ((PList)@pm).@append(P.L(@i, @jj));
                                break;
                            }
                        }
                        if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))
                        {
                            @jump = 1;
                        }
                    }
                    @jump = 0;
                    foreach (object __t8 in P.Iter(P.Range(P.Add(@j, 1), 8, 1)))
                    {
                        @jj = __t8;
                        if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))))
                        {
                            if (P.Truth((P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@back))) || P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@color))))))
                            {
                                break;
                            }
                            else
                            {
                                ((PList)@pm).@append(P.L(@i, @jj));
                                break;
                            }
                        }
                        if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))
                        {
                            @jump = 1;
                        }
                    }
                }
            }
            return @pm;
            return null;
        }
        // Original darkchess.pyx: 285
        public object @eat_by_bomb(object @org, object @a_map, object @my_chess)
        {
            object @i = null;
            object @ii = null;
            object @j = null;
            object @jj = null;
            object @jump = null;
            object @was_ate = null;
            object __t9 = @org;
            @i = P.Get(__t9, 0);
            @j = P.Get(__t9, 1);
            @jump = P.Int(0);
            @was_ate = P.Int(0);
            foreach (object __t10 in P.Iter(P.Range(P.Sub(@i, 1), P.Neg(1), P.Neg(1))))
            {
                @ii = __t10;
                if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))))
                {
                    if (P.Truth((P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@back))) || P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@color))))))
                    {
                        break;
                    }
                    else
                    {
                        if (P.Truth((P.Equal(2, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@value))))
                        {
                            @was_ate = P.Int(1);
                            break;
                        }
                    }
                }
                else
                {
                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))
                    {
                        @jump = P.Int(1);
                    }
                }
            }
            @jump = P.Int(0);
            foreach (object __t11 in P.Iter(P.Range(P.Add(@i, 1), 4, 1)))
            {
                @ii = __t11;
                if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))))
                {
                    if (P.Truth((P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@back))) || P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@color))))))
                    {
                        break;
                    }
                    else
                    {
                        if (P.Truth((P.Equal(2, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@value))))
                        {
                            @was_ate = P.Int(1);
                            break;
                        }
                    }
                }
                else
                {
                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))
                    {
                        @jump = P.Int(1);
                    }
                }
            }
            @jump = P.Int(0);
            foreach (object __t12 in P.Iter(P.Range(P.Sub(@j, 1), P.Neg(1), P.Neg(1))))
            {
                @jj = __t12;
                if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))))
                {
                    if (P.Truth((P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@back))) || P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@color))))))
                    {
                        break;
                    }
                    else
                    {
                        if (P.Truth((P.Equal(2, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@value))))
                        {
                            @was_ate = P.Int(1);
                            break;
                        }
                    }
                }
                else
                {
                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))
                    {
                        @jump = P.Int(1);
                    }
                }
            }
            @jump = P.Int(0);
            foreach (object __t13 in P.Iter(P.Range(P.Add(@j, 1), 8, 1)))
            {
                @jj = __t13;
                if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))))
                {
                    if (P.Truth((P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@back))) || P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@color))))))
                    {
                        break;
                    }
                    else
                    {
                        if (P.Truth((P.Equal(2, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@value))))
                        {
                            @was_ate = P.Int(1);
                            break;
                        }
                    }
                }
                else
                {
                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))
                    {
                        @jump = P.Int(1);
                    }
                }
            }
            return P.Int(@was_ate);
            return 0;
        }
        // Original darkchess.pyx: 331
        public object @check_eat_number(object @a_map, object @my_chess, object @n_min, object @n_max, object @no_min, object @no_max, object @y, object @x)
        {
            object @c = null;
            object @eat_possible_num = null;
            object @num = null;
            object @v = null;
            object @val = null;
            object @was_ate_num = null;
            @no_max = P.Int(@no_max);
            @n_min = P.Int(@n_min);
            @y = P.Int(@y);
            @x = P.Int(@x);
            @no_min = P.Int(@no_min);
            @n_max = P.Int(@n_max);
            @eat_possible_num = P.Int(0);
            @was_ate_num = P.Int(0);
            foreach (object __t14 in P.Iter(P.Enumerate(@back_value_num)))
            {
                object __t15 = __t14;
                @c = P.Get(__t15, 0);
                @val = P.Get(__t15, 1);
                foreach (object __t16 in P.Iter(P.Enumerate(@val)))
                {
                    object __t17 = __t16;
                    @v = P.Get(__t17, 0);
                    @num = P.Get(__t17, 1);
                    if (P.Truth((P.Truth((P.Equal(0, @num))) || P.Truth((P.Equal(0, @v))))))
                    {
                        continue;
                    }
                    if (P.Truth((P.Equal(@com_color, @c))))
                    {
                        if (P.Truth((P.Truth((P.Equal(2, @v))) && P.Truth(((P.Number(@n_max) < P.Number(3)))))))
                        {
                            if (P.Truth(((P.Number(0) < P.Number(@if_cannon_can_eat(P.L(@y, @x), @a_map, @my_chess, @com_color))))))
                            {
                                @eat_possible_num = P.Int(P.Add(@eat_possible_num, @num));
                            }
                        }
                        else
                        {
                            if (P.Truth((P.Equal(0, @n_max))))
                            {
                                continue;
                            }
                            else
                            {
                                if (P.Truth((P.Truth((P.Equal(1, @n_max))) && P.Truth((P.Equal(7, @v))))))
                                {
                                    @was_ate_num = P.Int(P.Add(@was_ate_num, @num));
                                }
                                else
                                {
                                    if (P.Truth((P.Truth((P.Equal(7, @n_max))) && P.Truth((P.Equal(1, @v))))))
                                    {
                                        if (P.Truth((P.Truth((P.Equal(7, @n_min))) || P.Truth((P.Equal(2, @n_min))))))
                                        {
                                            @eat_possible_num = P.Int(P.Add(@eat_possible_num, @num));
                                        }
                                        else
                                        {
                                            @was_ate_num = P.Int(P.Add(@was_ate_num, @num));
                                        }
                                    }
                                    else
                                    {
                                        if (P.Truth(((P.Number(@v) > P.Number(@n_max)))))
                                        {
                                            @eat_possible_num = P.Int(P.Add(@eat_possible_num, @num));
                                        }
                                        else
                                        {
                                            if (P.Truth((P.Truth((!P.Equal(@v, 1))) || P.Truth((!P.Equal(@n_max, 2))) || P.Truth((!P.Equal(@n_min, 2))))))
                                            {
                                                @was_ate_num = P.Int(P.Add(@was_ate_num, @num));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (P.Truth((P.Equal(2, @v))))
                        {
                            if (P.Truth(((P.Number(0) < P.Number(@if_cannon_can_eat(P.L(@y, @x), @a_map, @my_chess, @player_color))))))
                            {
                                @was_ate_num = P.Int(P.Add(@was_ate_num, @num));
                            }
                            else
                            {
                                if (P.Truth((P.Truth(((P.Number(@no_max) >= P.Number(3)))) && P.Truth(((P.Number(@no_max) > P.Number(@n_max)))))))
                                {
                                    @eat_possible_num = P.Int(P.Add(@eat_possible_num, @num));
                                }
                            }
                        }
                        else
                        {
                            if (P.Truth((P.Equal(8, @no_min))))
                            {
                                continue;
                            }
                            else
                            {
                                if (P.Truth((P.Truth((P.Equal(1, @no_min))) && P.Truth((P.Equal(7, @v))))))
                                {
                                    if (P.Truth((P.Equal(1, @no_max))))
                                    {
                                        @eat_possible_num = P.Int(P.Add(@eat_possible_num, @num));
                                    }
                                    else
                                    {
                                        @was_ate_num = P.Int(P.Add(@was_ate_num, @num));
                                    }
                                }
                                else
                                {
                                    if (P.Truth((P.Truth((P.Equal(7, @no_max))) && P.Truth((P.Equal(1, @v))))))
                                    {
                                        @was_ate_num = P.Int(P.Add(@was_ate_num, @num));
                                    }
                                    else
                                    {
                                        if (P.Truth(((P.Number(@v) >= P.Number(@no_min)))))
                                        {
                                            @was_ate_num = P.Int(P.Add(@was_ate_num, @num));
                                        }
                                        else
                                        {
                                            if (P.Truth((P.Truth((!P.Equal(@v, 1))) || P.Truth((!P.Equal(@no_max, 2))) || P.Truth((!P.Equal(@no_min, 2))))))
                                            {
                                                if (P.Truth(((P.Number(@no_max) > P.Number(@n_max)))))
                                                {
                                                    @eat_possible_num = P.Int(P.Add(@eat_possible_num, @num));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return P.Int(P.Sub(@eat_possible_num, @was_ate_num));
            return 0;
        }
        // Original darkchess.pyx: 385
        public object @if_cannon_can_eat(object @org, object @a_map, object @my_chess, object @owner_color)
        {
            object @eat_number = null;
            object @i = null;
            object @ii = null;
            object @j = null;
            object @jj = null;
            object @jump = null;
            object @opp_color = null;
            @owner_color = P.Int(@owner_color);
            object __t18 = @org;
            @i = P.Get(__t18, 0);
            @j = P.Get(__t18, 1);
            @jump = P.Int(0);
            @eat_number = P.Int(0);
            @opp_color = P.Int(P.Sub(1, @owner_color));
            foreach (object __t19 in P.Iter(P.Range(P.Sub(@i, 1), P.Neg(1), P.Neg(1))))
            {
                @ii = __t19;
                if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))))
                {
                    if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@back))))
                    {
                        break;
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@live))) && P.Truth((P.Equal(@opp_color, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@color))))))
                        {
                            @eat_number = P.Int(P.Add(@eat_number, 1));
                            break;
                        }
                    }
                }
                else
                {
                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))
                    {
                        @jump = P.Int(1);
                    }
                }
            }
            @jump = P.Int(0);
            foreach (object __t20 in P.Iter(P.Range(P.Add(@i, 1), 4, 1)))
            {
                @ii = __t20;
                if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))))
                {
                    if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@back))))
                    {
                        break;
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@live))) && P.Truth((P.Equal(@opp_color, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@color))))))
                        {
                            @eat_number = P.Int(P.Add(@eat_number, 1));
                            break;
                        }
                    }
                }
                else
                {
                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))
                    {
                        @jump = P.Int(1);
                    }
                }
            }
            @jump = P.Int(0);
            foreach (object __t21 in P.Iter(P.Range(P.Sub(@j, 1), P.Neg(1), P.Neg(1))))
            {
                @jj = __t21;
                if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))))
                {
                    if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@back))))
                    {
                        break;
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@live))) && P.Truth((P.Equal(@opp_color, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@color))))))
                        {
                            @eat_number = P.Int(P.Add(@eat_number, 1));
                            break;
                        }
                    }
                }
                else
                {
                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))
                    {
                        @jump = P.Int(1);
                    }
                }
            }
            @jump = P.Int(0);
            foreach (object __t22 in P.Iter(P.Range(P.Add(@j, 1), 8, 1)))
            {
                @jj = __t22;
                if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))))
                {
                    if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@back))))
                    {
                        break;
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@live))) && P.Truth((P.Equal(@opp_color, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@color))))))
                        {
                            @eat_number = P.Int(P.Add(@eat_number, 1));
                            break;
                        }
                    }
                }
                else
                {
                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))
                    {
                        @jump = P.Int(1);
                    }
                }
            }
            return P.Int(@eat_number);
            return 0;
        }
        // Original darkchess.pyx: 433
        public object @eat_by_player_bomb(object @org, object @a_map, object @my_chess, object @player_color)
        {
            object @i = null;
            object @ii = null;
            object @j = null;
            object @jj = null;
            object @jump = null;
            object @was_ate = null;
            @player_color = P.Int(@player_color);
            object __t23 = @org;
            @i = P.Get(__t23, 0);
            @j = P.Get(__t23, 1);
            @jump = P.Int(0);
            @was_ate = P.Int(0);
            foreach (object __t24 in P.Iter(P.Range(P.Sub(@i, 1), P.Neg(1), P.Neg(1))))
            {
                @ii = __t24;
                if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))))
                {
                    if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@back))))
                    {
                        break;
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(2, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@value))) && P.Truth((P.Equal(@player_color, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@color))))))
                        {
                            @was_ate = P.Int(1);
                            break;
                        }
                    }
                }
                else
                {
                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))
                    {
                        @jump = P.Int(1);
                    }
                }
            }
            @jump = P.Int(0);
            foreach (object __t25 in P.Iter(P.Range(P.Add(@i, 1), 4, 1)))
            {
                @ii = __t25;
                if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))))
                {
                    if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@back))))
                    {
                        break;
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(2, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@value))) && P.Truth((P.Equal(@player_color, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @ii), @j), 0)), P.Get(P.Get(P.Get(@a_map, @ii), @j), 1))).@color))))))
                        {
                            @was_ate = P.Int(1);
                            break;
                        }
                    }
                }
                else
                {
                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))
                    {
                        @jump = P.Int(1);
                    }
                }
            }
            @jump = P.Int(0);
            foreach (object __t26 in P.Iter(P.Range(P.Sub(@j, 1), P.Neg(1), P.Neg(1))))
            {
                @jj = __t26;
                if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))))
                {
                    if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@back))))
                    {
                        break;
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(2, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@value))) && P.Truth((P.Equal(@player_color, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@color))))))
                        {
                            @was_ate = P.Int(1);
                            break;
                        }
                    }
                }
                else
                {
                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))
                    {
                        @jump = P.Int(1);
                    }
                }
            }
            @jump = P.Int(0);
            foreach (object __t27 in P.Iter(P.Range(P.Add(@j, 1), 8, 1)))
            {
                @jj = __t27;
                if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))))
                {
                    if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@back))))
                    {
                        break;
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(2, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@value))) && P.Truth((P.Equal(@player_color, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @jj), 0)), P.Get(P.Get(P.Get(@a_map, @i), @jj), 1))).@color))))))
                        {
                            @was_ate = P.Int(1);
                            break;
                        }
                    }
                }
                else
                {
                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))
                    {
                        @jump = P.Int(1);
                    }
                }
            }
            return P.Int(@was_ate);
            return 0;
        }
        // Original darkchess.pyx: 478
        public object @near(object @i, object @j)
        {
            object @n_cor = null;
            @i = P.Int(@i);
            @j = P.Int(@j);
            @n_cor = P.L();
            if (P.Truth((P.Truth((P.Equal(0, @i))) && P.Truth((P.Equal(0, @j))))))
            {
                ((PList)@n_cor).@extend(P.L(P.L(1, 0), P.L(0, 1)));
            }
            else
            {
                if (P.Truth((P.Truth((P.Equal(3, @i))) && P.Truth((P.Equal(0, @j))))))
                {
                    ((PList)@n_cor).@extend(P.L(P.L(2, 0), P.L(3, 1)));
                }
                else
                {
                    if (P.Truth((P.Truth((P.Equal(0, @i))) && P.Truth((P.Equal(7, @j))))))
                    {
                        ((PList)@n_cor).@extend(P.L(P.L(0, 6), P.L(1, 7)));
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(3, @i))) && P.Truth((P.Equal(7, @j))))))
                        {
                            ((PList)@n_cor).@extend(P.L(P.L(3, 6), P.L(2, 7)));
                        }
                        else
                        {
                            if (P.Truth((P.Equal(0, @j))))
                            {
                                ((PList)@n_cor).@extend(P.L(P.L(P.Sub(@i, 1), @j), P.L(P.Add(@i, 1), @j), P.L(@i, P.Add(@j, 1))));
                            }
                            else
                            {
                                if (P.Truth((P.Equal(0, @i))))
                                {
                                    ((PList)@n_cor).@extend(P.L(P.L(@i, P.Sub(@j, 1)), P.L(@i, P.Add(@j, 1)), P.L(P.Add(@i, 1), @j)));
                                }
                                else
                                {
                                    if (P.Truth((P.Equal(7, @j))))
                                    {
                                        ((PList)@n_cor).@extend(P.L(P.L(P.Sub(@i, 1), @j), P.L(P.Add(@i, 1), @j), P.L(@i, P.Sub(@j, 1))));
                                    }
                                    else
                                    {
                                        if (P.Truth((P.Equal(3, @i))))
                                        {
                                            ((PList)@n_cor).@extend(P.L(P.L(@i, P.Sub(@j, 1)), P.L(@i, P.Add(@j, 1)), P.L(P.Sub(@i, 1), @j)));
                                        }
                                        else
                                        {
                                            ((PList)@n_cor).@extend(P.L(P.L(P.Sub(@i, 1), @j), P.L(P.Add(@i, 1), @j), P.L(@i, P.Add(@j, 1)), P.L(@i, P.Sub(@j, 1))));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return @n_cor;
            return null;
        }
        // Original darkchess.pyx: 517
        public object @near_max_value(object @open, object @org, object @a_map, object @my_chess)
        {
            object @an = null;
            object @kk = null;
            object @max = null;
            object @near_cor = null;
            object @ni = null;
            object @nj = null;
            object @x = null;
            object @y = null;
            if (P.Truth((P.Equal(null, @open))))
            {
                return null;
            }
            object __t28 = @open;
            @y = P.Get(__t28, 0);
            @x = P.Get(__t28, 1);
            @near_cor = @near(@y, @x);
            @max = P.Int(0);
            foreach (object __t29 in P.Iter(@near_cor))
            {
                @kk = __t29;
                if (P.Truth((P.Equal(@kk, @org))))
                {
                    continue;
                }
                else
                {
                    object __t30 = @kk;
                    @ni = P.Get(__t30, 0);
                    @nj = P.Get(__t30, 1);
                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ni), @nj), null))))
                    {
                        @an = P.Get(P.Get(@a_map, @ni), @nj);
                        if (P.Truth((P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@live, 1))) && P.Truth(((P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@back) < P.Number(1)))))))
                        {
                            if (P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@color, @com_color))))
                            {
                                continue;
                            }
                            if (P.Truth(((P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@value) > P.Number(@max)))))
                            {
                                @max = P.Int(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@value);
                            }
                        }
                    }
                }
            }
            return @max;
            return null;
        }
        // Original darkchess.pyx: 537
        public object @near_max_value_not_consider_com_color(object @open, object @org, object @a_map, object @my_chess)
        {
            object @an = null;
            object @kk = null;
            object @max = null;
            object @near_cor = null;
            object @ni = null;
            object @nj = null;
            object @x = null;
            object @y = null;
            if (P.Truth((P.Equal(null, @open))))
            {
                return null;
            }
            object __t31 = @open;
            @y = P.Get(__t31, 0);
            @x = P.Get(__t31, 1);
            @near_cor = @near(@y, @x);
            @max = P.Int(0);
            foreach (object __t32 in P.Iter(@near_cor))
            {
                @kk = __t32;
                if (P.Truth((P.Equal(@kk, @org))))
                {
                    continue;
                }
                else
                {
                    object __t33 = @kk;
                    @ni = P.Get(__t33, 0);
                    @nj = P.Get(__t33, 1);
                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ni), @nj), null))))
                    {
                        @an = P.Get(P.Get(@a_map, @ni), @nj);
                        if (P.Truth((P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@live, 1))) && P.Truth(((P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@back) < P.Number(1)))))))
                        {
                            if (P.Truth(((P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@value) > P.Number(@max)))))
                            {
                                @max = P.Int(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@value);
                            }
                        }
                    }
                }
            }
            return @max;
            return null;
        }
        // Original darkchess.pyx: 557
        public object @scan_player_bomb(object @a_map, object @my_chess)
        {
            object @an = null;
            object @c = null;
            object @cr = null;
            object @i = null;
            object @ii = null;
            object @n = null;
            object @near_cor = null;
            object @ni = null;
            object @nj = null;
            foreach (object __t34 in P.Iter(@my_chess))
            {
                @cr = __t34;
                foreach (object __t35 in P.Iter(@cr))
                {
                    @c = __t35;
                    if (P.Truth((P.Truth((P.Equal(1, ((Piece)@c).@live))) && P.Truth(((P.Number(((Piece)@c).@back) < P.Number(1)))) && P.Truth((P.Equal(2, ((Piece)@c).@value))) && P.Truth((P.Equal(@player_color, ((Piece)@c).@color))))))
                    {
                        @near_cor = @near(((Piece)@c).@row, ((Piece)@c).@col);
                        @i = random.RandInt(0, P.Sub(P.Len(@near_cor), 1));
                        foreach (object __t36 in P.Iter(P.Range(0, P.Len(@near_cor))))
                        {
                            @ii = __t36;
                            @n = P.Mod(P.Add(@i, @ii), P.Len(@near_cor));
                            object __t37 = P.Get(@near_cor, @n);
                            @ni = P.Get(__t37, 0);
                            @nj = P.Get(__t37, 1);
                            @an = P.Get(P.Get(@a_map, @ni), @nj);
                            if (P.Truth((P.Equal(null, @an))))
                            {
                                continue;
                            }
                            else
                            {
                                if (P.Truth((P.Truth((P.Equal(0, ((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@live))) || P.Truth(((P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@back) < P.Number(1)))))))
                                {
                                    continue;
                                }
                            }
                            if (P.Truth((P.Truth(((P.Number(@near_max_value(P.Get(@near_cor, @n), P.L(((Piece)@c).@row, ((Piece)@c).@col), @a_map, @my_chess)) <= P.Number(3)))) && P.Truth((P.Equal(0, @eat_by_player_bomb(P.Get(@near_cor, @n), @a_map, @my_chess, @player_color)))))))
                            {
                                return P.Get(@near_cor, @n);
                            }
                        }
                    }
                }
            }
            return null;
            return null;
        }
        // Original darkchess.pyx: 593
        public object @bomb_will_eat(object @org, object @a_map, object @my_chess)
        {
            object @an = null;
            object @bn = null;
            object @i = null;
            object @i2 = null;
            object @ii = null;
            object @jj = null;
            object @jump = null;
            object @n = null;
            object @ni = null;
            object @nj = null;
            if (P.Truth((P.Equal(null, @org))))
            {
                return null;
            }
            @i = P.Int(random.RandInt(0, 3));
            foreach (object __t38 in P.Iter(P.Range(0, 4)))
            {
                @i2 = __t38;
                @n = P.Mod(P.Add(@i, @i2), 4);
                object __t39 = @org;
                @ni = P.Get(__t39, 0);
                @nj = P.Get(__t39, 1);
                if (P.Truth((P.Equal(0, @n))))
                {
                    @jump = 0;
                    foreach (object __t40 in P.Iter(P.Range(P.Sub(@ni, 1), P.Neg(1), P.Neg(1))))
                    {
                        @ii = __t40;
                        if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @nj), null))))))
                        {
                            @an = P.Get(P.Get(@a_map, @ii), @nj);
                            if (P.Truth((P.Truth(((P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@back) < P.Number(1)))) && P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@color, @player_color))))))
                            {
                                if (P.Truth(((P.Number(@near_max_value(P.L(@ii, @nj), null, @a_map, @my_chess)) < P.Number(2)))))
                                {
                                    return @org;
                                }
                            }
                            break;
                        }
                        if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @nj), null))))
                        {
                            @bn = P.Get(P.Get(@a_map, @ii), @nj);
                            if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@bn, 0)), P.Get(@bn, 1))).@back))))
                            {
                                @jump = 1;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (P.Truth((P.Equal(1, @n))))
                    {
                        @jump = 0;
                        foreach (object __t41 in P.Iter(P.Range(P.Add(@ni, 1), 4, 1)))
                        {
                            @ii = __t41;
                            if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @nj), null))))))
                            {
                                @an = P.Get(P.Get(@a_map, @ii), @nj);
                                if (P.Truth((P.Truth(((P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@back) < P.Number(1)))) && P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@color, @player_color))))))
                                {
                                    if (P.Truth(((P.Number(@near_max_value(P.L(@ii, @nj), null, @a_map, @my_chess)) < P.Number(2)))))
                                    {
                                        return @org;
                                    }
                                }
                                break;
                            }
                            if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @nj), null))))
                            {
                                @bn = P.Get(P.Get(@a_map, @ii), @nj);
                                if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@bn, 0)), P.Get(@bn, 1))).@back))))
                                {
                                    @jump = 1;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (P.Truth((P.Equal(2, @n))))
                        {
                            @jump = 0;
                            foreach (object __t42 in P.Iter(P.Range(P.Sub(@nj, 1), P.Neg(1), P.Neg(1))))
                            {
                                @jj = __t42;
                                if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ni), @jj), null))))))
                                {
                                    @an = P.Get(P.Get(@a_map, @ni), @jj);
                                    if (P.Truth((P.Truth(((P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@back) < P.Number(1)))) && P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@color, @player_color))))))
                                    {
                                        if (P.Truth(((P.Number(@near_max_value(P.L(@ni, @jj), null, @a_map, @my_chess)) < P.Number(2)))))
                                        {
                                            return @org;
                                        }
                                    }
                                    break;
                                }
                                if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ni), @jj), null))))
                                {
                                    @bn = P.Get(P.Get(@a_map, @ni), @jj);
                                    if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@bn, 0)), P.Get(@bn, 1))).@back))))
                                    {
                                        @jump = 1;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (P.Truth((P.Equal(3, @n))))
                            {
                                @jump = 0;
                                foreach (object __t43 in P.Iter(P.Range(P.Add(@nj, 1), 8, 1)))
                                {
                                    @jj = __t43;
                                    if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ni), @jj), null))))))
                                    {
                                        @an = P.Get(P.Get(@a_map, @ni), @jj);
                                        if (P.Truth((P.Truth(((P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@back) < P.Number(1)))) && P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@color, @player_color))))))
                                        {
                                            if (P.Truth(((P.Number(@near_max_value(P.L(@ni, @jj), null, @a_map, @my_chess)) < P.Number(2)))))
                                            {
                                                return @org;
                                            }
                                        }
                                        break;
                                    }
                                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ni), @jj), null))))
                                    {
                                        @bn = P.Get(P.Get(@a_map, @ni), @jj);
                                        if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@bn, 0)), P.Get(@bn, 1))).@back))))
                                        {
                                            @jump = 1;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
            return null;
        }
        // Original darkchess.pyx: 663
        public object @bomb_may_eat(object @org, object @a_map, object @my_chess)
        {
            object @an = null;
            object @bn = null;
            object @i = null;
            object @i2 = null;
            object @ii = null;
            object @jj = null;
            object @jump = null;
            object @n = null;
            object @ni = null;
            object @nj = null;
            if (P.Truth((P.Equal(null, @org))))
            {
                return null;
            }
            @i = P.Int(random.RandInt(0, 3));
            foreach (object __t44 in P.Iter(P.Range(0, 4)))
            {
                @i2 = __t44;
                @n = P.Mod(P.Add(@i, @i2), 4);
                object __t45 = @org;
                @ni = P.Get(__t45, 0);
                @nj = P.Get(__t45, 1);
                if (P.Truth((P.Equal(0, @n))))
                {
                    @jump = 0;
                    foreach (object __t46 in P.Iter(P.Range(P.Sub(@ni, 1), P.Neg(1), P.Neg(1))))
                    {
                        @ii = __t46;
                        if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @nj), null))))))
                        {
                            @an = P.Get(P.Get(@a_map, @ii), @nj);
                            if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@back))))
                            {
                                if (P.Truth(((P.Number(@near_max_value_not_consider_com_color(P.L(@ii, @nj), null, @a_map, @my_chess)) < P.Number(2)))))
                                {
                                    return P.L(@ii, @nj);
                                }
                            }
                            break;
                        }
                        if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @nj), null))))
                        {
                            @bn = P.Get(P.Get(@a_map, @ii), @nj);
                            if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@bn, 0)), P.Get(@bn, 1))).@back))))
                            {
                                @jump = 1;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (P.Truth((P.Equal(1, @n))))
                    {
                        @jump = 0;
                        foreach (object __t47 in P.Iter(P.Range(P.Add(@ni, 1), 4, 1)))
                        {
                            @ii = __t47;
                            if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @nj), null))))))
                            {
                                @an = P.Get(P.Get(@a_map, @ii), @nj);
                                if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@back))))
                                {
                                    if (P.Truth(((P.Number(@near_max_value_not_consider_com_color(P.L(@ii, @nj), null, @a_map, @my_chess)) < P.Number(2)))))
                                    {
                                        return P.L(@ii, @nj);
                                    }
                                }
                                break;
                            }
                            if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @nj), null))))
                            {
                                @bn = P.Get(P.Get(@a_map, @ii), @nj);
                                if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@bn, 0)), P.Get(@bn, 1))).@back))))
                                {
                                    @jump = 1;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (P.Truth((P.Equal(2, @n))))
                        {
                            @jump = 0;
                            foreach (object __t48 in P.Iter(P.Range(P.Sub(@nj, 1), P.Neg(1), P.Neg(1))))
                            {
                                @jj = __t48;
                                if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ni), @jj), null))))))
                                {
                                    @an = P.Get(P.Get(@a_map, @ni), @jj);
                                    if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@back))))
                                    {
                                        if (P.Truth(((P.Number(@near_max_value_not_consider_com_color(P.L(@ni, @jj), null, @a_map, @my_chess)) < P.Number(2)))))
                                        {
                                            return P.L(@ni, @jj);
                                        }
                                    }
                                    break;
                                }
                                if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ni), @jj), null))))
                                {
                                    @bn = P.Get(P.Get(@a_map, @ni), @jj);
                                    if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@bn, 0)), P.Get(@bn, 1))).@back))))
                                    {
                                        @jump = 1;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (P.Truth((P.Equal(3, @n))))
                            {
                                @jump = 0;
                                foreach (object __t49 in P.Iter(P.Range(P.Add(@nj, 1), 8, 1)))
                                {
                                    @jj = __t49;
                                    if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ni), @jj), null))))))
                                    {
                                        @an = P.Get(P.Get(@a_map, @ni), @jj);
                                        if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@back))))
                                        {
                                            if (P.Truth(((P.Number(@near_max_value_not_consider_com_color(P.L(@ni, @jj), null, @a_map, @my_chess)) < P.Number(2)))))
                                            {
                                                return P.L(@ni, @jj);
                                            }
                                        }
                                        break;
                                    }
                                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ni), @jj), null))))
                                    {
                                        @bn = P.Get(P.Get(@a_map, @ni), @jj);
                                        if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@bn, 0)), P.Get(@bn, 1))).@back))))
                                        {
                                            @jump = 1;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
            return null;
        }
        // Original darkchess.pyx: 733
        public object @scan_com_bomb(object @a_map, object @my_chess)
        {
            object @c = null;
            object @cor = null;
            object @cr = null;
            @cor = null;
            foreach (object __t50 in P.Iter(@my_chess))
            {
                @cr = __t50;
                foreach (object __t51 in P.Iter(@cr))
                {
                    @c = __t51;
                    if (P.Truth((P.Truth((P.Equal(1, ((Piece)@c).@live))) && P.Truth(((P.Number(((Piece)@c).@back) < P.Number(1)))) && P.Truth((P.Equal(2, ((Piece)@c).@value))) && P.Truth((P.Equal(@com_color, ((Piece)@c).@color))))))
                    {
                        @cor = @bomb_may_eat(P.L(((Piece)@c).@row, ((Piece)@c).@col), @a_map, @my_chess);
                    }
                }
            }
            return @cor;
            return null;
        }
        // Original darkchess.pyx: 772
        public object @select_back_chess(object @a_map, object @my_chess, object @org = null)
        {
            object @cor = null;
            object @i = null;
            object @j = null;
            object @randomi = null;
            object @x0 = null;
            object @x1 = null;
            object @x2 = null;
            object @y0 = null;
            object @y1 = null;
            object @y2 = null;
            object __t52 = P.L(null, null);
            @i = P.Get(__t52, 0);
            @j = P.Get(__t52, 1);
            @cor = @scan_player_bomb(@a_map, @my_chess);
            if (P.Truth((!P.Equal(@cor, null))))
            {
                return @cor;
            }
            @cor = @scan_com_bomb(@a_map, @my_chess);
            if (P.Truth((!P.Equal(@cor, null))))
            {
                return @cor;
            }
            @randomi = random.RandInt(0, 1);
            if (P.Truth((P.Equal(0, @randomi))))
            {
                @y0 = 0;
                @y1 = 4;
                @y2 = 1;
            }
            else
            {
                @y0 = 3;
                @y1 = P.Neg(1);
                @y2 = P.Neg(1);
            }
            @randomi = random.RandInt(0, 1);
            if (P.Truth((P.Equal(0, @randomi))))
            {
                @x0 = 0;
                @x1 = 8;
                @x2 = 1;
            }
            else
            {
                @x0 = 7;
                @x1 = P.Neg(1);
                @x2 = P.Neg(1);
            }
            object __t53 = @calc_good_backchess(@y0, @y1, @y2, @x0, @x1, @x2, @a_map, @my_chess, @max_eat_number: 0);
            @i = P.Get(__t53, 0);
            @j = P.Get(__t53, 1);
            if (P.Truth((!P.Equal(P.L(@i, @j), P.L(null, null)))))
            {
                return P.L(@i, @j);
            }
            else
            {
                if (P.Truth((!P.Equal(@org, null))))
                {
                    return P.L(P.Neg(1), P.Neg(1));
                }
                else
                {
                    if (P.Truth((P.Equal(1, @check_back_exist(@a_map, @my_chess)))))
                    {
                        return @calc_good_backchess(@y0, @y1, @y2, @x0, @x1, @x2, @a_map, @my_chess);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }
        // Original darkchess.pyx: 819
        public object @check_back_exist(object @a_map, object @my_chess)
        {
            object @back_exist = null;
            object @i = null;
            object @j = null;
            @back_exist = P.Int(0);
            foreach (object __t54 in P.Iter(P.Range(0, 4)))
            {
                @i = __t54;
                foreach (object __t55 in P.Iter(P.Range(0, 8)))
                {
                    @j = __t55;
                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @j), null))))
                    {
                        if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@back))))
                        {
                            return P.Int(1);
                        }
                    }
                }
            }
            return P.Int(@back_exist);
            return 0;
        }
        // Original darkchess.pyx: 830
        public object @calc_good_backchess(object @y0, object @y1, object @y2, object @x0, object @x1, object @x2, object @a_map, object @my_chess, int @max_eat_number = -33)
        {
            object @an = null;
            object @i = null;
            object @j = null;
            object @kk = null;
            object @n = null;
            object @ne = null;
            object @near_cor = null;
            object @near_max = null;
            object @near_min = null;
            object @near_our_max = null;
            object @near_our_min = null;
            object @ni = null;
            object @nj = null;
            object @x = null;
            object @y = null;
            @y0 = P.Int(@y0);
            @x0 = P.Int(@x0);
            @y1 = P.Int(@y1);
            @x2 = P.Int(@x2);
            @x1 = P.Int(@x1);
            @y2 = P.Int(@y2);
            @max_eat_number = P.Int(@max_eat_number);
            object __t56 = P.L(null, null);
            @i = P.Get(__t56, 0);
            @j = P.Get(__t56, 1);
            foreach (object __t57 in P.Iter(P.Range(@y0, @y1, @y2)))
            {
                @y = __t57;
                foreach (object __t58 in P.Iter(P.Range(@x0, @x1, @x2)))
                {
                    @x = __t58;
                    @near_min = 8;
                    @near_max = 0;
                    @near_our_min = 8;
                    @near_our_max = 0;
                    if (P.Truth((P.Equal(null, P.Get(P.Get(@a_map, @y), @x)))))
                    {
                        continue;
                    }
                    @n = P.Get(P.Get(@a_map, @y), @x);
                    if (P.Truth(((P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@back) < P.Number(1)))))
                    {
                        continue;
                    }
                    if (P.Truth((P.Equal(0, @eat_by_player_bomb(P.L(@y, @x), @a_map, @my_chess, @player_color)))))
                    {
                        @near_cor = @near(@y, @x);
                        foreach (object __t59 in P.Iter(@near_cor))
                        {
                            @kk = __t59;
                            object __t60 = @kk;
                            @ni = P.Get(__t60, 0);
                            @nj = P.Get(__t60, 1);
                            if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ni), @nj), null))))
                            {
                                @an = P.Get(P.Get(@a_map, @ni), @nj);
                                if (P.Truth(((P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@back) < P.Number(1)))))
                                {
                                    if (P.Truth((P.Truth(((P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@value) < P.Number(@near_min)))) && P.Truth((P.Equal(@player_color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@color))))))
                                    {
                                        @near_min = ((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@value;
                                    }
                                    if (P.Truth((P.Truth(((P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@value) > P.Number(@near_max)))) && P.Truth((P.Equal(@player_color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@color))))))
                                    {
                                        @near_max = ((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@value;
                                    }
                                    if (P.Truth((P.Truth(((P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@value) < P.Number(@near_our_min)))) && P.Truth((P.Equal(@com_color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@color))))))
                                    {
                                        @near_our_min = ((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@value;
                                    }
                                    if (P.Truth((P.Truth(((P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@value) > P.Number(@near_our_max)))) && P.Truth((P.Equal(@com_color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@color))))))
                                    {
                                        @near_our_max = ((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@value;
                                    }
                                }
                            }
                        }
                        @ne = @check_eat_number(@a_map, @my_chess, @near_min, @near_max, @near_our_min, @near_our_max, @y, @x);
                        if (P.Truth(((P.Number(@ne) > P.Number(@max_eat_number)))))
                        {
                            @max_eat_number = P.Int(@ne);
                            object __t61 = P.L(@y, @x);
                            @i = P.Get(__t61, 0);
                            @j = P.Get(__t61, 1);
                        }
                    }
                    else
                    {
                        if (P.Truth((P.Equal(P.Neg(33), @max_eat_number))))
                        {
                            @max_eat_number = P.Int(P.Mul(P.Neg(1), @back_num));
                            object __t62 = P.L(@y, @x);
                            @i = P.Get(__t62, 0);
                            @j = P.Get(__t62, 1);
                        }
                    }
                }
            }
            return P.L(@i, @j);
            return null;
        }
        // Original darkchess.pyx: 898
        public object @chess_ai()
        {
            object @a_map = null;
            object @c = null;
            object @cindex = null;
            object @dest = null;
            object @i = null;
            object @j = null;
            object @m = null;
            object @move_pre1 = null;
            object @move_pre2 = null;
            object @move_pre3 = null;
            object @move_pre4 = null;
            object @n1 = null;
            object @n2 = null;
            object @org = null;
            object @p = null;
            object @sc = null;
            object @score = null;
            object @sound_click = null;
            object @temp = null;
            if (P.Truth((P.Truth((P.Equal(0, @player_first))) && P.Truth((P.Equal(1, @first))))))
            {
                @i = random.RandInt(0, 3);
                @j = random.RandInt(0, 7);
                @cindex = ColorValueToIndex(((Piece)P.Get(P.Get(@server_main_chess, @i), @j)).@color, ((Piece)P.Get(P.Get(@server_main_chess, @i), @j)).@value, @back_value_num);
                P.Set(P.Get(@main_chess, @i), @j, NewPiece(@cindex, P.L(@i, @j)));
                @turn_id = P.Int(((Piece)P.Get(P.Get(@main_chess, @i), @j)).@color);
                ((Piece)P.Get(P.Get(@main_chess, @i), @j)).@back = P.Int(P.Neg(1));
                @back_num = P.Int(P.Sub(@back_num, 1));
                @com_color = P.Int(@turn_id);
                @player_color = P.Int(P.Sub(1, @com_color));
                @first = P.Int(0);
                P.Set(P.Get(@back_value_num, @com_color), ((Piece)P.Get(P.Get(@main_chess, @i), @j)).@value, P.Sub(P.Get(P.Get(@back_value_num, @com_color), ((Piece)P.Get(P.Get(@main_chess, @i), @j)).@value), 1));
            }
            else
            {
                if (P.Truth((P.Truth((P.Equal(@turn_id, @com_color))) && P.Truth((P.Equal(0, @first))))))
                {
                    @main_chess = @clean_back_n1_to_0(@main_chess);
                    @move_pre1 = P.Get(@move_step, P.Mod(P.Sub(@sindex, 1), 4));
                    @move_pre2 = P.Get(@move_step, P.Mod(P.Sub(@sindex, 2), 4));
                    @move_pre3 = P.Get(@move_step, P.Mod(P.Sub(@sindex, 3), 4));
                    @move_pre4 = P.Get(@move_step, @sindex);
                    if (P.Truth((P.Truth((!P.Equal(@move_pre1, null))) && P.Truth((!P.Equal(@move_pre2, null))) && P.Truth((!P.Equal(@move_pre3, null))) && P.Truth((!P.Equal(@move_pre4, null))))))
                    {
                        if (P.Truth((P.Truth((P.Equal(P.Get(@move_pre1, 0), @player_color))) && P.Truth((P.Equal(P.Get(@move_pre2, 0), @com_color))) && P.Truth((P.Equal(P.Get(@move_pre3, 0), @player_color))) && P.Truth((P.Equal(P.Get(@move_pre4, 0), @com_color))) && P.Truth(@in_com_possible_move(P.Get(@move_pre1, 1), P.Get(@move_pre2, 3))) && P.Truth(@in_com_possible_move(P.Get(@move_pre3, 1), P.Get(@move_pre4, 3))) && P.Truth((P.Equal(P.Get(@move_pre2, 2), P.Get(@move_pre4, 1)))) && P.Truth((P.Equal(P.Get(@move_pre1, 2), P.Get(@move_pre3, 1)))))))
                        {
                            @n1 = P.Get(@move_pre1, 1);
                            @n2 = P.Get(@move_pre2, 1);
                            @p = P.Get(@move_pre1, 2);
                            @c = P.Get(@move_pre2, 2);
                            ((PList)@break_long_capture_dest).@append(P.L(@n1, @n2, @p, @c));
                            ((PList)@break_long_capture_org).@append(P.L(@p, @c));
                            ((PList)@com_ban_step).@append(P.Get(@move_pre4, 1));
                        }
                    }
                    object __t63 = @com_think(@main_map, @main_chess);
                    @org = P.Get(__t63, 0);
                    @dest = P.Get(__t63, 1);
                    @score = P.Get(__t63, 2);
                    if (P.Truth((P.Truth((P.Equal(0, @back_num))) && P.Truth((P.Equal(1, @cant_move(@main_map, @main_chess, @com_color)))))))
                    {
                        @player_win = P.Int(1);
                    }
                    if (P.Truth(((P.Number(@back_num) > P.Number(0)))))
                    {
                        if (P.Truth((!P.Equal(@open_score, null))))
                        {
                            if (P.Truth((P.Equal(null, @org))))
                            {
                                @dest = @select_back_chess(@main_map, @main_chess);
                                @sound_click = NewSound(@s_click);
                                ((DisplaySound)@sound_click).@play();
                                @sc = P.Get(P.Get(@server_main_chess, P.Get(P.Get(P.Get(@main_map, P.Get(@dest, 0)), P.Get(@dest, 1)), 0)), P.Get(P.Get(P.Get(@main_map, P.Get(@dest, 0)), P.Get(@dest, 1)), 1));
                                @cindex = ColorValueToIndex(((Piece)@sc).@color, ((Piece)@sc).@value, @back_value_num);
                                P.Set(P.Get(@main_chess, P.Get(P.Get(P.Get(@main_map, P.Get(@dest, 0)), P.Get(@dest, 1)), 0)), P.Get(P.Get(P.Get(@main_map, P.Get(@dest, 0)), P.Get(@dest, 1)), 1), NewPiece(@cindex, P.L(P.Get(P.Get(P.Get(@main_map, P.Get(@dest, 0)), P.Get(@dest, 1)), 0), P.Get(P.Get(P.Get(@main_map, P.Get(@dest, 0)), P.Get(@dest, 1)), 1))));
                                @m = P.Get(P.Get(@main_chess, P.Get(P.Get(P.Get(@main_map, P.Get(@dest, 0)), P.Get(@dest, 1)), 0)), P.Get(P.Get(P.Get(@main_map, P.Get(@dest, 0)), P.Get(@dest, 1)), 1));
                                ((Piece)@m).@back = P.Int(P.Neg(1));
                                @back_num = P.Int(P.Sub(@back_num, 1));
                                P.Set(P.Get(@back_value_num, ((Piece)@m).@color), ((Piece)@m).@value, P.Sub(P.Get(P.Get(@back_value_num, ((Piece)@m).@color), ((Piece)@m).@value), 1));
                            }
                            else
                            {
                                if (P.Truth(((P.Number(@score) > P.Number(@open_score)))))
                                {
                                    if (P.Truth(((P.Number(@score) > P.Number(18)))))
                                    {
                                        @org = null;
                                    }
                                    @temp = @select_back_chess(@main_map, @main_chess, @org);
                                    if (P.Truth((P.Equal(P.L(P.Neg(1), P.Neg(1)), @temp))))
                                    {
                                        object __t64 = @move_s(@org, @dest, @main_map, @main_chess);
                                        @main_map = P.Get(__t64, 0);
                                        @main_chess = P.Get(__t64, 1);
                                        @a_map = P.Get(__t64, 2);
                                        @save_step_and_break_long_capture(@org, @dest, @a_map, @main_chess);
                                    }
                                    else
                                    {
                                        @sound_click = NewSound(@s_click);
                                        ((DisplaySound)@sound_click).@play();
                                        @sc = P.Get(P.Get(@server_main_chess, P.Get(P.Get(P.Get(@main_map, P.Get(@temp, 0)), P.Get(@temp, 1)), 0)), P.Get(P.Get(P.Get(@main_map, P.Get(@temp, 0)), P.Get(@temp, 1)), 1));
                                        @cindex = ColorValueToIndex(((Piece)@sc).@color, ((Piece)@sc).@value, @back_value_num);
                                        P.Set(P.Get(@main_chess, P.Get(P.Get(P.Get(@main_map, P.Get(@temp, 0)), P.Get(@temp, 1)), 0)), P.Get(P.Get(P.Get(@main_map, P.Get(@temp, 0)), P.Get(@temp, 1)), 1), NewPiece(@cindex, P.L(P.Get(P.Get(P.Get(@main_map, P.Get(@temp, 0)), P.Get(@temp, 1)), 0), P.Get(P.Get(P.Get(@main_map, P.Get(@temp, 0)), P.Get(@temp, 1)), 1))));
                                        @m = P.Get(P.Get(@main_chess, P.Get(P.Get(P.Get(@main_map, P.Get(@temp, 0)), P.Get(@temp, 1)), 0)), P.Get(P.Get(P.Get(@main_map, P.Get(@temp, 0)), P.Get(@temp, 1)), 1));
                                        ((Piece)@m).@back = P.Int(P.Neg(1));
                                        @back_num = P.Int(P.Sub(@back_num, 1));
                                        P.Set(P.Get(@back_value_num, ((Piece)@m).@color), ((Piece)@m).@value, P.Sub(P.Get(P.Get(@back_value_num, ((Piece)@m).@color), ((Piece)@m).@value), 1));
                                    }
                                }
                                else
                                {
                                    if (P.Truth((P.Equal(@score, @open_score))))
                                    {
                                        if (P.Truth(((P.Number(@score) >= P.Number(0)))))
                                        {
                                            if (P.Truth(((P.Number(@score) > P.Number(18)))))
                                            {
                                                @org = null;
                                            }
                                            @temp = @select_back_chess(@main_map, @main_chess, @org);
                                            if (P.Truth((P.Equal(P.L(P.Neg(1), P.Neg(1)), @temp))))
                                            {
                                                object __t65 = @move_s(@org, @dest, @main_map, @main_chess);
                                                @main_map = P.Get(__t65, 0);
                                                @main_chess = P.Get(__t65, 1);
                                                @a_map = P.Get(__t65, 2);
                                                @save_step_and_break_long_capture(@org, @dest, @a_map, @main_chess);
                                            }
                                            else
                                            {
                                                @sound_click = NewSound(@s_click);
                                                ((DisplaySound)@sound_click).@play();
                                                @sc = P.Get(P.Get(@server_main_chess, P.Get(P.Get(P.Get(@main_map, P.Get(@temp, 0)), P.Get(@temp, 1)), 0)), P.Get(P.Get(P.Get(@main_map, P.Get(@temp, 0)), P.Get(@temp, 1)), 1));
                                                @cindex = ColorValueToIndex(((Piece)@sc).@color, ((Piece)@sc).@value, @back_value_num);
                                                P.Set(P.Get(@main_chess, P.Get(P.Get(P.Get(@main_map, P.Get(@temp, 0)), P.Get(@temp, 1)), 0)), P.Get(P.Get(P.Get(@main_map, P.Get(@temp, 0)), P.Get(@temp, 1)), 1), NewPiece(@cindex, P.L(P.Get(P.Get(P.Get(@main_map, P.Get(@temp, 0)), P.Get(@temp, 1)), 0), P.Get(P.Get(P.Get(@main_map, P.Get(@temp, 0)), P.Get(@temp, 1)), 1))));
                                                @m = P.Get(P.Get(@main_chess, P.Get(P.Get(P.Get(@main_map, P.Get(@temp, 0)), P.Get(@temp, 1)), 0)), P.Get(P.Get(P.Get(@main_map, P.Get(@temp, 0)), P.Get(@temp, 1)), 1));
                                                ((Piece)@m).@back = P.Int(P.Neg(1));
                                                @back_num = P.Int(P.Sub(@back_num, 1));
                                                P.Set(P.Get(@back_value_num, ((Piece)@m).@color), ((Piece)@m).@value, P.Sub(P.Get(P.Get(@back_value_num, ((Piece)@m).@color), ((Piece)@m).@value), 1));
                                            }
                                        }
                                        else
                                        {
                                            object __t66 = @move_s(@org, @dest, @main_map, @main_chess);
                                            @main_map = P.Get(__t66, 0);
                                            @main_chess = P.Get(__t66, 1);
                                            @a_map = P.Get(__t66, 2);
                                            @save_step_and_break_long_capture(@org, @dest, @a_map, @main_chess);
                                        }
                                    }
                                    else
                                    {
                                        object __t67 = @move_s(@org, @dest, @main_map, @main_chess);
                                        @main_map = P.Get(__t67, 0);
                                        @main_chess = P.Get(__t67, 1);
                                        @a_map = P.Get(__t67, 2);
                                        @save_step_and_break_long_capture(@org, @dest, @a_map, @main_chess);
                                    }
                                }
                            }
                        }
                        else
                        {
                            object __t68 = @move_s(@org, @dest, @main_map, @main_chess);
                            @main_map = P.Get(__t68, 0);
                            @main_chess = P.Get(__t68, 1);
                            @a_map = P.Get(__t68, 2);
                            @save_step_and_break_long_capture(@org, @dest, @a_map, @main_chess);
                        }
                    }
                    else
                    {
                        if (P.Truth((P.Equal(0, @player_win))))
                        {
                            object __t69 = @move_s(@org, @dest, @main_map, @main_chess);
                            @main_map = P.Get(__t69, 0);
                            @main_chess = P.Get(__t69, 1);
                            @a_map = P.Get(__t69, 2);
                            @save_step_and_break_long_capture(@org, @dest, @a_map, @main_chess);
                        }
                    }
                }
            }
            if (P.Truth((P.Equal(@turn_id, @com_color))))
            {
                @step = P.Int(P.Add(@step, 1));
                @turn_id = P.Int(2);
            }
            return null;
        }
        // Original darkchess.pyx: 1021
        public object @f_short_dist(object @i, object @j, object @dist, object @a_map)
        {
            object @d = null;
            object @nc = null;
            object @ncor = null;
            @i = P.Int(@i);
            @j = P.Int(@j);
            @d = 0;
            @ncor = @near(@i, @j);
            foreach (object __t70 in P.Iter(@ncor))
            {
                @nc = __t70;
                if (P.Truth((P.Truth((!P.Equal(P.Get(P.Get(@mark, P.Get(@nc, 0)), P.Get(@nc, 1)), 0))) && P.Truth(((P.Number(P.Get(P.Get(@mark, P.Get(@nc, 0)), P.Get(@nc, 1))) < P.Number(@dist)))))))
                {
                    if (P.Truth((P.Equal(0, @d))))
                    {
                        @d = P.Add(P.Get(P.Get(@mark, P.Get(@nc, 0)), P.Get(@nc, 1)), 1);
                    }
                    else
                    {
                        if (P.Truth(((P.Number(P.Add(P.Get(P.Get(@mark, P.Get(@nc, 0)), P.Get(@nc, 1)), 1)) < P.Number(@d)))))
                        {
                            @d = P.Add(P.Get(P.Get(@mark, P.Get(@nc, 0)), P.Get(@nc, 1)), 1);
                        }
                    }
                }
            }
            if (P.Truth((P.Truth((P.Equal(0, @d))) && P.Truth((P.Equal(null, P.Get(P.Get(@a_map, @i), @j)))))))
            {
                @d = @dist;
            }
            return P.Int(@d);
            return 0;
        }
        // Original darkchess.pyx: 1036
        public object @f_calc_move_score(object @max_value, object @max_dist, object @my_value)
        {
            object @mvalue = null;
            @max_value = P.Number(@max_value);
            @max_dist = P.Number(@max_dist);
            @my_value = P.Number(@my_value);
            @mvalue = P.Div(@my_value, 11);
            if (P.Truth((P.Equal(9, @max_value))))
            {
                if (P.Truth((!P.Equal(@max_dist, 0))))
                {
                    if (P.Truth(((P.Number(3.5) > P.Number(P.Mul(0.2, @max_dist))))))
                    {
                        return P.Number(SourceFloat.Fma(P.Neg(0.2), @max_dist, 4.2));
                    }
                    else
                    {
                        return P.Number(P.Sub(0.7, P.Div(@max_dist, 1000)));
                    }
                }
                else
                {
                    return P.Number(0);
                }
            }
            else
            {
                if (P.Truth((!P.Equal(@max_value, 0))))
                {
                    if (P.Truth(((P.Number(P.Div(@max_value, 2)) > P.Number(P.Mul(0.2, @max_dist))))))
                    {
                        return P.Number(P.Add(SourceFloat.Fma(P.Neg(0.2), @max_dist, P.Div(@max_value, 2)), @mvalue));
                    }
                    else
                    {
                        return P.Number(P.Sub(@mvalue, P.Div(@max_dist, 1000)));
                    }
                }
                else
                {
                    return P.Number(P.Neg(0.1));
                }
            }
            return 0;
        }
        // Original darkchess.pyx: 1060
        public object @first_move_max_value(object @orgx, object @orgy, object @destx, object @desty, object @my_chess, object @a_map, object @org_value, object @owner_color, object @i, object @j, int @dist = 1)
        {
            object @an = null;
            object @current_dist = null;
            object @n_c = null;
            object @nc = null;
            object @ni = null;
            object @nj = null;
            object @opp_color = null;
            @orgx = P.Int(@orgx);
            @j = P.Int(@j);
            @dist = P.Int(@dist);
            @i = P.Int(@i);
            @destx = P.Int(@destx);
            @owner_color = P.Int(@owner_color);
            @org_value = P.Int(@org_value);
            @desty = P.Int(@desty);
            @orgy = P.Int(@orgy);
            if (P.Truth((P.Truth((P.Equal(@i, P.Neg(1)))) || P.Truth((P.Equal(@j, P.Neg(1)))) || P.Truth((P.Equal(@i, 4))) || P.Truth((P.Equal(@j, 8))))))
            {
                return null;
            }
            else
            {
                if (P.Truth((P.Truth((P.Equal(@i, @orgy))) && P.Truth((P.Equal(@j, @orgx))))))
                {
                    return null;
                }
                else
                {
                    if (P.Truth((P.Truth(((P.Number(P.Get(P.Get(@mark, @i), @j)) > P.Number(0)))) || P.Truth(((P.Number(P.Get(P.Get(@cannon_mark, @i), @j)) > P.Number(0)))))))
                    {
                        return null;
                    }
                }
            }
            @n_c = @near(@i, @j);
            foreach (object __t71 in P.Iter(@n_c))
            {
                @nc = __t71;
                object __t72 = @nc;
                @ni = P.Get(__t72, 0);
                @nj = P.Get(__t72, 1);
                if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ni), @nj), null))))
                {
                    @an = P.Get(P.Get(@a_map, @ni), @nj);
                    if (P.Truth((P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@live, 1))) && P.Truth(((P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@back) < P.Number(1)))) && P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@color, @owner_color))))))
                    {
                        if (P.Truth((P.Equal(1, @can_be_ate(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @orgy), @orgx), 0)), P.Get(P.Get(P.Get(@a_map, @orgy), @orgx), 1))).@value, ((Piece)P.Get(P.Get(@my_chess, P.Get(@an, 0)), P.Get(@an, 1))).@value)))))
                        {
                            return null;
                        }
                    }
                }
            }
            if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @j), null))))
            {
                if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@back))))
                {
                    return null;
                }
            }
            @opp_color = P.Sub(1, @owner_color);
            @current_dist = 32;
            if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @j), null))))
            {
                @current_dist = @f_short_dist(@i, @j, @dist, @a_map);
            }
            else
            {
                P.Set(P.Get(@mark, @i), @j, @f_short_dist(@i, @j, @dist, @a_map));
            }
            if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @j), null))))
            {
                if (P.Truth((P.Equal(@opp_color, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@color))))
                {
                    if (P.Truth((P.Equal(7, @org_value))))
                    {
                        if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@value))))
                        {
                            return null;
                        }
                        else
                        {
                            if (P.Truth((P.Truth((P.Equal(2, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@value))) && P.Truth(((P.Number(@max_value) <= P.Number(5.5)))))))
                            {
                                if (P.Truth(((P.Number(@max_value) < P.Number(5.5)))))
                                {
                                    @max_value = P.Number(5.5);
                                    @max_dist = P.Int(@current_dist);
                                }
                                else
                                {
                                    if (P.Truth(((P.Number(@current_dist) < P.Number(@max_dist)))))
                                    {
                                        @max_dist = P.Int(@current_dist);
                                    }
                                }
                                return null;
                            }
                            else
                            {
                                if (P.Truth(((P.Number(@max_value) <= P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@value)))))
                                {
                                    if (P.Truth(((P.Number(@max_value) < P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@value)))))
                                    {
                                        @max_value = P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@value);
                                        @max_dist = P.Int(@current_dist);
                                    }
                                    else
                                    {
                                        if (P.Truth(((P.Number(@current_dist) < P.Number(@max_dist)))))
                                        {
                                            @max_dist = P.Int(@current_dist);
                                        }
                                    }
                                    return null;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (P.Truth((P.Equal(1, @org_value))))
                        {
                            if (P.Truth((P.Equal(7, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@value))))
                            {
                                if (P.Truth((!P.Equal(@max_value, 9))))
                                {
                                    @max_value = P.Number(9);
                                    @max_dist = P.Int(@current_dist);
                                }
                                else
                                {
                                    if (P.Truth(((P.Number(@current_dist) < P.Number(@max_dist)))))
                                    {
                                        @max_dist = P.Int(@current_dist);
                                    }
                                }
                                return null;
                            }
                            else
                            {
                                if (P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@value, 1))))
                                {
                                    if (P.Truth((!P.Equal(@max_value, 1))))
                                    {
                                        @max_value = P.Number(1);
                                        @max_dist = P.Int(@current_dist);
                                    }
                                    else
                                    {
                                        if (P.Truth(((P.Number(@current_dist) < P.Number(@max_dist)))))
                                        {
                                            @max_dist = P.Int(@current_dist);
                                        }
                                    }
                                    return null;
                                }
                            }
                        }
                        else
                        {
                            if (P.Truth((P.Truth((P.Equal(2, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@value))) && P.Truth(((P.Number(@org_value) > P.Number(2)))) && P.Truth(((P.Number(@max_value) <= P.Number(5.5)))))))
                            {
                                if (P.Truth(((P.Number(@max_value) < P.Number(5.5)))))
                                {
                                    @max_value = P.Number(5.5);
                                    @max_dist = P.Int(@current_dist);
                                }
                                else
                                {
                                    if (P.Truth(((P.Number(@current_dist) < P.Number(@max_dist)))))
                                    {
                                        @max_dist = P.Int(@current_dist);
                                    }
                                }
                                return null;
                            }
                            else
                            {
                                if (P.Truth((P.Truth(((P.Number(@max_value) <= P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@value)))) && P.Truth(((P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@value) <= P.Number(@org_value)))))))
                                {
                                    if (P.Truth(((P.Number(@max_value) < P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@value)))))
                                    {
                                        @max_value = P.Number(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @i), @j), 0)), P.Get(P.Get(P.Get(@a_map, @i), @j), 1))).@value);
                                        @max_dist = P.Int(@current_dist);
                                    }
                                    else
                                    {
                                        if (P.Truth(((P.Number(@current_dist) < P.Number(@max_dist)))))
                                        {
                                            @max_dist = P.Int(@current_dist);
                                        }
                                    }
                                    return null;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (P.Truth((P.Truth((P.Equal(@orgy, @desty))) && P.Truth((P.Equal(P.Add(@orgx, 1), @destx))))))
                {
                    @first_move_max_value(@orgx, @orgy, @destx, @desty, @my_chess, @a_map, @org_value, @owner_color, @i, P.Add(@j, 1), P.Int(P.Add(@dist, 1)));
                    @first_move_max_value(@orgx, @orgy, @destx, @desty, @my_chess, @a_map, @org_value, @owner_color, @i, P.Sub(@j, 1), P.Int(P.Add(@dist, 1)));
                    @first_move_max_value(@orgx, @orgy, @destx, @desty, @my_chess, @a_map, @org_value, @owner_color, P.Add(@i, 1), @j, P.Int(P.Add(@dist, 1)));
                    @first_move_max_value(@orgx, @orgy, @destx, @desty, @my_chess, @a_map, @org_value, @owner_color, P.Sub(@i, 1), @j, P.Int(P.Add(@dist, 1)));
                }
                else
                {
                    if (P.Truth((P.Truth((P.Equal(@orgy, @desty))) && P.Truth((P.Equal(P.Sub(@orgx, 1), @destx))))))
                    {
                        @first_move_max_value(@orgx, @orgy, @destx, @desty, @my_chess, @a_map, @org_value, @owner_color, @i, P.Sub(@j, 1), P.Int(P.Add(@dist, 1)));
                        @first_move_max_value(@orgx, @orgy, @destx, @desty, @my_chess, @a_map, @org_value, @owner_color, @i, P.Add(@j, 1), P.Int(P.Add(@dist, 1)));
                        @first_move_max_value(@orgx, @orgy, @destx, @desty, @my_chess, @a_map, @org_value, @owner_color, P.Add(@i, 1), @j, P.Int(P.Add(@dist, 1)));
                        @first_move_max_value(@orgx, @orgy, @destx, @desty, @my_chess, @a_map, @org_value, @owner_color, P.Sub(@i, 1), @j, P.Int(P.Add(@dist, 1)));
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(P.Add(@orgy, 1), @desty))) && P.Truth((P.Equal(@orgx, @destx))))))
                        {
                            @first_move_max_value(@orgx, @orgy, @destx, @desty, @my_chess, @a_map, @org_value, @owner_color, P.Add(@i, 1), @j, P.Int(P.Add(@dist, 1)));
                            @first_move_max_value(@orgx, @orgy, @destx, @desty, @my_chess, @a_map, @org_value, @owner_color, P.Sub(@i, 1), @j, P.Int(P.Add(@dist, 1)));
                            @first_move_max_value(@orgx, @orgy, @destx, @desty, @my_chess, @a_map, @org_value, @owner_color, @i, P.Add(@j, 1), P.Int(P.Add(@dist, 1)));
                            @first_move_max_value(@orgx, @orgy, @destx, @desty, @my_chess, @a_map, @org_value, @owner_color, @i, P.Sub(@j, 1), P.Int(P.Add(@dist, 1)));
                        }
                        else
                        {
                            if (P.Truth((P.Truth((P.Equal(P.Sub(@orgy, 1), @desty))) && P.Truth((P.Equal(@orgx, @destx))))))
                            {
                                @first_move_max_value(@orgx, @orgy, @destx, @desty, @my_chess, @a_map, @org_value, @owner_color, P.Sub(@i, 1), @j, P.Int(P.Add(@dist, 1)));
                                @first_move_max_value(@orgx, @orgy, @destx, @desty, @my_chess, @a_map, @org_value, @owner_color, P.Add(@i, 1), @j, P.Int(P.Add(@dist, 1)));
                                @first_move_max_value(@orgx, @orgy, @destx, @desty, @my_chess, @a_map, @org_value, @owner_color, @i, P.Add(@j, 1), P.Int(P.Add(@dist, 1)));
                                @first_move_max_value(@orgx, @orgy, @destx, @desty, @my_chess, @a_map, @org_value, @owner_color, @i, P.Sub(@j, 1), P.Int(P.Add(@dist, 1)));
                            }
                        }
                    }
                }
            }
            return null;
        }
        // Original darkchess.pyx: 1175
        public object @caca(object @org, object @dest, object @my_chess, object @a_map, object @owner_color)
        {
            object @destx = null;
            object @desty = null;
            object @eat_value = null;
            object @m = null;
            object @mc = null;
            object @n = null;
            object @orgx = null;
            object @orgy = null;
            @owner_color = P.Int(@owner_color);
            if (P.Truth((P.Equal(@org, null))))
            {
                return P.Int(0);
            }
            else
            {
                if (P.Truth((P.Equal(@owner_color, @player_color))))
                {
                    return P.Int(0);
                }
            }
            object __t73 = @org;
            @orgy = P.Get(__t73, 0);
            @orgx = P.Get(__t73, 1);
            object __t74 = @dest;
            @desty = P.Get(__t74, 0);
            @destx = P.Get(__t74, 1);
            @m = P.Get(P.Get(@a_map, @orgy), @orgx);
            if (P.Truth((P.Equal(@m, null))))
            {
                return P.Int(0);
            }
            else
            {
                if (P.Truth((P.Equal(2, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))))
                {
                    return P.Int(0);
                }
            }
            if (P.Truth((P.Truth(((P.Number(P.Sub(@desty, 1)) >= P.Number(0)))) && P.Truth(((P.Number(P.Sub(@destx, 1)) >= P.Number(0)))))))
            {
                @n = P.Get(P.Get(@a_map, P.Sub(@desty, 1)), P.Sub(@destx, 1));
                if (P.Truth((P.Equal(@n, null))))
                {
                }
                else
                {
                    @mc = P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1));
                    if (P.Truth((P.Truth((P.Equal(0, ((Piece)@mc).@live))) || P.Truth((P.Equal(1, ((Piece)@mc).@back))))))
                    {
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(7, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))) && P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value))) && P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))))
                        {
                        }
                        else
                        {
                            if (P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))
                            {
                                @eat_value = @can_be_ate_equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value);
                                if (P.Truth((P.Equal(1, @eat_value))))
                                {
                                    return P.Int(1);
                                }
                                else
                                {
                                    if (P.Truth((P.Equal(2, @eat_value))))
                                    {
                                        return P.Int(2);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (P.Truth((P.Truth(((P.Number(P.Sub(@desty, 1)) >= P.Number(0)))) && P.Truth(((P.Number(P.Add(@destx, 1)) <= P.Number(7)))))))
            {
                @n = P.Get(P.Get(@a_map, P.Sub(@desty, 1)), P.Add(@destx, 1));
                if (P.Truth((P.Equal(@n, null))))
                {
                }
                else
                {
                    @mc = P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1));
                    if (P.Truth((P.Truth((P.Equal(0, ((Piece)@mc).@live))) || P.Truth((P.Equal(1, ((Piece)@mc).@back))))))
                    {
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(7, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))) && P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value))) && P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))))
                        {
                        }
                        else
                        {
                            if (P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))
                            {
                                @eat_value = @can_be_ate_equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value);
                                if (P.Truth((P.Equal(1, @eat_value))))
                                {
                                    return P.Int(1);
                                }
                                else
                                {
                                    if (P.Truth((P.Equal(2, @eat_value))))
                                    {
                                        return P.Int(2);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (P.Truth((P.Truth(((P.Number(P.Add(@desty, 1)) <= P.Number(3)))) && P.Truth(((P.Number(P.Sub(@destx, 1)) >= P.Number(0)))))))
            {
                @n = P.Get(P.Get(@a_map, P.Add(@desty, 1)), P.Sub(@destx, 1));
                if (P.Truth((P.Equal(@n, null))))
                {
                }
                else
                {
                    @mc = P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1));
                    if (P.Truth((P.Truth((P.Equal(0, ((Piece)@mc).@live))) || P.Truth((P.Equal(1, ((Piece)@mc).@back))))))
                    {
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(7, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))) && P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value))) && P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))))
                        {
                        }
                        else
                        {
                            if (P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))
                            {
                                @eat_value = @can_be_ate_equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value);
                                if (P.Truth((P.Equal(1, @eat_value))))
                                {
                                    return P.Int(1);
                                }
                                else
                                {
                                    if (P.Truth((P.Equal(2, @eat_value))))
                                    {
                                        return P.Int(2);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (P.Truth((P.Truth(((P.Number(P.Add(@desty, 1)) <= P.Number(3)))) && P.Truth(((P.Number(P.Add(@destx, 1)) <= P.Number(7)))))))
            {
                @n = P.Get(P.Get(@a_map, P.Add(@desty, 1)), P.Add(@destx, 1));
                if (P.Truth((P.Equal(@n, null))))
                {
                }
                else
                {
                    @mc = P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1));
                    if (P.Truth((P.Truth((P.Equal(0, ((Piece)@mc).@live))) || P.Truth((P.Equal(1, ((Piece)@mc).@back))))))
                    {
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(7, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))) && P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value))) && P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))))
                        {
                        }
                        else
                        {
                            if (P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))
                            {
                                @eat_value = @can_be_ate_equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value);
                                if (P.Truth((P.Equal(1, @eat_value))))
                                {
                                    return P.Int(1);
                                }
                                else
                                {
                                    if (P.Truth((P.Equal(2, @eat_value))))
                                    {
                                        return P.Int(2);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (P.Truth(((P.Number(P.Sub(@desty, 2)) >= P.Number(0)))))
            {
                @n = P.Get(P.Get(@a_map, P.Sub(@desty, 2)), @destx);
                if (P.Truth((P.Equal(@n, null))))
                {
                }
                else
                {
                    @mc = P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1));
                    if (P.Truth((P.Truth((P.Equal(0, ((Piece)@mc).@live))) || P.Truth((P.Equal(1, ((Piece)@mc).@back))))))
                    {
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(7, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))) && P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value))) && P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))))
                        {
                        }
                        else
                        {
                            if (P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))
                            {
                                @eat_value = @can_be_ate_equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value);
                                if (P.Truth((P.Equal(2, @eat_value))))
                                {
                                    return P.Int(3);
                                }
                            }
                        }
                    }
                }
            }
            if (P.Truth(((P.Number(P.Add(@desty, 2)) <= P.Number(3)))))
            {
                @n = P.Get(P.Get(@a_map, P.Add(@desty, 2)), @destx);
                if (P.Truth((P.Equal(@n, null))))
                {
                }
                else
                {
                    @mc = P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1));
                    if (P.Truth((P.Truth((P.Equal(0, ((Piece)@mc).@live))) || P.Truth((P.Equal(1, ((Piece)@mc).@back))))))
                    {
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(7, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))) && P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value))) && P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))))
                        {
                        }
                        else
                        {
                            if (P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))
                            {
                                @eat_value = @can_be_ate_equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value);
                                if (P.Truth((P.Equal(2, @eat_value))))
                                {
                                    return P.Int(3);
                                }
                            }
                        }
                    }
                }
            }
            if (P.Truth(((P.Number(P.Sub(@destx, 2)) >= P.Number(0)))))
            {
                @n = P.Get(P.Get(@a_map, @desty), P.Sub(@destx, 2));
                if (P.Truth((P.Equal(@n, null))))
                {
                }
                else
                {
                    @mc = P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1));
                    if (P.Truth((P.Truth((P.Equal(0, ((Piece)@mc).@live))) || P.Truth((P.Equal(1, ((Piece)@mc).@back))))))
                    {
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(7, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))) && P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value))) && P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))))
                        {
                        }
                        else
                        {
                            if (P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))
                            {
                                @eat_value = @can_be_ate_equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value);
                                if (P.Truth((P.Equal(2, @eat_value))))
                                {
                                    return P.Int(3);
                                }
                            }
                        }
                    }
                }
            }
            if (P.Truth(((P.Number(P.Add(@destx, 2)) <= P.Number(7)))))
            {
                @n = P.Get(P.Get(@a_map, @desty), P.Add(@destx, 2));
                if (P.Truth((P.Equal(@n, null))))
                {
                }
                else
                {
                    @mc = P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1));
                    if (P.Truth((P.Truth((P.Equal(0, ((Piece)@mc).@live))) || P.Truth((P.Equal(1, ((Piece)@mc).@back))))))
                    {
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(7, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))) && P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value))) && P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))))
                        {
                        }
                        else
                        {
                            if (P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))
                            {
                                @eat_value = @can_be_ate_equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value);
                                if (P.Truth((P.Equal(2, @eat_value))))
                                {
                                    return P.Int(3);
                                }
                            }
                        }
                    }
                }
            }
            return P.Int(0);
            return 0;
        }
        // Original darkchess.pyx: 1315
        public object @near2_have_same_value(object @org, object @my_chess, object @a_map, object @owner_color)
        {
            object @m = null;
            object @mc = null;
            object @n = null;
            object @orgx = null;
            object @orgy = null;
            @owner_color = P.Int(@owner_color);
            if (P.Truth((P.Equal(@org, null))))
            {
                return P.Int(0);
            }
            else
            {
                if (P.Truth((P.Equal(@owner_color, @player_color))))
                {
                    return P.Int(0);
                }
            }
            object __t75 = @org;
            @orgy = P.Get(__t75, 0);
            @orgx = P.Get(__t75, 1);
            @m = P.Get(P.Get(@a_map, @orgy), @orgx);
            if (P.Truth((P.Equal(@m, null))))
            {
                return P.Int(0);
            }
            else
            {
                if (P.Truth((P.Equal(2, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))))
                {
                    return P.Int(0);
                }
            }
            if (P.Truth(((P.Number(P.Sub(@orgy, 2)) >= P.Number(0)))))
            {
                @n = P.Get(P.Get(@a_map, P.Sub(@orgy, 2)), @orgx);
                if (P.Truth((P.Equal(@n, null))))
                {
                }
                else
                {
                    @mc = P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1));
                    if (P.Truth((P.Truth((P.Equal(0, ((Piece)@mc).@live))) || P.Truth((P.Equal(1, ((Piece)@mc).@back))))))
                    {
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(7, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))) && P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value))) && P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))))
                        {
                        }
                        else
                        {
                            if (P.Truth((P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))) && P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))))))
                            {
                                return P.Int(1);
                            }
                        }
                    }
                }
            }
            if (P.Truth(((P.Number(P.Add(@orgy, 2)) <= P.Number(3)))))
            {
                @n = P.Get(P.Get(@a_map, P.Add(@orgy, 2)), @orgx);
                if (P.Truth((P.Equal(@n, null))))
                {
                }
                else
                {
                    @mc = P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1));
                    if (P.Truth((P.Truth((P.Equal(0, ((Piece)@mc).@live))) || P.Truth((P.Equal(1, ((Piece)@mc).@back))))))
                    {
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(7, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))) && P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value))) && P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))))
                        {
                        }
                        else
                        {
                            if (P.Truth((P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))) && P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))))))
                            {
                                return P.Int(1);
                            }
                        }
                    }
                }
            }
            if (P.Truth(((P.Number(P.Sub(@orgx, 2)) >= P.Number(0)))))
            {
                @n = P.Get(P.Get(@a_map, @orgy), P.Sub(@orgx, 2));
                if (P.Truth((P.Equal(@n, null))))
                {
                }
                else
                {
                    @mc = P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1));
                    if (P.Truth((P.Truth((P.Equal(0, ((Piece)@mc).@live))) || P.Truth((P.Equal(1, ((Piece)@mc).@back))))))
                    {
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(7, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))) && P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value))) && P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))))
                        {
                        }
                        else
                        {
                            if (P.Truth((P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))) && P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))))))
                            {
                                return P.Int(1);
                            }
                        }
                    }
                }
            }
            if (P.Truth(((P.Number(P.Add(@orgx, 2)) <= P.Number(7)))))
            {
                @n = P.Get(P.Get(@a_map, @orgy), P.Add(@orgx, 2));
                if (P.Truth((P.Equal(@n, null))))
                {
                }
                else
                {
                    @mc = P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1));
                    if (P.Truth((P.Truth((P.Equal(0, ((Piece)@mc).@live))) || P.Truth((P.Equal(1, ((Piece)@mc).@back))))))
                    {
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(7, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))) && P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value))) && P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))))
                        {
                        }
                        else
                        {
                            if (P.Truth((P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))) && P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))))))
                            {
                                return P.Int(1);
                            }
                        }
                    }
                }
            }
            if (P.Truth((P.Truth(((P.Number(P.Sub(@orgy, 1)) >= P.Number(0)))) && P.Truth(((P.Number(P.Sub(@orgx, 1)) >= P.Number(0)))))))
            {
                @n = P.Get(P.Get(@a_map, P.Sub(@orgy, 1)), P.Sub(@orgx, 1));
                if (P.Truth((P.Equal(@n, null))))
                {
                }
                else
                {
                    @mc = P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1));
                    if (P.Truth((P.Truth((P.Equal(0, ((Piece)@mc).@live))) || P.Truth((P.Equal(1, ((Piece)@mc).@back))))))
                    {
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(7, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))) && P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value))) && P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))))
                        {
                        }
                        else
                        {
                            if (P.Truth((P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))) && P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))))))
                            {
                                return P.Int(1);
                            }
                        }
                    }
                }
            }
            if (P.Truth((P.Truth(((P.Number(P.Sub(@orgy, 1)) >= P.Number(0)))) && P.Truth(((P.Number(P.Add(@orgx, 1)) <= P.Number(7)))))))
            {
                @n = P.Get(P.Get(@a_map, P.Sub(@orgy, 1)), P.Add(@orgx, 1));
                if (P.Truth((P.Equal(@n, null))))
                {
                }
                else
                {
                    @mc = P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1));
                    if (P.Truth((P.Truth((P.Equal(0, ((Piece)@mc).@live))) || P.Truth((P.Equal(1, ((Piece)@mc).@back))))))
                    {
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(7, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))) && P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value))) && P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))))
                        {
                        }
                        else
                        {
                            if (P.Truth((P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))) && P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))))))
                            {
                                return P.Int(1);
                            }
                        }
                    }
                }
            }
            if (P.Truth((P.Truth(((P.Number(P.Add(@orgy, 1)) <= P.Number(3)))) && P.Truth(((P.Number(P.Sub(@orgx, 1)) >= P.Number(0)))))))
            {
                @n = P.Get(P.Get(@a_map, P.Add(@orgy, 1)), P.Sub(@orgx, 1));
                if (P.Truth((P.Equal(@n, null))))
                {
                }
                else
                {
                    @mc = P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1));
                    if (P.Truth((P.Truth((P.Equal(0, ((Piece)@mc).@live))) || P.Truth((P.Equal(1, ((Piece)@mc).@back))))))
                    {
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(7, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))) && P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value))) && P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))))
                        {
                        }
                        else
                        {
                            if (P.Truth((P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))) && P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))))))
                            {
                                return P.Int(1);
                            }
                        }
                    }
                }
            }
            if (P.Truth((P.Truth(((P.Number(P.Add(@orgy, 1)) <= P.Number(3)))) && P.Truth(((P.Number(P.Add(@orgx, 1)) <= P.Number(7)))))))
            {
                @n = P.Get(P.Get(@a_map, P.Add(@orgy, 1)), P.Add(@orgx, 1));
                if (P.Truth((P.Equal(@n, null))))
                {
                }
                else
                {
                    @mc = P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1));
                    if (P.Truth((P.Truth((P.Equal(0, ((Piece)@mc).@live))) || P.Truth((P.Equal(1, ((Piece)@mc).@back))))))
                    {
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(7, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))) && P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value))) && P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))))))
                        {
                        }
                        else
                        {
                            if (P.Truth((P.Truth((!P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@color))) && P.Truth((P.Equal(((Piece)P.Get(P.Get(@my_chess, P.Get(@n, 0)), P.Get(@n, 1))).@value, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))))))
                            {
                                return P.Int(1);
                            }
                        }
                    }
                }
            }
            return P.Int(0);
            return 0;
        }
        // Original darkchess.pyx: 1443
        public object @scan_king(object @my_chess)
        {
            object @ch = null;
            object @chr = null;
            foreach (object __t76 in P.Iter(@my_chess))
            {
                @chr = __t76;
                foreach (object __t77 in P.Iter(@chr))
                {
                    @ch = __t77;
                    if (P.Truth((P.Equal(7, ((Piece)@ch).@value))))
                    {
                        P.Set(@king_live, ((Piece)@ch).@color, ((Piece)@ch).@live);
                    }
                }
            }
            return null;
        }
        // Original darkchess.pyx: 1451
        public object @in_com_possible_move(object @org, object @possible_mv)
        {
            if (P.Truth((P.Contains(@possible_mv, @org))))
            {
                return P.Int(true);
            }
            return P.Int(false);
            return 0;
        }
        // Original darkchess.pyx: 1460
        public object @save_step_and_break_long_capture(object @org, object @dest, object @a_map, object @my_chess)
        {
            object @b = null;
            object @br = null;
            object @col = null;
            object @dt = null;
            object @i = null;
            object @j = null;
            object @o = null;
            object @possible_mv = null;
            object @row = null;
            if (P.Truth((!P.Equal(@org, @dest))))
            {
                @possible_mv = @collect_possible_move(P.Get(@dest, 0), P.Get(@dest, 1), @a_map, @my_chess);
                P.Set(@move_step, @sindex, P.L(@com_color, @org, @dest, @possible_mv));
                @sindex = P.Int(P.Mod(P.Add(@sindex, 1), 4));
                object __t78 = @dest;
                @row = P.Get(__t78, 0);
                @col = P.Get(__t78, 1);
                foreach (object __t79 in P.Iter(P.Range(P.Neg(2), 3)))
                {
                    @i = __t79;
                    foreach (object __t80 in P.Iter(P.Range(P.Neg(2), 3)))
                    {
                        @j = __t80;
                        if (P.Truth((P.Truth(((P.Number(P.Add(P.Abs(@i), P.Abs(@j))) <= P.Number(2)))) && P.Truth(((P.Number(0) <= P.Number(P.Add(@row, @i))) && (P.Number(P.Add(@row, @i)) <= P.Number(3)))) && P.Truth(((P.Number(0) <= P.Number(P.Add(@col, @j))) && (P.Number(P.Add(@col, @j)) <= P.Number(7)))))))
                        {
                            @br = 0;
                            @dt = P.L(P.Add(@row, @i), P.Add(@col, @j));
                            while (P.Truth(((P.Number(@br) < P.Number(P.Len(@break_long_capture_dest))))))
                            {
                                @b = 0;
                                if (P.Truth((P.Contains(P.Get(@break_long_capture_dest, @br), @dt))))
                                {
                                    P.Delete(@break_long_capture_dest, @br);
                                    P.Delete(@break_long_capture_org, @br);
                                    P.Delete(@com_ban_step, @br);
                                    @b = 1;
                                }
                                if (P.Truth((P.Equal(0, @b))))
                                {
                                    @br = P.Add(@br, 1);
                                }
                            }
                        }
                    }
                }
                @br = 0;
                while (P.Truth(((P.Number(@br) < P.Number(P.Len(@break_long_capture_org))))))
                {
                    @b = 0;
                    foreach (object __t81 in P.Iter(P.Get(@break_long_capture_org, @br)))
                    {
                        @o = __t81;
                        if (P.Truth((P.Equal(@org, @o))))
                        {
                            P.Delete(@break_long_capture_dest, @br);
                            P.Delete(@break_long_capture_org, @br);
                            P.Delete(@com_ban_step, @br);
                            @b = 1;
                            break;
                        }
                    }
                    if (P.Truth((P.Equal(0, @b))))
                    {
                        @br = P.Add(@br, 1);
                    }
                }
            }
            return null;
        }
        // Original darkchess.pyx: 1516
        public object @calc_cannon_mark(object @my_chess, object @a_map, object @owner_color)
        {
            object @c = null;
            object @cannon_mark = null;
            object @find_player_cannon_num = null;
            object @i = null;
            object @ii = null;
            object @j = null;
            object @jj = null;
            object @jump = null;
            object @r = null;
            @owner_color = P.Int(@owner_color);
            @cannon_mark = P.L(P.Mul(P.L(0), 8), P.Mul(P.L(0), 8), P.Mul(P.L(0), 8), P.Mul(P.L(0), 8));
            @find_player_cannon_num = 0;
            @jump = 0;
            if (P.Truth((P.Equal(@owner_color, @player_color))))
            {
                return @cannon_mark;
            }
            foreach (object __t82 in P.Iter(P.Range(0, 4)))
            {
                @r = __t82;
                foreach (object __t83 in P.Iter(P.Range(0, 8)))
                {
                    @c = __t83;
                    if (P.Truth((P.Equal(2, @find_player_cannon_num))))
                    {
                        break;
                    }
                    if (P.Truth((P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, @r), @c)).@live))) && P.Truth((P.Equal(0, ((Piece)P.Get(P.Get(@my_chess, @r), @c)).@back))) && P.Truth((P.Equal(@player_color, ((Piece)P.Get(P.Get(@my_chess, @r), @c)).@color))) && P.Truth((P.Equal(2, ((Piece)P.Get(P.Get(@my_chess, @r), @c)).@value))))))
                    {
                        @find_player_cannon_num = P.Add(@find_player_cannon_num, 1);
                        object __t84 = P.L(((Piece)P.Get(P.Get(@my_chess, @r), @c)).@row, ((Piece)P.Get(P.Get(@my_chess, @r), @c)).@col);
                        @i = P.Get(__t84, 0);
                        @j = P.Get(__t84, 1);
                        foreach (object __t85 in P.Iter(P.Range(P.Sub(@i, 1), P.Neg(1), P.Neg(1))))
                        {
                            @ii = __t85;
                            if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))))
                            {
                                break;
                            }
                            else
                            {
                                if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((P.Equal(null, P.Get(P.Get(@a_map, @ii), @j)))))))
                                {
                                    P.Set(P.Get(@cannon_mark, @ii), @j, 1);
                                }
                                else
                                {
                                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))
                                    {
                                        @jump = 1;
                                    }
                                }
                            }
                        }
                        @jump = 0;
                        foreach (object __t86 in P.Iter(P.Range(P.Add(@i, 1), 4, 1)))
                        {
                            @ii = __t86;
                            if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))))
                            {
                                break;
                            }
                            else
                            {
                                if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((P.Equal(null, P.Get(P.Get(@a_map, @ii), @j)))))))
                                {
                                    P.Set(P.Get(@cannon_mark, @ii), @j, 1);
                                }
                                else
                                {
                                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @ii), @j), null))))
                                    {
                                        @jump = 1;
                                    }
                                }
                            }
                        }
                        @jump = 0;
                        foreach (object __t87 in P.Iter(P.Range(P.Sub(@j, 1), P.Neg(1), P.Neg(1))))
                        {
                            @jj = __t87;
                            if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))))
                            {
                                break;
                            }
                            else
                            {
                                if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((P.Equal(null, P.Get(P.Get(@a_map, @i), @jj)))))))
                                {
                                    P.Set(P.Get(@cannon_mark, @i), @jj, 1);
                                }
                                else
                                {
                                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))
                                    {
                                        @jump = 1;
                                    }
                                }
                            }
                        }
                        @jump = 0;
                        foreach (object __t88 in P.Iter(P.Range(P.Add(@j, 1), 8, 1)))
                        {
                            @jj = __t88;
                            if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))))
                            {
                                break;
                            }
                            else
                            {
                                if (P.Truth((P.Truth((P.Equal(1, @jump))) && P.Truth((P.Equal(null, P.Get(P.Get(@a_map, @i), @jj)))))))
                                {
                                    P.Set(P.Get(@cannon_mark, @i), @jj, 1);
                                }
                                else
                                {
                                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @i), @jj), null))))
                                    {
                                        @jump = 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return @cannon_mark;
            return null;
        }
        // Original darkchess.pyx: 1566
        public object @first_move_score(object @org, object @dest, object @my_chess, object @a_map, object @owner_color, object @player_color, object @com_color, object @com_ban_step, object @king_live)
        {
            object @a = null;
            object @af_ch = null;
            object @af_map = null;
            object @am = null;
            object @c = null;
            object @cannon = null;
            object @cvalue = null;
            object @destx = null;
            object @desty = null;
            object @mp = null;
            object @mvalue = null;
            object @nc = null;
            object @ncor = null;
            object @ndead = null;
            object @org_score = null;
            object @org_value = null;
            object @orgx = null;
            object @orgy = null;
            object @pm = null;
            object @pmx = null;
            object @pmy = null;
            object @small_value = null;
            @owner_color = P.Int(@owner_color);
            @com_color = P.Int(@com_color);
            @player_color = P.Int(@player_color);
            if (P.Truth((P.Truth((P.Equal(@org, @dest))) || P.Truth((P.Equal(null, @org))) || P.Truth((P.Equal(null, @dest))))))
            {
                return P.Number(0);
            }
            object __t89 = @org;
            @orgy = P.Get(__t89, 0);
            @orgx = P.Get(__t89, 1);
            object __t90 = @dest;
            @desty = P.Get(__t90, 0);
            @destx = P.Get(__t90, 1);
            if (P.Truth((P.Equal(P.Get(P.Get(@a_map, @desty), @destx), null))))
            {
                if (P.Truth((P.Contains(@com_ban_step, @org))))
                {
                    return P.Number(P.Neg(10.2));
                }
                @ndead = @escape_way_to_run(@org, @dest, @my_chess, @a_map, @owner_color);
                if (P.Truth((P.Equal(@owner_color, @player_color))))
                {
                    if (P.Truth((P.Equal(1, @will_eat2_more(@org, @dest, @my_chess, @a_map, @owner_color)))))
                    {
                        return P.Number(8);
                    }
                    return P.Number(0);
                }
                else
                {
                    if (P.Truth((P.Truth((P.Equal(0, @dest_will_dead_owner_wont_eat(@org, @dest, @main_chess, @main_map, @player_color)))) && P.Truth((P.Equal(1, @stand_will_dead_pity(P.L(@orgy, @orgx), @main_chess, @main_map, @com_color)))))))
                    {
                        return P.Number(P.Add(9, @ndead));
                    }
                }
                if (P.Truth((P.Equal(2, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @orgy), @orgx), 0)), P.Get(P.Get(P.Get(@a_map, @orgy), @orgx), 1))).@value))))
                {
                    @af_map = P.DeepCopy(@a_map);
                    @af_ch = P.DeepCopy(@my_chess);
                    if (P.Truth((P.Truth((!P.Equal(@org, null))) && P.Truth((!P.Equal(@dest, null))))))
                    {
                        object __t91 = @move(@org, @dest, @af_map, @af_ch);
                        @af_map = P.Get(__t91, 0);
                        @af_ch = P.Get(__t91, 1);
                        @all_chess_move(@af_map, @af_ch);
                        @cannon = P.Get(P.Get(@af_ch, P.Get(P.Get(P.Get(@af_map, P.Get(@dest, 0)), P.Get(@dest, 1)), 0)), P.Get(P.Get(P.Get(@af_map, P.Get(@dest, 0)), P.Get(@dest, 1)), 1));
                        foreach (object __t92 in P.Iter(((Piece)@cannon).@possible_move))
                        {
                            @pm = __t92;
                            object __t93 = @pm;
                            @pmy = P.Get(__t93, 0);
                            @pmx = P.Get(__t93, 1);
                            @am = P.Get(P.Get(@af_map, @pmy), @pmx);
                            if (P.Truth((P.Equal(null, @am))))
                            {
                                continue;
                            }
                            @c = P.Get(P.Get(@af_ch, P.Get(@am, 0)), P.Get(@am, 1));
                            if (P.Truth(((P.Number(((Piece)@c).@value) > P.Number(5)))))
                            {
                                return P.Number(7.3);
                            }
                        }
                    }
                    return P.Number(0);
                }
                @max_value = P.Number(0);
                @max_dist = P.Int(32);
                P.Replace(P.Get(@mark, 0), P.L(0, 0, 0, 0, 0, 0, 0, 0));
                P.Replace(P.Get(@mark, 1), P.L(0, 0, 0, 0, 0, 0, 0, 0));
                P.Replace(P.Get(@mark, 2), P.L(0, 0, 0, 0, 0, 0, 0, 0));
                P.Replace(P.Get(@mark, 3), P.L(0, 0, 0, 0, 0, 0, 0, 0));
                @org_value = ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @orgy), @orgx), 0)), P.Get(P.Get(P.Get(@a_map, @orgy), @orgx), 1))).@value;
                @mvalue = 0;
                if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @orgy), @orgx), null))))
                {
                    @mp = P.Get(P.Get(@a_map, @orgy), @orgx);
                    @mvalue = ((Piece)P.Get(P.Get(@my_chess, P.Get(@mp, 0)), P.Get(@mp, 1))).@value;
                }
                @cannon_mark = @calc_cannon_mark(@my_chess, @a_map, @owner_color);
                @first_move_max_value(@orgx, @orgy, @destx, @desty, @my_chess, @a_map, @org_value, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @orgy), @orgx), 0)), P.Get(P.Get(P.Get(@a_map, @orgy), @orgx), 1))).@color, @desty, @destx);
                @cvalue = @caca(@org, @dest, @my_chess, @a_map, @owner_color);
                if (P.Truth((P.Equal(1, @cvalue))))
                {
                    return P.Number(P.Add(@f_calc_move_score(@max_value, @max_dist, @mvalue), 0.1));
                }
                else
                {
                    if (P.Truth((P.Equal(2, @cvalue))))
                    {
                        return P.Number(P.Add(@f_calc_move_score(@max_value, @max_dist, @mvalue), 0.3));
                    }
                    else
                    {
                        if (P.Truth((P.Equal(3, @cvalue))))
                        {
                            return P.Number(P.Add(@f_calc_move_score(@max_value, @max_dist, @mvalue), 0.28));
                        }
                    }
                }
                if (P.Truth((P.Equal(1, @near2_have_same_value(@org, @my_chess, @a_map, @owner_color)))))
                {
                    if (P.Truth((P.Equal(0, @will_dead_pity_even_equal(@org, @dest, @my_chess, @a_map, @owner_color)))))
                    {
                        return P.Number(P.Neg(0.1));
                    }
                }
                @ncor = @near(@orgy, @orgx);
                foreach (object __t94 in P.Iter(@ncor))
                {
                    @nc = __t94;
                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, P.Get(@nc, 0)), P.Get(@nc, 1)), null))))
                    {
                        @a = P.Get(P.Get(@a_map, P.Get(@nc, 0)), P.Get(@nc, 1));
                        @small_value = ((Piece)P.Get(P.Get(@my_chess, P.Get(@a, 0)), P.Get(@a, 1))).@value;
                        if (P.Truth((P.Equal(1, ((Piece)P.Get(P.Get(@my_chess, P.Get(@a, 0)), P.Get(@a, 1))).@back))))
                        {
                            continue;
                        }
                        if (P.Truth((P.Truth((P.Equal(@player_color, ((Piece)P.Get(P.Get(@my_chess, P.Get(@a, 0)), P.Get(@a, 1))).@color))) && P.Truth((P.Equal(1, @can_be_ate(@small_value, @org_value)))))))
                        {
                            return P.Number(P.Neg(0.1));
                        }
                    }
                }
                return P.Number(P.Add(@f_calc_move_score(@max_value, @max_dist, @mvalue), @ndead));
            }
            else
            {
                @ndead = @escape_way_to_run(@org, @dest, @my_chess, @a_map, @owner_color);
                @org_score = @eating_value_to_score(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @desty), @destx), 0)), P.Get(P.Get(P.Get(@a_map, @desty), @destx), 1))).@value, @king_live, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @orgy), @orgx), 0)), P.Get(P.Get(P.Get(@a_map, @orgy), @orgx), 1))).@color);
                if (P.Truth((P.Equal(0, @ndead))))
                {
                    return P.Number(P.Add(@org_score, 10));
                }
                else
                {
                    return P.Number(@org_score);
                }
            }
            return 0;
        }
        // Original darkchess.pyx: 1676
        public object @move_score(object @org, object @dest, object @my_chess, object @a_map, object @owner_color, object @player_color, object @com_color, object @com_ban_step, object @king_live, int @step = 1)
        {
            object @af_ch = null;
            object @af_map = null;
            object @am = null;
            object @c = null;
            object @cannon = null;
            object @destx = null;
            object @desty = null;
            object @m = null;
            object @ndead = null;
            object @org_score = null;
            object @orgx = null;
            object @orgy = null;
            object @pm = null;
            object @pmx = null;
            object @pmy = null;
            @step = P.Int(@step);
            @owner_color = P.Int(@owner_color);
            @com_color = P.Int(@com_color);
            @player_color = P.Int(@player_color);
            if (P.Truth((P.Truth((P.Equal(@org, @dest))) || P.Truth((P.Equal(null, @org))) || P.Truth((P.Equal(null, @dest))))))
            {
                return P.Number(0);
            }
            object __t95 = @org;
            @orgy = P.Get(__t95, 0);
            @orgx = P.Get(__t95, 1);
            object __t96 = @dest;
            @desty = P.Get(__t96, 0);
            @destx = P.Get(__t96, 1);
            if (P.Truth((P.Equal(P.Get(P.Get(@a_map, @desty), @destx), null))))
            {
                if (P.Truth((P.Contains(@com_ban_step, @org))))
                {
                    return P.Number(P.Neg(10.2));
                }
                @ndead = @escape_way_to_run(@org, @dest, @my_chess, @a_map, @owner_color);
                if (P.Truth((P.Truth(((P.Number(@step) > P.Number(2)))) && P.Truth((P.Equal(0, @ndead))))))
                {
                    if (P.Truth((!P.Equal(P.Get(P.Get(@a_map, @orgy), @orgx), null))))
                    {
                        @m = P.Get(P.Get(@a_map, @orgy), @orgx);
                        if (P.Truth((P.Equal(3, ((Piece)P.Get(P.Get(@my_chess, P.Get(@m, 0)), P.Get(@m, 1))).@value))))
                        {
                            return P.Number(7);
                        }
                        else
                        {
                            return P.Number(10);
                        }
                    }
                }
                else
                {
                    if (P.Truth((P.Equal(@owner_color, @player_color))))
                    {
                        if (P.Truth((P.Equal(1, @will_eat2_more(@org, @dest, @my_chess, @a_map, @owner_color)))))
                        {
                            return P.Number(8);
                        }
                        return P.Number(0);
                    }
                    else
                    {
                        if (P.Truth((P.Truth((P.Equal(0, @dest_will_dead_owner_wont_eat(@org, @dest, @main_chess, @main_map, @player_color)))) && P.Truth((P.Equal(1, @stand_will_dead_pity(P.L(@orgy, @orgx), @main_chess, @main_map, @com_color)))))))
                        {
                            return P.Number(P.Add(9, @ndead));
                        }
                    }
                }
                if (P.Truth((P.Equal(2, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @orgy), @orgx), 0)), P.Get(P.Get(P.Get(@a_map, @orgy), @orgx), 1))).@value))))
                {
                    @af_map = P.DeepCopy(@a_map);
                    @af_ch = P.DeepCopy(@my_chess);
                    if (P.Truth((P.Truth((!P.Equal(@org, null))) && P.Truth((!P.Equal(@dest, null))))))
                    {
                        object __t97 = @move(@org, @dest, @af_map, @af_ch);
                        @af_map = P.Get(__t97, 0);
                        @af_ch = P.Get(__t97, 1);
                        @all_chess_move(@af_map, @af_ch);
                        @cannon = P.Get(P.Get(@af_ch, P.Get(P.Get(P.Get(@af_map, P.Get(@dest, 0)), P.Get(@dest, 1)), 0)), P.Get(P.Get(P.Get(@af_map, P.Get(@dest, 0)), P.Get(@dest, 1)), 1));
                        foreach (object __t98 in P.Iter(((Piece)@cannon).@possible_move))
                        {
                            @pm = __t98;
                            object __t99 = @pm;
                            @pmy = P.Get(__t99, 0);
                            @pmx = P.Get(__t99, 1);
                            @am = P.Get(P.Get(@af_map, @pmy), @pmx);
                            if (P.Truth((P.Equal(null, @am))))
                            {
                                continue;
                            }
                            @c = P.Get(P.Get(@af_ch, P.Get(@am, 0)), P.Get(@am, 1));
                            if (P.Truth(((P.Number(((Piece)@c).@value) > P.Number(5)))))
                            {
                                return P.Number(7.3);
                            }
                        }
                    }
                    return P.Number(0);
                }
                return P.Number(0);
            }
            else
            {
                @ndead = @escape_way_to_run(@org, @dest, @my_chess, @a_map, @owner_color);
                @org_score = @eating_value_to_score(((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @desty), @destx), 0)), P.Get(P.Get(P.Get(@a_map, @desty), @destx), 1))).@value, @king_live, ((Piece)P.Get(P.Get(@my_chess, P.Get(P.Get(P.Get(@a_map, @orgy), @orgx), 0)), P.Get(P.Get(P.Get(@a_map, @orgy), @orgx), 1))).@color);
                if (P.Truth((P.Equal(0, @ndead))))
                {
                    return P.Number(P.Add(@org_score, 10));
                }
                else
                {
                    return P.Number(@org_score);
                }
            }
            return 0;
        }
        // Original darkchess.pyx: 1740
        public object @move_s(object @org, object @dest, object @a_map, object @a_ch)
        {
            object @af_map = null;
            object @desti = null;
            object @destj = null;
            object @org_ch = null;
            object @orgi = null;
            object @orgj = null;
            object @sound_capture = null;
            object @sound_move = null;
            object __t100 = @org;
            @orgi = P.Get(__t100, 0);
            @orgj = P.Get(__t100, 1);
            object __t101 = @dest;
            @desti = P.Get(__t101, 0);
            @destj = P.Get(__t101, 1);
            @af_map = P.DeepCopy(@a_map);
            if (P.Truth((P.Equal(null, P.Get(P.Get(@a_map, @desti), @destj)))))
            {
                @org_ch = P.Get(P.Get(@a_ch, P.Get(P.Get(P.Get(@a_map, @orgi), @orgj), 0)), P.Get(P.Get(P.Get(@a_map, @orgi), @orgj), 1));
                object __t102 = P.L(@desti, @destj);
                ((Piece)@org_ch).@row = P.Int(P.Get(__t102, 0));
                ((Piece)@org_ch).@col = P.Int(P.Get(__t102, 1));
                @com_mv_map = P.CopyList(P.Get(P.Get(@a_map, @orgi), @orgj));
                P.Set(P.Get(@af_map, @desti), @destj, P.L(P.Get(P.CopyList(P.Get(P.Get(@a_map, @orgi), @orgj)), 0), P.Get(P.CopyList(P.Get(P.Get(@a_map, @orgi), @orgj)), 1)));
                P.Set(P.Get(@af_map, @orgi), @orgj, null);
                P.Set(P.Get(@a_map, @orgi), @orgj, null);
                @sound_move = NewSound(@s_move2);
                ((DisplaySound)@sound_move).@play();
            }
            else
            {
                @org_ch = P.Get(P.Get(@a_ch, P.Get(P.Get(P.Get(@a_map, @orgi), @orgj), 0)), P.Get(P.Get(P.Get(@a_map, @orgi), @orgj), 1));
                object __t103 = P.L(@desti, @destj);
                ((Piece)@org_ch).@row = P.Int(P.Get(__t103, 0));
                ((Piece)@org_ch).@col = P.Int(P.Get(__t103, 1));
                @com_mv_map = P.CopyList(P.Get(P.Get(@a_map, @orgi), @orgj));
                P.Set(P.Get(@af_map, @desti), @destj, P.L(P.Get(P.CopyList(P.Get(P.Get(@a_map, @orgi), @orgj)), 0), P.Get(P.CopyList(P.Get(P.Get(@a_map, @orgi), @orgj)), 1)));
                P.Set(P.Get(@af_map, @orgi), @orgj, null);
                P.Set(P.Get(@a_map, @orgi), @orgj, null);
                @sound_capture = NewSound(@s_capture);
                ((DisplaySound)@sound_capture).@play();
            }
            return P.L(@a_map, @a_ch, @af_map);
            return null;
        }
        // Original darkchess.pyx: 1777
        public object @move(object @org, object @dest, object @a_map, object @a_ch)
        {
            object @dest_ch = null;
            object @desti = null;
            object @destj = null;
            object @org_ch = null;
            object @orgi = null;
            object @orgj = null;
            if (P.Truth((P.Truth((P.Equal(@org, @dest))) || P.Truth((P.Equal(null, @org))) || P.Truth((P.Equal(null, @dest))))))
            {
                return P.L(@a_map, @a_ch);
            }
            object __t104 = @org;
            @orgi = P.Get(__t104, 0);
            @orgj = P.Get(__t104, 1);
            object __t105 = @dest;
            @desti = P.Get(__t105, 0);
            @destj = P.Get(__t105, 1);
            if (P.Truth((P.Equal(null, P.Get(P.Get(@a_map, @desti), @destj)))))
            {
                @org_ch = P.Get(P.Get(@a_ch, P.Get(P.Get(P.Get(@a_map, @orgi), @orgj), 0)), P.Get(P.Get(P.Get(@a_map, @orgi), @orgj), 1));
                object __t106 = P.L(@desti, @destj);
                ((Piece)@org_ch).@row = P.Int(P.Get(__t106, 0));
                ((Piece)@org_ch).@col = P.Int(P.Get(__t106, 1));
                P.Set(P.Get(@a_map, @desti), @destj, P.L(P.Get(P.CopyList(P.Get(P.Get(@a_map, @orgi), @orgj)), 0), P.Get(P.CopyList(P.Get(P.Get(@a_map, @orgi), @orgj)), 1)));
                P.Set(P.Get(@a_map, @orgi), @orgj, null);
            }
            else
            {
                @dest_ch = P.Get(P.Get(@a_ch, P.Get(P.Get(P.Get(@a_map, @desti), @destj), 0)), P.Get(P.Get(P.Get(@a_map, @desti), @destj), 1));
                @org_ch = P.Get(P.Get(@a_ch, P.Get(P.Get(P.Get(@a_map, @orgi), @orgj), 0)), P.Get(P.Get(P.Get(@a_map, @orgi), @orgj), 1));
                ((Piece)@dest_ch).@live = P.Int(0);
                object __t107 = P.L(@desti, @destj);
                ((Piece)@org_ch).@row = P.Int(P.Get(__t107, 0));
                ((Piece)@org_ch).@col = P.Int(P.Get(__t107, 1));
                P.Set(P.Get(@a_map, @desti), @destj, P.L(P.Get(P.CopyList(P.Get(P.Get(@a_map, @orgi), @orgj)), 0), P.Get(P.CopyList(P.Get(P.Get(@a_map, @orgi), @orgj)), 1)));
                P.Set(P.Get(@a_map, @orgi), @orgj, null);
            }
            return P.L(@a_map, @a_ch);
            return null;
        }
        // Original darkchess.pyx: 1803
        public object @cant_move(object @a_map, object @a_ch, object @owner_color)
        {
            object @ch = null;
            object @chr = null;
            @owner_color = P.Int(@owner_color);
            @all_chess_move(@a_map, @a_ch);
            foreach (object __t108 in P.Iter(@a_ch))
            {
                @chr = __t108;
                foreach (object __t109 in P.Iter(@chr))
                {
                    @ch = __t109;
                    if (P.Truth((P.Truth((P.Equal(((Piece)@ch).@color, @owner_color))) && P.Truth((P.Equal(1, ((Piece)@ch).@live))))))
                    {
                        if (P.Truth(((Piece)@ch).@possible_move))
                        {
                            return P.Int(0);
                        }
                    }
                }
            }
            return P.Int(1);
            return 0;
        }
        // Original darkchess.pyx: 1813
        public object @com_think(object @a_map, object @a_ch)
        {
            object @alpha = null;
            object @beta = null;
            object @ch = null;
            object @chr = null;
            object @dest = null;
            object @m = null;
            object @mf = null;
            object @min_index = null;
            object @min_score = null;
            object @org = null;
            object @pity = null;
            object @pm = null;
            object @result = null;
            object @sc = null;
            object @score = null;
            object @self_score = null;
            @m = P.L();
            @min_score = P.Number(9000);
            @sc = P.Number(0);
            @all_chess_move(@a_map, @a_ch);
            @scan_king(@a_ch);
            if (P.Truth(((P.Number(@back_num) > P.Number(0)))))
            {
                @open_score = 0.01;
                ((PList)@m).@append(P.L(null, null, 0.01, 0));
                @min_score = P.Number(0.01);
                @org = null;
                @dest = null;
            }
            else
            {
                @open_score = null;
            }
            foreach (object __t110 in P.Iter(@a_ch))
            {
                @chr = __t110;
                foreach (object __t111 in P.Iter(@chr))
                {
                    @ch = __t111;
                    if (P.Truth((P.Truth((P.Equal(1, ((Piece)@ch).@back))) || P.Truth((P.Equal(0, ((Piece)@ch).@live))))))
                    {
                        continue;
                    }
                    if (P.Truth((P.Equal(((Piece)@ch).@color, @com_color))))
                    {
                        foreach (object __t112 in P.Iter(((Piece)@ch).@possible_move))
                        {
                            @pm = __t112;
                            @pity = 0;
                            if (P.Truth((P.Equal(0, @will_dead_pity(P.L(((Piece)@ch).@row, ((Piece)@ch).@col), @pm, @a_ch, @a_map, @com_color)))))
                            {
                                @score = P.Sub(@sc, @first_move_score(P.L(((Piece)@ch).@row, ((Piece)@ch).@col), @pm, @a_ch, @a_map, @com_color, @player_color, @com_color, @com_ban_step, @king_live));
                            }
                            else
                            {
                                @self_score = P.Mul(@eating_value_to_score(((Piece)@ch).@value, @king_live, @com_color), 0.2);
                                @score = P.Sub(P.Add(P.Add(@sc, 40), @self_score), @first_move_score(P.L(((Piece)@ch).@row, ((Piece)@ch).@col), @pm, @a_ch, @a_map, @com_color, @player_color, @com_color, @com_ban_step, @king_live));
                                @pity = 1;
                            }
                            ((PList)@m).@append(P.L(P.L(((Piece)@ch).@row, ((Piece)@ch).@col), @pm, @score, @pity));
                            if (P.Truth(((P.Number(@score) < P.Number(@min_score)))))
                            {
                                @min_score = P.Number(@score);
                                @org = P.L(((Piece)@ch).@row, ((Piece)@ch).@col);
                                @dest = @pm;
                            }
                        }
                    }
                }
            }
            @alpha = @AI_min_score;
            @final_score = P.Number(@AI_min_score);
            @min_index = null;
            @beta = P.Mul(P.Neg(1), @AI_min_score);
            if (P.Truth(((P.Number(P.Len(@m)) > P.Number(1)))))
            {
                @mf = ParallelSearch(@m, @a_map, @a_ch, @alpha, @beta);
                @min_index = null;
                foreach (object __t113 in P.Iter(@mf))
                {
                    @result = __t113;
                    if (P.Truth((P.Equal(P.Get(@result, 0), P.Get(@result, 1)))))
                    {
                        @open_score = P.Get(@result, 2);
                    }
                    if (P.Truth(((P.Number(@final_score) > P.Number(P.Get(@result, 2))))))
                    {
                        @final_score = P.Number(P.Get(@result, 2));
                        @min_index = ((PList)@mf).@index(@result);
                    }
                }
                if (P.Truth(@mf))
                {
                    return P.L(P.Get(P.Get(@mf, @min_index), 0), P.Get(P.Get(@mf, @min_index), 1), P.Get(P.Get(@mf, @min_index), 2));
                }
                else
                {
                    return P.L(@org, @dest, @min_score);
                }
            }
            else
            {
                if (P.Truth((P.Equal(1, P.Len(@m)))))
                {
                    return P.L(P.Get(P.Get(@m, 0), 0), P.Get(P.Get(@m, 0), 1), P.Get(P.Get(@m, 0), 2));
                }
                else
                {
                    return P.L(null, null, 0);
                }
            }
            return null;
        }
        // Original darkchess.pyx: 1925
        public object @one_turn(object @q, object @a_map, object @a_ch, object @mm, object @owner_color, object @nexti, object @nextj, object @sc, object @pt, object @div, object @ind, object @alpha, object @beta, object @player_color, object @com_color, object @back_num, object @com_ban_step, object @king_live, object @gb_m2)
        {
            object @af_ch = null;
            object @af_ch_2 = null;
            object @af_ch_3 = null;
            object @af_map = null;
            object @af_map_2 = null;
            object @af_map_3 = null;
            object @all_pm = null;
            object @all_pm_2 = null;
            object @all_pm_3 = null;
            object @apm = null;
            object @apm_com = null;
            object @apm_p = null;
            object @ban = null;
            object @bomb_score = null;
            object @c_a = null;
            object @ch = null;
            object @ch_1 = null;
            object @ch_com = null;
            object @ch_comp = null;
            object @ch_p = null;
            object @ch_player = null;
            object @ch_position = null;
            object @ch_position2 = null;
            object @ch_position3 = null;
            object @chr = null;
            object @chr_com = null;
            object @chr_p = null;
            object @coms = null;
            object @div2 = null;
            object @event = null;
            object @m2 = null;
            object @m3 = null;
            object @m4 = null;
            object @max_index = null;
            object @max_p_score = null;
            object @min_index = null;
            object @mscore = null;
            object @p_a = null;
            object @pity = null;
            object @pm = null;
            object @pm_1 = null;
            object @pm_com = null;
            object @pm_comp = null;
            object @pm_p = null;
            object @pm_player = null;
            object @ps = null;
            object @score = null;
            object @score2 = null;
            object @score3 = null;
            @sc = P.Number(@sc);
            @ind = P.Int(@ind);
            @beta = P.Number(@beta);
            @div = P.Number(@div);
            @owner_color = P.Int(@owner_color);
            @alpha = P.Number(@alpha);
            @player_color = P.Int(@player_color);
            @back_num = P.Int(@back_num);
            @com_color = P.Int(@com_color);
            @pt = P.Int(@pt);
            @max_p_score = P.Number(P.Neg(9000));
            @div2 = 0.901;
            @m2 = P.L();
            @m3 = P.L();
            @m4 = P.L();
            foreach (object __t114 in P.Iter(@com_ban_step))
            {
                @ban = __t114;
                if (P.Truth((P.Equal(@nexti, @ban))))
                {
                    @pt = P.Int(1);
                }
            }
            @af_map = P.DeepCopy(@a_map);
            @af_ch = P.DeepCopy(@a_ch);
            if (P.Truth((P.Truth((!P.Equal(@nexti, null))) && P.Truth((!P.Equal(@nextj, null))))))
            {
                object __t115 = @move(@nexti, @nextj, @af_map, @af_ch);
                @af_map = P.Get(__t115, 0);
                @af_ch = P.Get(__t115, 1);
                @all_chess_move(@af_map, @af_ch);
            }
            if (P.Truth((P.Truth((P.Equal(@owner_color, @player_color))) && P.Truth((P.Equal(1, @cant_move(@af_map, @af_ch, @player_color)))) && P.Truth((P.Equal(0, @back_num))))))
            {
                ((PList)@m2).@append(P.L(P.Get(@mm, 0), P.Get(@mm, 1), null, null, @max_p_score));
                P.Set(@gb_m2, @ind, @m2);
                ((ResultQueue)@q).@put(P.Get(@gb_m2, @ind));
                return null;
            }
            if (P.Truth(((P.Number(@back_num) > P.Number(0)))))
            {
                @all_pm = P.L(P.L(null, null));
            }
            else
            {
                @all_pm = P.L();
            }
            foreach (object __t116 in P.Iter(@af_ch))
            {
                @chr = __t116;
                foreach (object __t117 in P.Iter(@chr))
                {
                    @ch = __t117;
                    if (P.Truth((P.Truth((P.Equal(((Piece)@ch).@color, @owner_color))) && P.Truth((P.Equal(1, ((Piece)@ch).@live))) && P.Truth(((P.Number(((Piece)@ch).@back) < P.Number(1)))))))
                    {
                        foreach (object __t118 in P.Iter(((Piece)@ch).@possible_move))
                        {
                            @apm = __t118;
                            ((PList)@all_pm).@append(P.L(P.L(((Piece)@ch).@row, ((Piece)@ch).@col), @apm));
                        }
                        @score = @sc;
                    }
                }
            }
            foreach (object __t119 in P.Iter(@all_pm))
            {
                object __t120 = __t119;
                @ch_position = P.Get(__t120, 0);
                @pm = P.Get(__t120, 1);
                @mscore = @move_score(@ch_position, @pm, @af_ch, @af_map, @player_color, @player_color, @com_color, @com_ban_step, @king_live, P.Int(2));
                @score = SourceFloat.Fma(@div2, @mscore, @sc);
                @af_map_2 = P.DeepCopy(@af_map);
                @af_ch_2 = P.DeepCopy(@af_ch);
                if (P.Truth((P.Truth((!P.Equal(@ch_position, null))) && P.Truth((!P.Equal(@pm, null))))))
                {
                    object __t121 = @move(@ch_position, @pm, @af_map_2, @af_ch_2);
                    @af_map_2 = P.Get(__t121, 0);
                    @af_ch_2 = P.Get(__t121, 1);
                    @all_chess_move(@af_map_2, @af_ch_2);
                }
                if (P.Truth(((P.Number(@back_num) > P.Number(0)))))
                {
                    @all_pm_2 = P.L(P.L(null, null));
                }
                else
                {
                    @all_pm_2 = P.L();
                }
                foreach (object __t122 in P.Iter(@af_ch_2))
                {
                    @chr_com = __t122;
                    foreach (object __t123 in P.Iter(@chr_com))
                    {
                        @ch_com = __t123;
                        if (P.Truth((P.Truth((P.Equal(((Piece)@ch_com).@color, P.Sub(1, @owner_color)))) && P.Truth((P.Equal(1, ((Piece)@ch_com).@live))) && P.Truth(((P.Number(((Piece)@ch_com).@back) < P.Number(1)))))))
                        {
                            foreach (object __t124 in P.Iter(((Piece)@ch_com).@possible_move))
                            {
                                @apm_com = __t124;
                                ((PList)@all_pm_2).@append(P.L(P.L(((Piece)@ch_com).@row, ((Piece)@ch_com).@col), @apm_com));
                            }
                        }
                    }
                }
                foreach (object __t125 in P.Iter(@all_pm_2))
                {
                    object __t126 = __t125;
                    @ch_position2 = P.Get(__t126, 0);
                    @pm_com = P.Get(__t126, 1);
                    if (P.Truth((P.Truth((P.Equal(1, @pt))) && P.Truth((P.Equal(@ch_position2, @nextj))))))
                    {
                        @score2 = @score;
                    }
                    else
                    {
                        @score2 = SourceFloat.Fma(P.Neg(@div), @move_score(@ch_position2, @pm_com, @af_ch_2, @af_map_2, @com_color, @player_color, @com_color, @com_ban_step, @king_live, P.Int(3)), @score);
                    }
                    @af_map_3 = P.DeepCopy(@af_map_2);
                    @af_ch_3 = P.DeepCopy(@af_ch_2);
                    if (P.Truth((P.Truth((!P.Equal(@ch_position2, null))) && P.Truth((!P.Equal(@pm_com, null))))))
                    {
                        object __t127 = @move(@ch_position2, @pm_com, @af_map_3, @af_ch_3);
                        @af_map_3 = P.Get(__t127, 0);
                        @af_ch_3 = P.Get(__t127, 1);
                        @all_chess_move(@af_map_3, @af_ch_3);
                    }
                    if (P.Truth(((P.Number(@back_num) > P.Number(0)))))
                    {
                        @all_pm_3 = P.L(P.L(null, null));
                    }
                    else
                    {
                        @all_pm_3 = P.L();
                    }
                    foreach (object __t128 in P.Iter(@af_ch_3))
                    {
                        @chr_p = __t128;
                        foreach (object __t129 in P.Iter(@chr_p))
                        {
                            @ch_p = __t129;
                            if (P.Truth((P.Truth((P.Equal(((Piece)@ch_p).@color, @owner_color))) && P.Truth((P.Equal(1, ((Piece)@ch_p).@live))) && P.Truth(((P.Number(((Piece)@ch_p).@back) < P.Number(1)))))))
                            {
                                foreach (object __t130 in P.Iter(((Piece)@ch_p).@possible_move))
                                {
                                    @apm_p = __t130;
                                    ((PList)@all_pm_3).@append(P.L(P.L(((Piece)@ch_p).@row, ((Piece)@ch_p).@col), @apm_p));
                                }
                            }
                        }
                    }
                    foreach (object __t131 in P.Iter(@all_pm_3))
                    {
                        object __t132 = __t131;
                        @ch_position3 = P.Get(__t132, 0);
                        @pm_p = P.Get(__t132, 1);
                        @pity = @will_dead_pity(@ch_position3, @pm_p, @af_ch_3, @af_map_3, @owner_color);
                        if (P.Truth((P.Equal(0, @pity))))
                        {
                            if (P.Truth((P.Equal(0, @will_dead_pity_even_equal(@ch_position3, @pm_p, @af_ch_3, @af_map_3, @owner_color)))))
                            {
                                @p_a = P.Get(P.Get(@af_map_3, P.Get(@ch_position3, 0)), P.Get(@ch_position3, 1));
                                @c_a = P.Get(P.Get(@af_ch_3, P.Get(@p_a, 0)), P.Get(@p_a, 1));
                                @score3 = SourceFloat.Fma(@div, @move_score(@ch_position3, @pm_p, @af_ch_3, @af_map_3, @player_color, @player_color, @com_color, @com_ban_step, @king_live, P.Int(4)), @score2);
                                @bomb_score = 290;
                                if (P.Truth((P.Truth((P.Equal(2, ((Piece)@c_a).@value))) && P.Truth(((P.Number(@score3) > P.Number(@bomb_score)))))))
                                {
                                    @score3 = P.Sub(@score3, @bomb_score);
                                }
                            }
                            else
                            {
                                @score3 = @score2;
                            }
                        }
                        else
                        {
                            if (P.Truth((P.Equal(1, @pity))))
                            {
                                @score3 = P.Sub(@score2, 8);
                            }
                            else
                            {
                                @score3 = @score2;
                            }
                        }
                        if (P.Truth(((P.Number(@score3) > P.Number(@max_p_score)))))
                        {
                            @max_p_score = P.Number(@score3);
                            @ch_player = @ch_position3;
                            @pm_player = @pm_p;
                        }
                        if (P.Truth(((P.Number(@score3) > P.Number(@alpha)))))
                        {
                            break;
                        }
                    }
                    if (P.Truth((!P.Equal(@max_p_score, P.Neg(9000)))))
                    {
                        if (P.Truth(((P.Number(@alpha) > P.Number(@max_p_score)))))
                        {
                            @alpha = P.Number(@max_p_score);
                        }
                        ((PList)@m4).@append(P.L(@ch_position2, @pm_com, @ch_player, @pm_player, @max_p_score));
                        if (P.Truth(((P.Number(@max_p_score) < P.Number(@beta)))))
                        {
                            @max_p_score = P.Number(P.Neg(9000));
                            break;
                        }
                        @max_p_score = P.Number(P.Neg(9000));
                    }
                    else
                    {
                        ((PList)@m4).@append(P.L(@ch_position2, @pm_com, null, null, @score2));
                    }
                }
                if (P.Truth(@m4))
                {
                    @min_index = ((PList)@m4).@index(P.Extreme(@m4, false, new Func<object, object>((@s) => P.Get(@s, 4))));
                    @coms = P.Get(P.Get(@m4, @min_index), 4);
                    @ch_comp = P.Get(P.Get(@m4, @min_index), 0);
                    @pm_comp = P.Get(P.Get(@m4, @min_index), 1);
                    @alpha = P.Number(@AI_min_score);
                    if (P.Truth(((P.Number(@beta) < P.Number(@coms)))))
                    {
                        @beta = P.Number(@coms);
                    }
                    ((PList)@m3).@append(P.L(@ch_position, @pm, @ch_comp, @pm_comp, @coms));
                    @m4 = P.L();
                }
                else
                {
                    ((PList)@m3).@append(P.L(@ch_position, @pm, null, null, @score));
                }
            }
            if (P.Truth(@m3))
            {
                @max_index = ((PList)@m3).@index(P.Extreme(@m3, true, new Func<object, object>((@s) => P.Get(@s, 4))));
                @ps = P.Get(P.Get(@m3, @max_index), 4);
                @ch_1 = P.Get(P.Get(@m3, @max_index), 0);
                @pm_1 = P.Get(P.Get(@m3, @max_index), 1);
                ((PList)@m2).@append(P.L(P.Get(@mm, 0), P.Get(@mm, 1), @ch_1, @pm_1, @ps));
            }
            else
            {
                ((PList)@m2).@append(P.L(P.Get(@mm, 0), P.Get(@mm, 1), null, null, @sc));
            }
            P.Set(@gb_m2, @ind, @m2);
            ((ResultQueue)@q).@put(P.Get(@gb_m2, @ind));
            return null;
        }
        // Original darkchess.pyx: 2141
        public object @dest_will_dead_owner_wont_eat(object @org, object @dest, object @a_ch, object @a_map, object @opp_color)
        {
            object @af_ch = null;
            object @af_map = null;
            object @ch = null;
            object @chr = null;
            object @m = null;
            object @mm = null;
            object @my = null;
            object @n = null;
            @opp_color = P.Int(@opp_color);
            @n = P.Get(P.Get(@a_map, P.Get(@org, 0)), P.Get(@org, 1));
            @m = P.Get(P.Get(@a_map, P.Get(@dest, 0)), P.Get(@dest, 1));
            if (P.Truth((P.Equal(null, @n))))
            {
                return P.Int(0);
            }
            else
            {
                if (P.Truth((!P.Equal(@m, null))))
                {
                    return P.Int(0);
                }
            }
            @af_map = P.DeepCopy(@a_map);
            @af_ch = P.DeepCopy(@a_ch);
            object __t133 = @move(@org, @dest, @af_map, @af_ch);
            @af_map = P.Get(__t133, 0);
            @af_ch = P.Get(__t133, 1);
            @all_chess_move(@af_map, @af_ch);
            @mm = P.Get(P.Get(@af_map, P.Get(@dest, 0)), P.Get(@dest, 1));
            @my = P.Get(P.Get(@af_ch, P.Get(@mm, 0)), P.Get(@mm, 1));
            foreach (object __t134 in P.Iter(@af_ch))
            {
                @chr = __t134;
                foreach (object __t135 in P.Iter(@chr))
                {
                    @ch = __t135;
                    if (P.Truth((P.Equal(@ch, @my))))
                    {
                        continue;
                    }
                    if (P.Truth((P.Truth((P.Equal(1, ((Piece)@ch).@live))) && P.Truth(((P.Number(((Piece)@ch).@back) < P.Number(1)))) && P.Truth((P.Equal(((Piece)@ch).@color, @opp_color))))))
                    {
                        if (P.Truth((P.Contains(((Piece)@ch).@possible_move, @dest))))
                        {
                            return P.Int(1);
                        }
                    }
                }
            }
            return P.Int(0);
            return 0;
        }
        // Original darkchess.pyx: 2170
        public object @will_dead(object @org, object @a_ch, object @a_map, object @opp_color)
        {
            object @ch = null;
            object @chr = null;
            object @my = null;
            object @n = null;
            @opp_color = P.Int(@opp_color);
            @n = P.Get(P.Get(@a_map, P.Get(@org, 0)), P.Get(@org, 1));
            if (P.Truth((P.Equal(null, @n))))
            {
                return P.Int(0);
            }
            @my = P.Get(P.Get(@a_ch, P.Get(@n, 0)), P.Get(@n, 1));
            foreach (object __t136 in P.Iter(@a_ch))
            {
                @chr = __t136;
                foreach (object __t137 in P.Iter(@chr))
                {
                    @ch = __t137;
                    if (P.Truth((P.Equal(@ch, @my))))
                    {
                        continue;
                    }
                    if (P.Truth((P.Truth((P.Equal(1, ((Piece)@ch).@live))) && P.Truth(((P.Number(((Piece)@ch).@back) < P.Number(1)))) && P.Truth((P.Equal(((Piece)@ch).@color, @opp_color))))))
                    {
                        if (P.Truth((P.Contains(((Piece)@ch).@possible_move, @org))))
                        {
                            return P.Int(1);
                        }
                    }
                }
            }
            return P.Int(0);
            return 0;
        }
        // Original darkchess.pyx: 2187
        public object @will_eat2_more(object @nexti, object @nextj, object @a_ch, object @a_map, object @owner_color)
        {
            object @af_ch = null;
            object @af_map = null;
            object @can_eat = null;
            object @ch = null;
            object @chr = null;
            object @n = null;
            object @nch = null;
            object @opp_color = null;
            object @pm = null;
            @owner_color = P.Int(@owner_color);
            @opp_color = P.Int(P.Sub(1, @owner_color));
            @can_eat = P.Int(0);
            @af_map = P.DeepCopy(@a_map);
            @af_ch = P.DeepCopy(@a_ch);
            if (P.Truth((P.Truth((!P.Equal(@nexti, null))) && P.Truth((!P.Equal(@nextj, null))))))
            {
                object __t138 = @move(@nexti, @nextj, @af_map, @af_ch);
                @af_map = P.Get(__t138, 0);
                @af_ch = P.Get(__t138, 1);
                @all_chess_move(@af_map, @af_ch);
            }
            foreach (object __t139 in P.Iter(@af_ch))
            {
                @chr = __t139;
                foreach (object __t140 in P.Iter(@chr))
                {
                    @ch = __t140;
                    if (P.Truth((P.Truth((P.Equal(1, ((Piece)@ch).@live))) && P.Truth(((P.Number(((Piece)@ch).@back) < P.Number(1)))) && P.Truth((P.Equal(((Piece)@ch).@color, @owner_color))))))
                    {
                        foreach (object __t141 in P.Iter(((Piece)@ch).@possible_move))
                        {
                            @pm = __t141;
                            @n = P.Get(P.Get(@af_map, P.Get(@pm, 0)), P.Get(@pm, 1));
                            if (P.Truth((!P.Equal(@n, null))))
                            {
                                @nch = P.Get(P.Get(@af_ch, P.Get(@n, 0)), P.Get(@n, 1));
                                if (P.Truth((P.Equal(((Piece)@ch).@value, ((Piece)@nch).@value))))
                                {
                                    continue;
                                }
                            }
                            if (P.Truth((P.Equal(1, @stand_will_dead_pity(@pm, @af_ch, @af_map, @opp_color)))))
                            {
                                @can_eat = P.Int(P.Add(@can_eat, 1));
                            }
                        }
                    }
                }
            }
            if (P.Truth(((P.Number(@can_eat) >= P.Number(2)))))
            {
                return P.Int(1);
            }
            else
            {
                return P.Int(0);
            }
            return 0;
        }
        // Original darkchess.pyx: 2213
        public object @escape_way_to_run(object @nexti, object @nextj, object @a_ch, object @a_map, object @owner_color)
        {
            object @af_ch = null;
            object @af_map = null;
            object @eat_pm = null;
            object @eat_step = null;
            object @escape_step = null;
            object @m = null;
            object @my = null;
            object @n = null;
            object @nch = null;
            object @opp_color = null;
            object @pm = null;
            @owner_color = P.Int(@owner_color);
            @opp_color = P.Sub(1, @owner_color);
            @af_map = P.DeepCopy(@a_map);
            @af_ch = P.DeepCopy(@a_ch);
            if (P.Truth((P.Truth((!P.Equal(@nexti, null))) && P.Truth((!P.Equal(@nextj, null))))))
            {
                object __t142 = @move(@nexti, @nextj, @af_map, @af_ch);
                @af_map = P.Get(__t142, 0);
                @af_ch = P.Get(__t142, 1);
                @all_chess_move(@af_map, @af_ch);
            }
            @m = P.Get(P.Get(@af_map, P.Get(@nextj, 0)), P.Get(@nextj, 1));
            @my = P.Get(P.Get(@af_ch, P.Get(@m, 0)), P.Get(@m, 1));
            @escape_step = 0;
            foreach (object __t143 in P.Iter(((Piece)@my).@possible_move))
            {
                @eat_pm = __t143;
                if (P.Truth((!P.Equal(P.Get(P.Get(@af_map, P.Get(@eat_pm, 0)), P.Get(@eat_pm, 1)), null))))
                {
                    @eat_step = 0;
                    @n = P.Get(P.Get(@af_map, P.Get(@eat_pm, 0)), P.Get(@eat_pm, 1));
                    @nch = P.Get(P.Get(@af_ch, P.Get(@n, 0)), P.Get(@n, 1));
                    if (P.Truth((P.Equal(((Piece)@nch).@value, ((Piece)@my).@value))))
                    {
                        continue;
                    }
                    if (P.Truth((P.Truth((P.Equal(1, ((Piece)@nch).@live))) && P.Truth(((P.Number(((Piece)@nch).@back) < P.Number(1)))) && P.Truth((P.Equal(((Piece)@nch).@color, @opp_color))))))
                    {
                        if (P.Truth((P.Equal(1, @stand_will_dead_pity(@eat_pm, @af_ch, @af_map, @opp_color)))))
                        {
                            foreach (object __t144 in P.Iter(((Piece)@nch).@possible_move))
                            {
                                @pm = __t144;
                                if (P.Truth((P.Equal(1, @will_dead_pity_uncheck_will_dead(@eat_pm, @pm, @af_ch, @af_map, @opp_color)))))
                                {
                                    @eat_step = P.Add(@eat_step, 1);
                                }
                            }
                            if (P.Truth((P.Equal(@eat_step, P.Len(((Piece)@nch).@possible_move)))))
                            {
                                return P.Number(0);
                            }
                            else
                            {
                                if (P.Truth(((P.Number(@escape_step) > P.Number(P.Sub(@eat_step, P.Len(((Piece)@nch).@possible_move)))))))
                                {
                                    @escape_step = P.Sub(@eat_step, P.Len(((Piece)@nch).@possible_move));
                                }
                            }
                        }
                    }
                }
            }
            if (P.Truth((P.Equal(0, @escape_step))))
            {
                return P.Number(P.Neg(0.09));
            }
            else
            {
                return P.Number(P.Div(@escape_step, 100));
            }
            return 0;
        }
        // Original darkchess.pyx: 2248
        public object @stand_will_dead_pity(object @org, object @a_ch, object @a_map, object @owner_color)
        {
            object @ch = null;
            object @chr = null;
            object @my = null;
            object @n = null;
            object @opp_color = null;
            @owner_color = P.Int(@owner_color);
            @opp_color = P.Int(P.Sub(1, @owner_color));
            @n = P.Get(P.Get(@a_map, P.Get(@org, 0)), P.Get(@org, 1));
            if (P.Truth((P.Equal(null, @n))))
            {
                return P.Int(0);
            }
            @my = P.Get(P.Get(@a_ch, P.Get(@n, 0)), P.Get(@n, 1));
            foreach (object __t145 in P.Iter(@a_ch))
            {
                @chr = __t145;
                foreach (object __t146 in P.Iter(@chr))
                {
                    @ch = __t146;
                    if (P.Truth((P.Equal(@ch, @my))))
                    {
                        continue;
                    }
                    if (P.Truth((P.Truth((P.Equal(1, ((Piece)@ch).@live))) && P.Truth(((P.Number(((Piece)@ch).@back) < P.Number(1)))) && P.Truth((P.Equal(((Piece)@ch).@color, @opp_color))))))
                    {
                        if (P.Truth((P.Contains(((Piece)@ch).@possible_move, @org))))
                        {
                            if (P.Truth((P.Equal(0, @will_dead_pity(P.L(((Piece)@ch).@row, ((Piece)@ch).@col), @org, @a_ch, @a_map, @opp_color)))))
                            {
                                return P.Int(1);
                            }
                        }
                    }
                }
            }
            return P.Int(0);
            return 0;
        }
        // Original darkchess.pyx: 2269
        public object @will_dead_pity_uncheck_will_dead(object @nexti, object @nextj, object @a_ch, object @a_map, object @owner_color)
        {
            object @a = null;
            object @af2_ch = null;
            object @af2_map = null;
            object @af3_ch = null;
            object @af3_map = null;
            object @af_ch = null;
            object @af_map = null;
            object @b = null;
            object @bb = null;
            object @bbb = null;
            object @ch = null;
            object @chr = null;
            object @i2 = null;
            object @i3 = null;
            object @ii = null;
            object @j2 = null;
            object @j3 = null;
            object @jj = null;
            object @opp_color = null;
            object @pity = null;
            object @x = null;
            object @y = null;
            @owner_color = P.Int(@owner_color);
            object __t147 = @nexti;
            @y = P.Get(__t147, 0);
            @x = P.Get(__t147, 1);
            @a = P.Get(P.Get(@a_map, @y), @x);
            if (P.Truth((!P.Equal(@nextj, null))))
            {
                object __t148 = @nextj;
                @ii = P.Get(__t148, 0);
                @jj = P.Get(__t148, 1);
                @b = P.Get(P.Get(@a_map, @ii), @jj);
                if (P.Truth((!P.Equal(@b, null))))
                {
                    if (P.Truth((P.Equal(2, ((Piece)P.Get(P.Get(@a_ch, P.Get(@a, 0)), P.Get(@a, 1))).@value))))
                    {
                        if (P.Truth(((P.Number(((Piece)P.Get(P.Get(@a_ch, P.Get(@b, 0)), P.Get(@b, 1))).@value) > P.Number(5)))))
                        {
                            return P.Int(0);
                        }
                    }
                    if (P.Truth((P.Equal(((Piece)P.Get(P.Get(@a_ch, P.Get(@b, 0)), P.Get(@b, 1))).@value, ((Piece)P.Get(P.Get(@a_ch, P.Get(@a, 0)), P.Get(@a, 1))).@value))))
                    {
                        return P.Int(0);
                    }
                }
            }
            @af_map = P.DeepCopy(@a_map);
            @af_ch = P.DeepCopy(@a_ch);
            object __t149 = @move(@nexti, @nextj, @af_map, @af_ch);
            @af_map = P.Get(__t149, 0);
            @af_ch = P.Get(__t149, 1);
            @all_chess_move(@af_map, @af_ch);
            @opp_color = P.Sub(1, @owner_color);
            @pity = 0;
            @i2 = null;
            @j2 = null;
            @i3 = null;
            @j3 = null;
            foreach (object __t150 in P.Iter(@af_ch))
            {
                @chr = __t150;
                foreach (object __t151 in P.Iter(@chr))
                {
                    @ch = __t151;
                    if (P.Truth((P.Truth((P.Equal(1, ((Piece)@ch).@back))) || P.Truth((P.Equal(0, ((Piece)@ch).@live))))))
                    {
                        continue;
                    }
                    if (P.Truth((P.Equal(((Piece)@ch).@color, @opp_color))))
                    {
                        if (P.Truth((P.Contains(((Piece)@ch).@possible_move, @nextj))))
                        {
                            if (P.Truth((P.Equal(@b, null))))
                            {
                                @i2 = P.L(((Piece)@ch).@row, ((Piece)@ch).@col);
                                @j2 = @nextj;
                                @pity = 1;
                                @af2_map = P.DeepCopy(@af_map);
                                @af2_ch = P.DeepCopy(@af_ch);
                                object __t152 = @move(@i2, @j2, @af2_map, @af2_ch);
                                @af2_map = P.Get(__t152, 0);
                                @af2_ch = P.Get(__t152, 1);
                                @all_chess_move(@af2_map, @af2_ch);
                                object __t153 = @j2;
                                @ii = P.Get(__t153, 0);
                                @jj = P.Get(__t153, 1);
                                @bb = P.Get(P.Get(@af2_map, @ii), @jj);
                                foreach (object __t154 in P.Iter(@af2_ch))
                                {
                                    @chr = __t154;
                                    foreach (object __t155 in P.Iter(@chr))
                                    {
                                        @ch = __t155;
                                        if (P.Truth((P.Truth((P.Equal(1, ((Piece)@ch).@back))) || P.Truth((P.Equal(0, ((Piece)@ch).@live))))))
                                        {
                                            continue;
                                        }
                                        if (P.Truth((P.Equal(((Piece)@ch).@color, @owner_color))))
                                        {
                                            if (P.Truth((P.Contains(((Piece)@ch).@possible_move, @j2))))
                                            {
                                                if (P.Truth(((P.Number(@eating_value_to_score(((Piece)P.Get(P.Get(@a_ch, P.Get(@a, 0)), P.Get(@a, 1))).@value, @king_live, P.Sub(1, @owner_color))) <= P.Number(@eating_value_to_score(((Piece)P.Get(P.Get(@af2_ch, P.Get(@bb, 0)), P.Get(@bb, 1))).@value, @king_live, @owner_color))))))
                                                {
                                                    @i3 = P.L(((Piece)@ch).@row, ((Piece)@ch).@col);
                                                    @j3 = @j2;
                                                    @pity = 0;
                                                    @af3_map = P.DeepCopy(@af2_map);
                                                    @af3_ch = P.DeepCopy(@af2_ch);
                                                    object __t156 = @move(@i3, @j3, @af3_map, @af3_ch);
                                                    @af3_map = P.Get(__t156, 0);
                                                    @af3_ch = P.Get(__t156, 1);
                                                    @all_chess_move(@af3_map, @af3_ch);
                                                    foreach (object __t157 in P.Iter(@af3_ch))
                                                    {
                                                        @chr = __t157;
                                                        foreach (object __t158 in P.Iter(@chr))
                                                        {
                                                            @ch = __t158;
                                                            if (P.Truth((P.Truth((P.Equal(1, ((Piece)@ch).@back))) || P.Truth((P.Equal(0, ((Piece)@ch).@live))))))
                                                            {
                                                                continue;
                                                            }
                                                            if (P.Truth((P.Equal(((Piece)@ch).@color, @opp_color))))
                                                            {
                                                                if (P.Truth((P.Contains(((Piece)@ch).@possible_move, @j3))))
                                                                {
                                                                    return P.Int(1);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (P.Truth((P.Equal(1, @pity))))
                                {
                                    return P.Int(1);
                                }
                            }
                            else
                            {
                                if (P.Truth(((P.Number(@eating_value_to_score(((Piece)P.Get(P.Get(@a_ch, P.Get(@a, 0)), P.Get(@a, 1))).@value, @king_live, P.Sub(1, @owner_color))) > P.Number(@eating_value_to_score(((Piece)P.Get(P.Get(@a_ch, P.Get(@b, 0)), P.Get(@b, 1))).@value, @king_live, @owner_color))))))
                                {
                                    @i2 = P.L(((Piece)@ch).@row, ((Piece)@ch).@col);
                                    @j2 = @nextj;
                                    @pity = 1;
                                    @af2_map = P.DeepCopy(@af_map);
                                    @af2_ch = P.DeepCopy(@af_ch);
                                    object __t159 = @move(@i2, @j2, @af2_map, @af2_ch);
                                    @af2_map = P.Get(__t159, 0);
                                    @af2_ch = P.Get(__t159, 1);
                                    @all_chess_move(@af2_map, @af2_ch);
                                    object __t160 = @j2;
                                    @ii = P.Get(__t160, 0);
                                    @jj = P.Get(__t160, 1);
                                    @bbb = P.Get(P.Get(@af2_map, @ii), @jj);
                                    foreach (object __t161 in P.Iter(@af2_ch))
                                    {
                                        @chr = __t161;
                                        foreach (object __t162 in P.Iter(@chr))
                                        {
                                            @ch = __t162;
                                            if (P.Truth((P.Truth((P.Equal(1, ((Piece)@ch).@back))) || P.Truth((P.Equal(0, ((Piece)@ch).@live))))))
                                            {
                                                continue;
                                            }
                                            if (P.Truth((P.Equal(((Piece)@ch).@color, @owner_color))))
                                            {
                                                if (P.Truth((P.Contains(((Piece)@ch).@possible_move, @j2))))
                                                {
                                                    if (P.Truth(((P.Number(@eating_value_to_score(((Piece)P.Get(P.Get(@a_ch, P.Get(@a, 0)), P.Get(@a, 1))).@value, @king_live, P.Sub(1, @owner_color))) <= P.Number(@eating_value_to_score(((Piece)P.Get(P.Get(@af2_ch, P.Get(@bbb, 0)), P.Get(@bbb, 1))).@value, @king_live, @owner_color))))))
                                                    {
                                                        @i3 = P.L(((Piece)@ch).@row, ((Piece)@ch).@col);
                                                        @j3 = @j2;
                                                        @pity = 0;
                                                        @af3_map = P.DeepCopy(@af2_map);
                                                        @af3_ch = P.DeepCopy(@af2_ch);
                                                        object __t163 = @move(@i3, @j3, @af3_map, @af3_ch);
                                                        @af3_map = P.Get(__t163, 0);
                                                        @af3_ch = P.Get(__t163, 1);
                                                        @all_chess_move(@af3_map, @af3_ch);
                                                        foreach (object __t164 in P.Iter(@af3_ch))
                                                        {
                                                            @chr = __t164;
                                                            foreach (object __t165 in P.Iter(@chr))
                                                            {
                                                                @ch = __t165;
                                                                if (P.Truth((P.Truth((P.Equal(1, ((Piece)@ch).@back))) || P.Truth((P.Equal(0, ((Piece)@ch).@live))))))
                                                                {
                                                                    continue;
                                                                }
                                                                if (P.Truth((P.Equal(((Piece)@ch).@color, @opp_color))))
                                                                {
                                                                    if (P.Truth((P.Contains(((Piece)@ch).@possible_move, @j3))))
                                                                    {
                                                                        return P.Int(1);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (P.Truth((P.Equal(1, @pity))))
                                    {
                                        return P.Int(1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return P.Int(@pity);
            return 0;
        }
        // Original darkchess.pyx: 2383
        public object @will_dead_pity_even_equal(object @nexti, object @nextj, object @a_ch, object @a_map, object @owner_color)
        {
            object @a = null;
            object @af2_ch = null;
            object @af2_map = null;
            object @af3_ch = null;
            object @af3_map = null;
            object @af_ch = null;
            object @af_map = null;
            object @b = null;
            object @bb = null;
            object @bbb = null;
            object @ch = null;
            object @chr = null;
            object @i2 = null;
            object @i3 = null;
            object @ii = null;
            object @j2 = null;
            object @j3 = null;
            object @jj = null;
            object @opp_color = null;
            object @pity = null;
            object @x = null;
            object @y = null;
            @owner_color = P.Int(@owner_color);
            if (P.Truth((P.Truth((P.Equal(null, @nexti))) || P.Truth((P.Equal(null, @nextj))))))
            {
                return null;
            }
            object __t166 = @nexti;
            @y = P.Get(__t166, 0);
            @x = P.Get(__t166, 1);
            @a = P.Get(P.Get(@a_map, @y), @x);
            if (P.Truth((!P.Equal(@nextj, null))))
            {
                object __t167 = @nextj;
                @ii = P.Get(__t167, 0);
                @jj = P.Get(__t167, 1);
                @b = P.Get(P.Get(@a_map, @ii), @jj);
                if (P.Truth((!P.Equal(@b, null))))
                {
                    if (P.Truth((P.Equal(2, ((Piece)P.Get(P.Get(@a_ch, P.Get(@a, 0)), P.Get(@a, 1))).@value))))
                    {
                        if (P.Truth(((P.Number(((Piece)P.Get(P.Get(@a_ch, P.Get(@b, 0)), P.Get(@b, 1))).@value) > P.Number(5)))))
                        {
                            return 0;
                        }
                    }
                    if (P.Truth((P.Equal(((Piece)P.Get(P.Get(@a_ch, P.Get(@b, 0)), P.Get(@b, 1))).@value, ((Piece)P.Get(P.Get(@a_ch, P.Get(@a, 0)), P.Get(@a, 1))).@value))))
                    {
                        return 0;
                    }
                }
            }
            @af_map = P.DeepCopy(@a_map);
            @af_ch = P.DeepCopy(@a_ch);
            object __t168 = @move(@nexti, @nextj, @af_map, @af_ch);
            @af_map = P.Get(__t168, 0);
            @af_ch = P.Get(__t168, 1);
            @all_chess_move(@af_map, @af_ch);
            @opp_color = P.Sub(1, @owner_color);
            @pity = 0;
            @i2 = null;
            @j2 = null;
            @i3 = null;
            @j3 = null;
            foreach (object __t169 in P.Iter(@af_ch))
            {
                @chr = __t169;
                foreach (object __t170 in P.Iter(@chr))
                {
                    @ch = __t170;
                    if (P.Truth((P.Truth((P.Equal(1, ((Piece)@ch).@back))) || P.Truth((P.Equal(0, ((Piece)@ch).@live))))))
                    {
                        continue;
                    }
                    if (P.Truth((P.Equal(((Piece)@ch).@color, @opp_color))))
                    {
                        if (P.Truth((P.Contains(((Piece)@ch).@possible_move, @nextj))))
                        {
                            if (P.Truth((P.Equal(@b, null))))
                            {
                                @i2 = P.L(((Piece)@ch).@row, ((Piece)@ch).@col);
                                @j2 = @nextj;
                                @pity = 1;
                                @af2_map = P.DeepCopy(@af_map);
                                @af2_ch = P.DeepCopy(@af_ch);
                                object __t171 = @move(@i2, @j2, @af2_map, @af2_ch);
                                @af2_map = P.Get(__t171, 0);
                                @af2_ch = P.Get(__t171, 1);
                                @all_chess_move(@af2_map, @af2_ch);
                                object __t172 = @j2;
                                @ii = P.Get(__t172, 0);
                                @jj = P.Get(__t172, 1);
                                @bb = P.Get(P.Get(@af2_map, @ii), @jj);
                                foreach (object __t173 in P.Iter(@af2_ch))
                                {
                                    @chr = __t173;
                                    foreach (object __t174 in P.Iter(@chr))
                                    {
                                        @ch = __t174;
                                        if (P.Truth((P.Truth((P.Equal(1, ((Piece)@ch).@back))) || P.Truth((P.Equal(0, ((Piece)@ch).@live))))))
                                        {
                                            continue;
                                        }
                                        if (P.Truth((P.Equal(((Piece)@ch).@color, @owner_color))))
                                        {
                                            if (P.Truth((P.Contains(((Piece)@ch).@possible_move, @j2))))
                                            {
                                                if (P.Truth(((P.Number(@eating_value_to_score(((Piece)P.Get(P.Get(@a_ch, P.Get(@a, 0)), P.Get(@a, 1))).@value, @king_live, P.Sub(1, @owner_color))) < P.Number(@eating_value_to_score(((Piece)P.Get(P.Get(@af2_ch, P.Get(@bb, 0)), P.Get(@bb, 1))).@value, @king_live, @owner_color))))))
                                                {
                                                    @i3 = P.L(((Piece)@ch).@row, ((Piece)@ch).@col);
                                                    @j3 = @j2;
                                                    @pity = 0;
                                                    @af3_map = P.DeepCopy(@af2_map);
                                                    @af3_ch = P.DeepCopy(@af2_ch);
                                                    object __t175 = @move(@i3, @j3, @af3_map, @af3_ch);
                                                    @af3_map = P.Get(__t175, 0);
                                                    @af3_ch = P.Get(__t175, 1);
                                                    @all_chess_move(@af3_map, @af3_ch);
                                                    foreach (object __t176 in P.Iter(@af3_ch))
                                                    {
                                                        @chr = __t176;
                                                        foreach (object __t177 in P.Iter(@chr))
                                                        {
                                                            @ch = __t177;
                                                            if (P.Truth((P.Truth((P.Equal(1, ((Piece)@ch).@back))) || P.Truth((P.Equal(0, ((Piece)@ch).@live))))))
                                                            {
                                                                continue;
                                                            }
                                                            if (P.Truth((P.Equal(((Piece)@ch).@color, @opp_color))))
                                                            {
                                                                if (P.Truth((P.Contains(((Piece)@ch).@possible_move, @j3))))
                                                                {
                                                                    return 1;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (P.Truth((P.Equal(1, @pity))))
                                {
                                    return 1;
                                }
                            }
                            else
                            {
                                if (P.Truth(((P.Number(@eating_value_to_score(((Piece)P.Get(P.Get(@a_ch, P.Get(@a, 0)), P.Get(@a, 1))).@value, @king_live, P.Sub(1, @owner_color))) >= P.Number(@eating_value_to_score(((Piece)P.Get(P.Get(@a_ch, P.Get(@b, 0)), P.Get(@b, 1))).@value, @king_live, @owner_color))))))
                                {
                                    @i2 = P.L(((Piece)@ch).@row, ((Piece)@ch).@col);
                                    @j2 = @nextj;
                                    @pity = 1;
                                    @af2_map = P.DeepCopy(@af_map);
                                    @af2_ch = P.DeepCopy(@af_ch);
                                    object __t178 = @move(@i2, @j2, @af2_map, @af2_ch);
                                    @af2_map = P.Get(__t178, 0);
                                    @af2_ch = P.Get(__t178, 1);
                                    @all_chess_move(@af2_map, @af2_ch);
                                    object __t179 = @j2;
                                    @ii = P.Get(__t179, 0);
                                    @jj = P.Get(__t179, 1);
                                    @bbb = P.Get(P.Get(@af2_map, @ii), @jj);
                                    foreach (object __t180 in P.Iter(@af2_ch))
                                    {
                                        @chr = __t180;
                                        foreach (object __t181 in P.Iter(@chr))
                                        {
                                            @ch = __t181;
                                            if (P.Truth((P.Truth((P.Equal(1, ((Piece)@ch).@back))) || P.Truth((P.Equal(0, ((Piece)@ch).@live))))))
                                            {
                                                continue;
                                            }
                                            if (P.Truth((P.Equal(((Piece)@ch).@color, @owner_color))))
                                            {
                                                if (P.Truth((P.Contains(((Piece)@ch).@possible_move, @j2))))
                                                {
                                                    if (P.Truth(((P.Number(@eating_value_to_score(((Piece)P.Get(P.Get(@a_ch, P.Get(@a, 0)), P.Get(@a, 1))).@value, @king_live, P.Sub(1, @owner_color))) < P.Number(@eating_value_to_score(((Piece)P.Get(P.Get(@af2_ch, P.Get(@bbb, 0)), P.Get(@bbb, 1))).@value, @king_live, @owner_color))))))
                                                    {
                                                        @i3 = P.L(((Piece)@ch).@row, ((Piece)@ch).@col);
                                                        @j3 = @j2;
                                                        @pity = 0;
                                                        @af3_map = P.DeepCopy(@af2_map);
                                                        @af3_ch = P.DeepCopy(@af2_ch);
                                                        object __t182 = @move(@i3, @j3, @af3_map, @af3_ch);
                                                        @af3_map = P.Get(__t182, 0);
                                                        @af3_ch = P.Get(__t182, 1);
                                                        @all_chess_move(@af3_map, @af3_ch);
                                                        foreach (object __t183 in P.Iter(@af3_ch))
                                                        {
                                                            @chr = __t183;
                                                            foreach (object __t184 in P.Iter(@chr))
                                                            {
                                                                @ch = __t184;
                                                                if (P.Truth((P.Truth((P.Equal(1, ((Piece)@ch).@back))) || P.Truth((P.Equal(0, ((Piece)@ch).@live))))))
                                                                {
                                                                    continue;
                                                                }
                                                                if (P.Truth((P.Equal(((Piece)@ch).@color, @opp_color))))
                                                                {
                                                                    if (P.Truth((P.Contains(((Piece)@ch).@possible_move, @j3))))
                                                                    {
                                                                        return 1;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (P.Truth((P.Equal(1, @pity))))
                                    {
                                        return 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return @pity;
            return null;
        }
        // Original darkchess.pyx: 2504
        public object @will_dead_pity(object @nexti, object @nextj, object @a_ch, object @a_map, object @owner_color)
        {
            @owner_color = P.Int(@owner_color);
            if (P.Truth((P.Truth((P.Equal(null, @nexti))) || P.Truth((P.Equal(null, @nextj))))))
            {
                return null;
            }
            if (P.Truth((P.Equal(1, @will_dead(@nexti, @a_ch, @a_map, P.Sub(1, @owner_color))))))
            {
                return 0;
            }
            return @will_dead_pity_uncheck_will_dead(@nexti, @nextj, @a_ch, @a_map, @owner_color);
            return null;
        }
        // Original darkchess.pyx: 2515
        public object @eating_value_to_score(object @value, object @king, object @owner_color)
        {
            @value = P.Int(@value);
            @owner_color = P.Int(@owner_color);
            if (P.Truth((P.Equal(1, @value))))
            {
                if (P.Truth((P.Equal(1, P.Get(@king, @owner_color)))))
                {
                    return P.Int(30);
                }
                else
                {
                    return P.Int(25);
                }
            }
            else
            {
                if (P.Truth((P.Equal(2, @value))))
                {
                    return P.Int(290);
                }
                else
                {
                    if (P.Truth((P.Equal(3, @value))))
                    {
                        return P.Int(28);
                    }
                    else
                    {
                        if (P.Truth((P.Equal(4, @value))))
                        {
                            return P.Int(75);
                        }
                        else
                        {
                            if (P.Truth((P.Equal(5, @value))))
                            {
                                return P.Int(165);
                            }
                            else
                            {
                                if (P.Truth((P.Equal(6, @value))))
                                {
                                    return P.Int(595);
                                }
                                else
                                {
                                    if (P.Truth((P.Equal(7, @value))))
                                    {
                                        return P.Int(1205);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return 0;
        }
    }
}
