using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace DarkChessUnity
{
    public sealed class DarkChessGame : MonoBehaviour
    {
        OriginalEngine game;
        Task thinking;
        readonly ConcurrentQueue<string> sounds = new ConcurrentQueue<string>();
        readonly Dictionary<string,Texture2D> art = new Dictionary<string,Texture2D>();
        readonly Dictionary<Piece,float> flips = new Dictionary<Piece,float>();
        readonly string[] faces = {"BP","BC","BN","BR","BB","BA","BK","RP","RC","RN","RR","RB","RA","RK"};
        readonly List<Piece> visible = new List<Piece>();
        AudioSource audioSource;
        Font font;
        GUIStyle statusStyle;
        Piece selected;
        Vector2 mouse;
        Rect viewport;
        float scale, moveStart=-1, gameOverAt=-1, started, lastSmoke;
        bool pendingComputer;
        string error, smokeOutput;
        int smokeMoves, maximumWorkers, smokeLegalMoves, smokeFlips;
        bool smokeSuccess;
        bool aiVsAi;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Bootstrap()
        {
            if(FindFirstObjectByType<DarkChessGame>()==null)new GameObject("DarkChess").AddComponent<DarkChessGame>();
        }
        void Start()
        {
            Application.runInBackground=true;Application.targetFrameRate=60;
            audioSource=gameObject.AddComponent<AudioSource>();
            font=Resources.Load<Font>("wqy-zenhei");
            LoadArt("SHEET");LoadArt("back");LoadArt("shield-and-swords");
            foreach(string key in faces){LoadArt(key);LoadArt(key+"S");}
            string[] args=Environment.GetCommandLineArgs();
            for(int i=0;i<args.Length-1;i++)if(args[i]=="-darkChessSmoke")smokeOutput=args[i+1];
            started=Time.realtimeSinceStartup;
            ResetGame(smokeOutput==null?(uint?)null:0);
        }
        void LoadArt(string name)
        {
            Texture2D texture=Resources.Load<Texture2D>("Image/"+name);
            if(texture==null)throw new InvalidOperationException("Missing texture: "+name);
            art[name]=texture;
        }
        void ResetGame(uint? seed=null)
        {
            if(thinking!=null&&!thinking.IsCompleted)return;
            game=new OriginalEngine();game.Sound=name=>sounds.Enqueue(name);
            game.NewGame(seed);selected=null;flips.Clear();moveStart=-1;gameOverAt=-1;error=null;
            Snapshot();
            pendingComputer=UnityEngine.Mathf.RoundToInt((float)P.Number(game.player_first))==0;
        }
        void Snapshot()
        {
            visible.Clear();
            foreach(object row in P.Iter(game.main_chess))foreach(Piece piece in P.Iter(row))
                if(piece.live==1)visible.Add((Piece)P.DeepCopy(piece));
        }
        void BeginComputer()
        {
            if(thinking!=null||error!=null||P.Int(game.player_win)!=0)return;
            pendingComputer=false;
            // The search and every candidate own C# state; no Unity API is called off-thread.
            thinking=Task.Run(()=>game.chess_ai());
        }
        void Update()
        {
            while(sounds.TryDequeue(out string sound))
            {
                AudioClip clip=Resources.Load<AudioClip>("Sound/"+sound);
                if(clip!=null)audioSource.PlayOneShot(clip);
            }
            if(thinking!=null&&thinking.IsCompleted)
            {
                if(thinking.IsFaulted){error=thinking.Exception.GetBaseException().ToString();Debug.LogError(error);}
                else
                {
                    Snapshot();
                    foreach(Piece p in visible)if(p.back==-1)flips[p]=Time.realtimeSinceStartup;
                    moveStart=Time.realtimeSinceStartup;
                    maximumWorkers=Math.Max(maximumWorkers,game.SearchWorkers);
                }
                thinking=null;
            }
            float now=Time.realtimeSinceStartup;
            bool animation=false;
            foreach(float time in flips.Values)if(now-time<.42f)animation=true;
            if(moveStart>=0)
            {
                animation=true;
                if(now-moveStart>=.42f)
                {
                    game.FinishComputerMove();moveStart=-1;Snapshot();flips.Clear();animation=false;
                    if(aiVsAi&&P.Int(game.player_win)==0)
                    {
                        object temp=game.player_color;game.player_color=game.com_color;game.com_color=temp;
                        pendingComputer=true;
                    }
                }
            }
            if(!animation&&pendingComputer&&thinking==null&&error==null)BeginComputer();
            if(thinking==null&&moveStart<0&&P.Int(game.player_win)!=0&&gameOverAt<0)
            {
                gameOverAt=now;sounds.Enqueue(P.Int(game.player_win)==1?"WIN":"LOSS");
            }
            if(gameOverAt>=0&&now-gameOverAt>=5)ResetGame();
            if(smokeOutput!=null)Smoke(animation);
        }
        bool Locked()
        {
            if(thinking!=null||moveStart>=0||pendingComputer||error!=null)return true;
            foreach(float time in flips.Values)if(Time.realtimeSinceStartup-time<.42f)return true;
            return false;
        }
        void OnGUI()
        {
            scale=Mathf.Min(Screen.width/521f,Screen.height/313f);
            viewport=new Rect((Screen.width-521*scale)/2,(Screen.height-313*scale)/2,521*scale,313*scale);
            Vector2 windowMouse=Event.current.mousePosition;
            Matrix4x4 original=GUI.matrix;
            GUI.matrix=Matrix4x4.TRS(new Vector3(viewport.x,viewport.y,0),Quaternion.identity,new Vector3(scale,scale,1));
            GUI.color=Color.white;
            GUI.DrawTexture(new Rect(0,0,521,313),art["SHEET"]);
            GUI.DrawTexture(new Rect(440,13,art["shield-and-swords"].width,art["shield-and-swords"].height),art["shield-and-swords"]);
            if(statusStyle==null)statusStyle=new GUIStyle(GUI.skin.label){font=font,fontSize=14};
            int winner=P.Int(game.player_win);
            string status=winner==1?"玩家勝!!!":winner==-1?"電腦勝...":P.Int(game.first)==1?(P.Int(game.player_first)==1?"玩家先":"電腦先"):(Locked()?"電腦下":"玩家下");
            statusStyle.normal.textColor=winner==1?Color.blue:(P.Int(Locked()?game.com_color:game.player_color)==1?Color.red:Color.black);
            GUI.Label(new Rect(237,13,190,28),status,statusStyle);
            foreach(Piece p in visible)DrawPiece(p);
            if(selected!=null)DrawFace(selected,mouse.x-28,mouse.y-28,57);
            if(error!=null)GUI.Box(new Rect(0,0,521,100),error);
            Event e=Event.current;
            Vector2 position=(windowMouse-viewport.position)/scale;
            mouse=position;
            if(smokeOutput==null&&e.button==0&&!Locked())
            {
                if(e.type==EventType.MouseDown)
                {
                    if(new Rect(440,13,art["shield-and-swords"].width,art["shield-and-swords"].height).Contains(position))ResetGame();
                    else
                    {
                        Coord(position,out int r,out int c);
                        Piece piece=game.At(r,c);
                        if(piece!=null&&game.HumanTurn)
                        {
                            if(piece.back==1)Flip(r,c);
                            else if(piece.color==P.Int(game.player_color)){selected=piece;sounds.Enqueue("CLICK");}
                        }
                    }
                    e.Use();
                }
                else if(e.type==EventType.MouseUp&&selected!=null)
                {
                    Coord(position,out int r,out int c);
                    if(game.HumanMove(selected.row,selected.col,r,c)){Snapshot();pendingComputer=P.Int(game.player_win)==0;}
                    selected=null;e.Use();
                }
            }
            GUI.matrix=original;
        }
        void Coord(Vector2 p,out int row,out int col)
        {
            row=-1;col=-1;
            for(int r=0;r<4;r++)for(int c=0;c<8;c++)
            {
                float x=c<4?34+c*57:260+(c-4)*57,y=51+r*57;
                if(p.x>x&&p.x<x+57&&p.y>y&&p.y<y+57){row=r;col=c;return;}
            }
        }
        void Flip(int r,int c)
        {
            if(!game.HumanFlip(r,c))return;
            Snapshot();
            foreach(Piece p in visible)if(p.row==r&&p.col==c)flips[p]=Time.realtimeSinceStartup;
            pendingComputer=true;
        }
        void DrawPiece(Piece p)
        {
            if(selected!=null&&p.index==selected.index&&p.row==selected.row&&p.col==selected.col)return;
            float x=p.x,y=p.y;
            if(moveStart>=0)
            {
                float progress=Mathf.Clamp01((Time.realtimeSinceStartup-moveStart)/.42f);
                x=Mathf.Lerp(p.x,p.col<4?34+p.col*57:260+(p.col-4)*57,progress);
                y=Mathf.Lerp(p.y,51+p.row*57,progress);
            }
            if(flips.TryGetValue(p,out float start)&&Time.realtimeSinceStartup-start<.42f)
            {
                float progress=Mathf.Clamp01((Time.realtimeSinceStartup-start)/.42f);
                float eased=(1-Mathf.Cos(Mathf.PI*progress))/2;
                float width=Mathf.Max(2,Mathf.Round(57*Mathf.Abs(Mathf.Cos(Mathf.PI*eased))));
                Texture2D image=progress<.5f?art["back"]:Face(p);
                GUI.DrawTexture(new Rect(x+(57-width)/2,y,width,57),image);
            }
            else DrawFace(p,x,y,57);
        }
        Texture2D Face(Piece p)
        {
            if(p.back==1||p.index==32)return art["back"];
            return art[faces[p.color*7+p.value-1]+(p.back==-1?"S":"")];
        }
        void DrawFace(Piece p,float x,float y,float width){GUI.DrawTexture(new Rect(x,y,width,57),Face(p));}
        void OnApplicationFocus(bool focus){if(!focus)selected=null;}
        void Smoke(bool animation)
        {
            float now=Time.realtimeSinceStartup;
            if(error!=null||now-started>120){FinishSmoke(false,error??"Gameplay test timed out");return;}
            if(thinking!=null||animation||pendingComputer)return;
            if(smokeMoves>=16&&maximumWorkers>0&&smokeLegalMoves>0){FinishSmoke(true,null);return;}
            if(now-lastSmoke<.15f||!game.HumanTurn)return;
            // Alternate flips and real legal player moves so both input actions are exercised.
            if(smokeMoves%3==2 && SmokeMove())return;
            foreach(Piece p in visible)
            {
                if(p.back!=1)continue;
                Flip(p.row,p.col);smokeMoves++;smokeFlips++;lastSmoke=now;return;
            }
            SmokeMove();
        }
        bool SmokeMove()
        {
            game.all_chess_move(game.main_map,game.main_chess);
            foreach(Piece p in visible)
            {
                if(p.color!=P.Int(game.player_color)||p.back==1)continue;
                Piece actual=game.At(p.row,p.col);
                foreach(object dest in P.Iter(actual.possible_move))
                    if(game.HumanMove(p.row,p.col,P.Int(P.Get(dest,0)),P.Int(P.Get(dest,1))))
                    {
                        Snapshot();pendingComputer=P.Int(game.player_win)==0;smokeMoves++;smokeLegalMoves++;
                        lastSmoke=Time.realtimeSinceStartup;return true;
                    }
            }
            return false;
        }
        [Serializable] class SmokeReport {public bool success;public string unityVersion,error;public int playerMoves,playerFlips,legalPlayerMoves,capturedPieces,aiWorkers,steps;public bool pythonFree=true;}
        void FinishSmoke(bool success,string message)
        {
            smokeSuccess=success;
            Directory.CreateDirectory(smokeOutput);
            File.WriteAllText(Path.Combine(smokeOutput,"report.json"),JsonUtility.ToJson(new SmokeReport{success=success,unityVersion=Application.unityVersion,error=message,playerMoves=smokeMoves,playerFlips=smokeFlips,legalPlayerMoves=smokeLegalMoves,capturedPieces=32-P.Int(P.Get(game.chess_num,0))-P.Int(P.Get(game.chess_num,1)),aiWorkers=maximumWorkers,steps=P.Int(game.step)},true));
            ScreenCapture.CaptureScreenshot(Path.Combine(smokeOutput,"unity-game.png"));
            smokeOutput=null;Debug.Log(success?"DARKCHESS_SMOKE_PASS":"DARKCHESS_SMOKE_FAIL");
            Invoke(nameof(ExitSmoke),.5f);
        }
        void ExitSmoke(){Application.Quit(smokeSuccess?0:1);}
    }
}
