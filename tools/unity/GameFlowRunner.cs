using System;
using DarkChessUnity;
class GameFlowRunner
{
    static int assertions;
    static void Check(bool pass,string name){assertions++;if(!pass)throw new Exception(name);}
    static OriginalEngine Position(bool lastEnemy)
    {
        var e=new OriginalEngine();e.NewGame(0);e.main_map=OriginalEngine.Grid(null);
        foreach(dynamic row in P.Iter(e.main_chess))foreach(Piece p in P.Iter(row))p.live=0;
        e.first=0;e.player_color=0;e.com_color=1;e.turn_id=0;e.back_num=0;
        Place(e,1,1,0);Place(e,1,2,31);
        if(!lastEnemy)Place(e,3,7,25);
        e.chess_num=P.L(1,lastEnemy?1:2);return e;
    }
    static void Place(OriginalEngine e,int r,int c,int index)
    {
        Piece p=(Piece)OriginalEngine.NewPiece(index,P.L(r,c));p.back=0;
        P.Set(P.Get(e.main_chess,r),c,p);P.Set(P.Get(e.main_map,r),c,P.L(r,c));
    }
    static int Main()
    {
        var e=new OriginalEngine();e.NewGame(0);
        Check(P.Int(e.player_first)==1,"Source seeded player-first selection");
        Check(P.Int(e.back_num)==32,"Initial hidden count");
        var seen=new bool[32];
        foreach(dynamic row in P.Iter(e.server_main_chess))foreach(Piece p in P.Iter(row)){Check(!seen[p.index],"Shuffle unique identity");seen[p.index]=true;}
        Check(e.HumanFlip(0,0),"Player first reveal");Check(P.Int(e.back_num)==31,"Reveal decrements hidden count");
        Check(P.Int(e.turn_id)==P.Int(e.com_color),"Reveal hands turn to AI");
        Check(!e.HumanFlip(0,1),"Cannot reveal twice while AI has turn");
        e.chess_ai();e.FinishComputerMove();
        Check(e.HumanTurn,"AI reply returns player turn");Check(P.Int(e.step)==2,"Player and AI each count a ply");
        e=Position(false);
        Check(!e.HumanMove(1,1,2,2),"Diagonal move rejected");Check(e.At(1,1).index==0,"Invalid move preserves origin");
        Check(!e.HumanMove(1,2,0,2),"Cannot move opponent's piece");
        Check(e.HumanMove(1,1,1,2),"Pawn captures king");Check(e.At(1,1)==null,"Capture vacates origin");
        Check(e.At(1,2).value==1,"Capture commits moving pawn");
        Check(P.Int(P.Get(e.chess_num,1))==1,"Enemy count decreases once");
        Check(P.Int(e.player_win)==0,"Enemy with remaining material has not lost");
        Check(P.Equal(P.Get(e.move_step,0),P.L(0,P.L(1,1),P.L(1,2),e.collect_possible_move(1,2,e.main_map,e.main_chess))),"Player history matches committed destination");
        e=Position(true);Check(e.HumanMove(1,1,1,2),"Final enemy capture is legal");
        Check(P.Int(e.player_win)==1,"Final capture ends game before another AI turn");
        e=Position(false);Place(e,1,1,15);Place(e,1,2,16);
        Check(!e.HumanMove(1,1,1,2),"King cannot capture pawn");
        e=Position(false);Place(e,1,1,5);Place(e,1,2,9);Place(e,1,3,31);
        Check(e.HumanMove(1,1,1,3),"Cannon jumps exactly one screen");
        Console.WriteLine("{\"success\":true,\"assertions\":"+assertions+"}");return 0;
    }
}
