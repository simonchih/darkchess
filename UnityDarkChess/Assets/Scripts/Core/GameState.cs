using System;
namespace DarkChessUnity
{
    public partial class OriginalEngine
    {
        public void NewGame(uint? seed = null)
        {
            if (seed.HasValue) random = new SourceRandom(seed.Value);
            player_win = 0; turn_id = 0; player_color = 0; com_color = 1; first = 1;
            chess_num = P.L(16,16); back_num = 32;
            break_long_capture_dest = P.L(); break_long_capture_org = P.L(); com_ban_step = P.L();
            move_step = P.L(null,null,null,null); sindex = 0; step = 0;
            back_value_num = P.L(P.L(0,5,2,2,2,2,2,1), P.L(0,5,2,2,2,2,2,1));
            player_first = random.RandInt(0,1);
            dynamic indices = ini_random_chess(P.Mul(P.L(0),32));
            main_chess = Grid(null); server_main_chess = Grid(null); main_map = Grid(null);
            for (int r = 0; r < 4; r++) for (int c = 0; c < 8; c++)
            {
                P.Set(P.Get(server_main_chess,r),c,NewPiece(P.Get(indices,8*r+c),P.L(r,c)));
                P.Set(P.Get(main_chess,r),c,NewPiece(32,P.L(r,c)));
                P.Set(P.Get(main_map,r),c,P.L(r,c));
            }
            Sound?.Invoke("NEWGAME");
        }
        public Piece At(int r, int c)
        {
            if (r < 0 || r >= 4 || c < 0 || c >= 8) return null;
            dynamic id = P.Get(P.Get(main_map,r),c);
            return id == null ? null : (Piece)P.Get(P.Get(main_chess,P.Get(id,0)),P.Get(id,1));
        }
        public bool HumanTurn { get { return P.Int(player_win)==0 && P.Int(turn_id)==P.Int(player_color) && (P.Int(first)==0 || P.Int(player_first)==1); } }
        public bool HumanFlip(int r,int c)
        {
            Piece p = At(r,c);
            if (!HumanTurn || p == null || p.back != 1 || p.live != 1) return false;
            // Hidden pieces have never moved: identity and board coordinates coincide.
            Piece secret = (Piece)P.Get(P.Get(server_main_chess,r),c);
            int index = P.Int(ColorValueToIndex(secret.color,secret.value,back_value_num));
            Piece revealed = (Piece)NewPiece(index,P.L(r,c)); revealed.back = 0;
            P.Set(P.Get(main_chess,r),c,revealed);
            if (P.Int(first)==1)
            {
                player_color = IndexToColor(index); com_color = 1-P.Int(player_color); first = 0;
            }
            back_num = P.Int(back_num)-1;
            dynamic counts = P.Get(back_value_num,revealed.color);
            P.Set(counts,revealed.value,P.Sub(P.Get(counts,revealed.value),1));
            turn_id = com_color; step = P.Int(step)+1;
            Sound?.Invoke("CLICK");
            return true;
        }
        public bool HumanMove(int r,int c,int rr,int cc)
        {
            Piece p=At(r,c);
            if (!HumanTurn || p==null || p.back==1 || p.color!=P.Int(player_color)) return false;
            all_chess_move(main_map,main_chess);
            dynamic dest=P.L(rr,cc);
            if (!P.Contains(p.possible_move,dest)) return false;
            Piece target=At(rr,cc);
            if (target!=null)
            {
                target.live=0;
                P.Set(chess_num,target.color,P.Sub(P.Get(chess_num,target.color),1));
                Sound?.Invoke("CAPTURE2");
            }
            else Sound?.Invoke("MOVE2");
            P.Set(P.Get(main_map,rr),cc,P.Get(P.Get(main_map,r),c));
            P.Set(P.Get(main_map,r),c,null);
            dynamic org=P.L(r,c);
            p.row=rr; p.col=cc;
            p.x=cc<4?34+cc*57:260+(cc-4)*57; p.y=51+rr*57;
            turn_id=com_color; step=P.Int(step)+1;
            dynamic possible=collect_possible_move(rr,cc,main_map,main_chess);
            P.Set(move_step,sindex,P.L(p.color,org,dest,possible));
            sindex=(P.Int(sindex)+1)%4;
            int br=0;
            while(br<P.Len(break_long_capture_dest))
            {
                bool found=false;
                foreach(object d in P.Iter(P.Get(break_long_capture_dest,br)))
                    if(P.Equal(dest,d)){RemoveBan(br);found=true;break;}
                if(!found)br++;
            }
            br=0;
            while(br<P.Len(break_long_capture_org))
            {
                bool found=false;
                foreach(object o in P.Iter(P.Get(break_long_capture_org,br)))
                    if(P.Equal(org,o)){RemoveBan(br);found=true;break;}
                if(!found)br++;
            }
            CheckCounts();
            return true;
        }
        void RemoveBan(int i)
        {
            P.Delete(break_long_capture_dest,i);P.Delete(break_long_capture_org,i);P.Delete(com_ban_step,i);
        }
        public void FinishComputerMove()
        {
            // The source defers map/capture commitment until the visual move reaches its destination.
            foreach(dynamic row in P.Iter(main_chess)) foreach(Piece p in P.Iter(row))
            {
                int x=p.col<4?34+p.col*57:260+(p.col-4)*57, y=51+p.row*57;
                if(p.x==x && p.y==y)continue;
                p.x=x;p.y=y;
                Piece captured=At(p.row,p.col);
                if(captured!=null)
                {
                    captured.live=0;
                    P.Set(chess_num,captured.color,P.Sub(P.Get(chess_num,captured.color),1));
                }
                P.Set(P.Get(main_map,p.row),p.col,P.L(P.Get(com_mv_map,0),P.Get(com_mv_map,1)));
            }
            turn_id=player_color;
            CheckEnd();
            all_chess_move(main_map,main_chess);
        }
        public void CheckEnd()
        {
            if(P.Int(back_num)==0 && P.Int(cant_move(main_map,main_chess,player_color))==1) player_win=-1;
            CheckCounts();
        }
        public void CheckCounts()
        {
            if(P.Int(P.Get(chess_num,player_color))==0) { RevealRemaining();player_win=-1; }
            else if(P.Int(P.Get(chess_num,com_color))==0) { RevealRemaining();player_win=1; }
        }
        void RevealRemaining()
        {
            for(int r=0;r<4;r++)for(int c=0;c<8;c++)
            {
                Piece p=(Piece)P.Get(P.Get(main_chess,r),c);
                if(p.index!=32||p.live!=1)continue;
                Piece secret=(Piece)P.Get(P.Get(server_main_chess,r),c);
                Piece face=(Piece)NewPiece(ColorValueToIndex(secret.color,secret.value,back_value_num),P.L(r,c));
                face.back=0;P.Set(P.Get(main_chess,r),c,face);
            }
        }
    }
}
