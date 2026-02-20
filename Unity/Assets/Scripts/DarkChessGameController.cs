using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using InputSystemTouch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using InputSystemTouchPhase = UnityEngine.InputSystem.TouchPhase;

public sealed class DarkChessGameController : MonoBehaviour
{
    private const int Rows = 4;
    private const int Cols = 8;

    private const float DesignWidth = 521f;
    private const float DesignHeight = 313f;

    private const float LeftStartX = 34f;
    private const float RightStartX = 260f;
    private const float StartY = 51f;

    private const float NewGameIconX = 440f;
    private const float NewGameIconY = 13f;
    private const float AiThinkTimeSeconds = 9.5f;
    private const int MonteCarloMaxDepth = 56;
    private const float UctExploration = 1.28f;

    private static readonly Vector2Int[] CardinalDirections =
    {
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1)
    };

    private static DarkChessGameController _instance;

    private enum PieceColor
    {
        Black = 0,
        Red = 1
    }

    private enum TurnState
    {
        Player,
        AI
    }

    private enum GameResult
    {
        Playing,
        PlayerWin,
        PlayerLose,
        Draw
    }

    private sealed class Piece
    {
        public PieceColor Color;
        public int Rank;
        public bool Revealed;
        public bool Alive = true;

        public Piece(PieceColor color, int rank)
        {
            Color = color;
            Rank = rank;
        }
    }

    private struct MoveCandidate
    {
        public Vector2Int From;
        public Vector2Int To;

        public MoveCandidate(Vector2Int from, Vector2Int to)
        {
            From = from;
            To = to;
        }
    }

    private readonly struct StepRecord
    {
        public readonly PieceColor Color;
        public readonly Vector2Int Origin;
        public readonly Vector2Int Destination;
        public readonly List<Vector2Int> PossibleMoves;

        public StepRecord(PieceColor color, Vector2Int origin, Vector2Int destination, List<Vector2Int> possibleMoves)
        {
            Color = color;
            Origin = origin;
            Destination = destination;
            PossibleMoves = possibleMoves;
        }
    }

    private enum MiniActionKind
    {
        None,
        Move,
        Reveal
    }

    private readonly struct MiniDecision
    {
        public readonly MiniActionKind Kind;
        public readonly Vector2Int From;
        public readonly Vector2Int To;
        public readonly Vector2Int RevealCell;

        public MiniDecision(MiniActionKind kind, Vector2Int from, Vector2Int to, Vector2Int revealCell)
        {
            Kind = kind;
            From = from;
            To = to;
            RevealCell = revealCell;
        }
    }

    private readonly struct MiniThinkMove
    {
        public readonly Vector2Int? Origin;
        public readonly Vector2Int? Destination;
        public readonly float Score;

        public MiniThinkMove(Vector2Int? origin, Vector2Int? destination, float score)
        {
            Origin = origin;
            Destination = destination;
            Score = score;
        }
    }

    private readonly struct MiniTurnMove
    {
        public readonly Vector2Int? RootOrigin;
        public readonly Vector2Int? RootDestination;
        public readonly Vector2Int? Origin;
        public readonly Vector2Int? Destination;
        public readonly float Score;

        public MiniTurnMove(Vector2Int? rootOrigin, Vector2Int? rootDestination, Vector2Int? origin, Vector2Int? destination, float score)
        {
            RootOrigin = rootOrigin;
            RootDestination = rootDestination;
            Origin = origin;
            Destination = destination;
            Score = score;
        }
    }

    private enum SimulationResult
    {
        Ongoing,
        AiWin,
        PlayerWin,
        Draw
    }

    private Piece[,] _board = new Piece[Rows, Cols];
    private readonly Image[,] _pieceImages = new Image[Rows, Cols];
    private readonly Vector2[,] _cellTopLeft = new Vector2[Rows, Cols];
    private readonly Vector2[,] _cellCenter = new Vector2[Rows, Cols];

    private readonly Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();
    private readonly Dictionary<string, AudioClip> _clips = new Dictionary<string, AudioClip>();

    private readonly System.Random _random = new System.Random();

    private Canvas _canvas;
    private RectTransform _boardRoot;
    private Text _statusText;
    private AudioSource _audioSource;
    private Material _uiImageMaterial;
    private Material _uiTextMaterial;

    private float _cellWidth;
    private float _cellHeight;
    private Rect _newGameRect;

    private Vector2Int? _selectedCell;
    private bool _isDragging;

    private bool _playerFirst;
    private bool _firstMove;
    private bool _colorsAssigned;
    private PieceColor _playerColor;
    private PieceColor _aiColor;
    private TurnState _turn;
    private GameResult _result;

    private readonly int[,] _backValueNum = new int[2, 8];
    private readonly int[] _kingLive = new int[2];
    private readonly List<Vector2Int> _comWillEatChess = new List<Vector2Int>();
    private readonly List<Vector2Int> _willEatEscapeChess = new List<Vector2Int>();
    private readonly List<Vector2Int> _cannonCor = new List<Vector2Int>();
    private readonly List<Vector2Int> _comBanStep = new List<Vector2Int>();
    private readonly List<Vector2Int[]> _breakLongCaptureDest = new List<Vector2Int[]>();
    private readonly List<Vector2Int[]> _breakLongCaptureOrg = new List<Vector2Int[]>();
    private readonly StepRecord?[] _moveStep = new StepRecord?[4];

    private readonly int[,] _evalMark = new int[Rows, Cols];
    private readonly int[,] _evalCannonMark = new int[Rows, Cols];

    private int _sindex;
    private float _aiMinScore = 2000f;
    private float? _openScore;
    private float _evalMaxValue;
    private int _evalMaxDist = 32;
    private Vector2Int? _pendingAiRevealCell;

    private float _aiActionTime;
    private bool _endSoundPlayed;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void BootstrapIfMissing()
    {
        if (FindObjectOfType<DarkChessGameController>() != null)
        {
            return;
        }

        var go = new GameObject("DarkChessGameController");
        go.AddComponent<DarkChessGameController>();
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        EnhancedTouchSupport.Enable();
        ConfigureOrientation();
        EnsureCameraAndAudioListener();
        EnsureAudioSource();
        LoadAssets();
        BuildUI();
        StartNewGame();
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            EnhancedTouchSupport.Disable();
            _instance = null;
        }

        if (_uiImageMaterial != null)
        {
            Destroy(_uiImageMaterial);
            _uiImageMaterial = null;
        }

        if (_uiTextMaterial != null)
        {
            Destroy(_uiTextMaterial);
            _uiTextMaterial = null;
        }
    }

    private void Update()
    {
        if (_result == GameResult.Playing)
        {
            HandleInput();

            if (_result == GameResult.Playing && _turn == TurnState.AI && Time.unscaledTime >= _aiActionTime)
            {
                RunAITurn();
            }

            if (_result == GameResult.Playing && _colorsAssigned)
            {
                EvaluateNoMoveForCurrentTurn();
            }
        }
        else
        {
            HandleInput();
            PlayEndSoundIfNeeded();
        }

        UpdateStatusText();
    }

    private void ConfigureOrientation()
    {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.AutoRotation;

        Application.targetFrameRate = 60;
    }

    private void EnsureCameraAndAudioListener()
    {
        var mainCamera = Camera.main;
        if (mainCamera == null)
        {
            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";

            mainCamera = camGo.AddComponent<Camera>();
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = new Color(0.05f, 0.07f, 0.1f, 1f);
            mainCamera.transform.position = new Vector3(0f, 0f, -10f);

            camGo.AddComponent<AudioListener>();
            return;
        }

        if (FindObjectOfType<AudioListener>() == null)
        {
            mainCamera.gameObject.AddComponent<AudioListener>();
        }
    }

    private void EnsureAudioSource()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        _audioSource.playOnAwake = false;
        _audioSource.loop = false;
        _audioSource.spatialBlend = 0f;
    }

    private void LoadAssets()
    {
        var spriteNames = new List<string>
        {
            "SHEET",
            "back",
            "shield-and-swords",
            "BA",
            "BAS",
            "BB",
            "BBS",
            "BC",
            "BCS",
            "BK",
            "BKS",
            "BN",
            "BNS",
            "BP",
            "BPS",
            "BR",
            "BRS",
            "RA",
            "RAS",
            "RB",
            "RBS",
            "RC",
            "RCS",
            "RK",
            "RKS",
            "RN",
            "RNS",
            "RP",
            "RPS",
            "RR",
            "RRS"
        };

        foreach (var name in spriteNames)
        {
            var sprite = LoadSpriteFlexible(name);
            if (sprite != null)
            {
                _sprites[name] = sprite;
            }
            else
            {
                Debug.LogWarning($"Missing sprite: Image/{name}");
            }
        }

        LoadClip("NEWGAME");
        LoadClip("CLICK");
        LoadClip("MOVE2");
        LoadClip("CAPTURE2");
        LoadClip("WIN");
        LoadClip("LOSS");
    }

    private Sprite LoadSpriteFlexible(string name)
    {
        var sprite = Resources.Load<Sprite>($"Image/{name}");
        if (sprite != null)
        {
            return sprite;
        }

        var texture = Resources.Load<Texture2D>($"Image/{name}");
        if (texture == null)
        {
            return null;
        }

        return Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f);
    }

    private void LoadClip(string name)
    {
        var clip = Resources.Load<AudioClip>($"Sound/{name}");
        if (clip != null)
        {
            _clips[name] = clip;
        }
        else
        {
            Debug.LogWarning($"Missing clip: Sound/{name}");
        }
    }

    private void BuildUI()
    {
        EnsureUIMaterials();

        var canvasGo = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        _canvas = canvasGo.GetComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.pixelPerfect = false;

        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(DesignWidth, DesignHeight);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 1f;

        var canvasRect = canvasGo.GetComponent<RectTransform>();

        var backdrop = CreateImage("Backdrop", canvasRect, null);
        backdrop.color = new Color(0.06f, 0.08f, 0.1f, 1f);
        var backdropRect = backdrop.rectTransform;
        backdropRect.anchorMin = Vector2.zero;
        backdropRect.anchorMax = Vector2.one;
        backdropRect.offsetMin = Vector2.zero;
        backdropRect.offsetMax = Vector2.zero;

        var boardRootGo = new GameObject("BoardRoot", typeof(RectTransform));
        _boardRoot = boardRootGo.GetComponent<RectTransform>();
        _boardRoot.SetParent(canvasRect, false);
        _boardRoot.anchorMin = new Vector2(0.5f, 0.5f);
        _boardRoot.anchorMax = new Vector2(0.5f, 0.5f);
        _boardRoot.pivot = new Vector2(0.5f, 0.5f);
        _boardRoot.sizeDelta = new Vector2(DesignWidth, DesignHeight);
        _boardRoot.anchoredPosition = Vector2.zero;

        var bgSprite = GetSprite("SHEET");
        var bgImage = CreateImage("Background", _boardRoot, bgSprite);
        bgImage.preserveAspect = true;
        SetElementAtTopLeft(bgImage.rectTransform, 0f, 0f, DesignWidth, DesignHeight);

        var iconSprite = GetSprite("shield-and-swords");
        var iconWidth = iconSprite != null ? iconSprite.rect.width : 64f;
        var iconHeight = iconSprite != null ? iconSprite.rect.height : 64f;
        _newGameRect = new Rect(NewGameIconX, NewGameIconY, iconWidth, iconHeight);

        var newGameImage = CreateImage("NewGame", _boardRoot, iconSprite);
        newGameImage.preserveAspect = true;
        SetElementAtTopLeft(newGameImage.rectTransform, NewGameIconX, NewGameIconY, iconWidth, iconHeight);

        var statusGo = new GameObject("StatusText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        statusGo.transform.SetParent(_boardRoot, false);
        _statusText = statusGo.GetComponent<Text>();
        _statusText.font = Resources.Load<Font>("Fonts/wqy-zenhei") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        _statusText.fontSize = 14;
        _statusText.alignment = TextAnchor.UpperCenter;
        _statusText.horizontalOverflow = HorizontalWrapMode.Wrap;
        _statusText.verticalOverflow = VerticalWrapMode.Overflow;
        _statusText.color = new Color(0.15f, 0.1f, 0.05f, 1f);
        if (_uiTextMaterial != null)
        {
            _statusText.material = _uiTextMaterial;
        }

        var statusRect = _statusText.rectTransform;
        statusRect.anchorMin = new Vector2(0.5f, 1f);
        statusRect.anchorMax = new Vector2(0.5f, 1f);
        statusRect.pivot = new Vector2(0.5f, 1f);
        statusRect.sizeDelta = new Vector2(320f, 76f);
        statusRect.anchoredPosition = new Vector2(-2f, -2f);

        var backSprite = GetSprite("back");
        _cellWidth = backSprite != null ? backSprite.rect.width : 52f;
        _cellHeight = backSprite != null ? backSprite.rect.height : 52f;

        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var topLeft = CellTopLeft(row, col);
                _cellTopLeft[row, col] = topLeft;
                _cellCenter[row, col] = TopLeftToAnchored(topLeft, _cellWidth, _cellHeight);

                var pieceImage = CreateImage($"Piece_{row}_{col}", _boardRoot, backSprite);
                pieceImage.preserveAspect = true;
                pieceImage.rectTransform.sizeDelta = new Vector2(_cellWidth, _cellHeight);
                pieceImage.rectTransform.anchoredPosition = _cellCenter[row, col];
                _pieceImages[row, col] = pieceImage;
            }
        }
    }

    private void HandleInput()
    {
        if (TryGetPointerDown(out var downScreen))
        {
            HandlePointerDown(downScreen);
        }

        if (_selectedCell.HasValue && _isDragging && TryGetPointerHeld(out var heldScreen))
        {
            if (TryScreenToLocal(heldScreen, out var local))
            {
                var cell = _selectedCell.Value;
                _pieceImages[cell.x, cell.y].rectTransform.anchoredPosition = local;
            }
        }

        if (_selectedCell.HasValue && TryGetPointerUp(out var upScreen))
        {
            HandlePointerUp(upScreen);
        }
    }

    private void HandlePointerDown(Vector2 screenPos)
    {
        if (!TryScreenToPixel(screenPos, out var pixel))
        {
            return;
        }

        if (ContainsTopLeftRect(_newGameRect, pixel))
        {
            StartNewGame();
            return;
        }

        if (_result != GameResult.Playing || _turn != TurnState.Player)
        {
            return;
        }

        if (!TryPixelToCell(pixel, out var cell))
        {
            return;
        }

        var piece = _board[cell.x, cell.y];
        if (piece == null)
        {
            return;
        }

        if (!piece.Revealed)
        {
            RevealPiece(cell, true);
            return;
        }

        if (!_colorsAssigned || piece.Color != _playerColor)
        {
            return;
        }

        if (CollectLegalMoves(cell).Count == 0)
        {
            return;
        }

        SelectCell(cell);
    }

    private void HandlePointerUp(Vector2 screenPos)
    {
        if (!_selectedCell.HasValue)
        {
            return;
        }

        var from = _selectedCell.Value;
        var validMoves = CollectLegalMoves(from);

        if (TryScreenToPixel(screenPos, out var pixel) && TryPixelToCell(pixel, out var to) && ContainsCell(validMoves, to))
        {
            ExecuteMove(from, to, true);
        }

        ClearSelection(true);
    }

    private void SelectCell(Vector2Int cell)
    {
        ClearSelection(false);

        _selectedCell = cell;
        _isDragging = true;

        var image = _pieceImages[cell.x, cell.y];
        image.transform.SetAsLastSibling();
        RefreshCell(cell.x, cell.y, true);
    }

    private void ClearSelection(bool snapToGrid)
    {
        if (!_selectedCell.HasValue)
        {
            return;
        }

        var cell = _selectedCell.Value;

        _isDragging = false;
        _selectedCell = null;

        if (snapToGrid)
        {
            _pieceImages[cell.x, cell.y].rectTransform.anchoredPosition = _cellCenter[cell.x, cell.y];
        }

        RefreshCell(cell.x, cell.y, false);
    }

    private void StartNewGame()
    {
        _board = new Piece[Rows, Cols];
        _selectedCell = null;
        _isDragging = false;

        _playerFirst = _random.Next(0, 2) == 1;
        _firstMove = true;
        _colorsAssigned = false;

        _playerColor = PieceColor.Black;
        _aiColor = PieceColor.Red;

        _turn = _playerFirst ? TurnState.Player : TurnState.AI;
        _result = GameResult.Playing;
        _endSoundPlayed = false;

        var pool = BuildPiecePool();
        Shuffle(pool);

        var idx = 0;
        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                _board[row, col] = pool[idx++];
            }
        }

        ResetMiniMaxState();

        RefreshAllCells();
        PlaySound("NEWGAME");

        if (_turn == TurnState.AI)
        {
            ScheduleAI();
        }
    }

    private List<Piece> BuildPiecePool()
    {
        var pool = new List<Piece>(Rows * Cols);

        for (var color = 0; color <= 1; color++)
        {
            var pieceColor = (PieceColor)color;

            AddPieces(pool, pieceColor, 1, 5);
            AddPieces(pool, pieceColor, 2, 2);
            AddPieces(pool, pieceColor, 3, 2);
            AddPieces(pool, pieceColor, 4, 2);
            AddPieces(pool, pieceColor, 5, 2);
            AddPieces(pool, pieceColor, 6, 2);
            AddPieces(pool, pieceColor, 7, 1);
        }

        return pool;
    }

    private static void AddPieces(List<Piece> pool, PieceColor color, int rank, int count)
    {
        for (var i = 0; i < count; i++)
        {
            pool.Add(new Piece(color, rank));
        }
    }

    private void Shuffle(List<Piece> list)
    {
        for (var i = list.Count - 1; i > 0; i--)
        {
            var swapIndex = _random.Next(0, i + 1);
            (list[i], list[swapIndex]) = (list[swapIndex], list[i]);
        }
    }

    private void RunAITurn()
    {
        if (_result != GameResult.Playing || _turn != TurnState.AI)
        {
            return;
        }

        if (_firstMove && !_playerFirst)
        {
            if (!RevealRandomHidden(false))
            {
                SetResult(GameResult.Draw);
            }

            return;
        }

        _comWillEatChess.Clear();
        _willEatEscapeChess.Clear();
        UpdateLongCaptureBan();

        if (TryPickBestAIMove(out var from, out var to))
        {
            if (_pendingAiRevealCell.HasValue)
            {
                var revealCell = _pendingAiRevealCell.Value;
                _pendingAiRevealCell = null;
                RevealPiece(revealCell, false);
                return;
            }

            ExecuteMove(from, to, false);
            return;
        }

        if (HiddenCount() > 0)
        {
            _pendingAiRevealCell = null;
            RevealRandomHidden(false);
            return;
        }

        SetResult(GameResult.PlayerWin);
    }

    private bool TryPickBestAIMove(out Vector2Int from, out Vector2Int to)
    {
        _pendingAiRevealCell = null;

        var board = CloneBoard(_board);
        var decision = MiniPickAction(board);
        if (decision.Kind == MiniActionKind.Move)
        {
            from = decision.From;
            to = decision.To;
            return true;
        }

        if (decision.Kind == MiniActionKind.Reveal)
        {
            _pendingAiRevealCell = decision.RevealCell;
            from = default;
            to = default;
            return true;
        }

        from = default;
        to = default;
        return false;
    }

    private List<MoveCandidate> CollectAllMoves(Piece[,] board, PieceColor color)
    {
        var moves = new List<MoveCandidate>();
        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var piece = board[row, col];
                if (piece == null || !piece.Revealed || piece.Color != color)
                {
                    continue;
                }

                var from = new Vector2Int(row, col);
                var legalMoves = CollectLegalMoves(board, from);
                foreach (var to in legalMoves)
                {
                    moves.Add(new MoveCandidate(from, to));
                }
            }
        }

        return moves;
    }

    private float EvaluateTacticalBias(MoveCandidate move)
    {
        var mover = _board[move.From.x, move.From.y];
        var target = _board[move.To.x, move.To.y];

        var bias = 0f;
        if (target != null)
        {
            bias += 0.18f + target.Rank * 0.03f;
            if (mover != null && mover.Rank == 1 && target.Rank == 7)
            {
                bias += 0.08f;
            }
        }
        else
        {
            bias += 0.01f;
        }

        if (mover != null && WouldBeCapturable(_board, move.From, move.To, mover.Color))
        {
            bias -= 0.12f;
        }

        return bias;
    }

    private float SimulateCandidate(MoveCandidate candidate)
    {
        var simulationBoard = CloneBoard(_board);
        ApplyMove(simulationBoard, candidate.From, candidate.To);

        var immediate = EvaluateSimulationState(simulationBoard);
        if (immediate != SimulationResult.Ongoing)
        {
            return ScoreSimulation(immediate);
        }

        return RunMonteCarloPlayout(simulationBoard, _playerColor);
    }

    private int SelectCandidateByUct(List<MoveCandidate> candidates, int[] visits, float[] values, int totalVisits)
    {
        var bestIndex = 0;
        var bestScore = float.NegativeInfinity;
        var logTotal = Mathf.Log(totalVisits + 1f);

        for (var i = 0; i < candidates.Count; i++)
        {
            if (visits[i] == 0)
            {
                return i;
            }

            var mean = values[i] / visits[i];
            var explore = UctExploration * Mathf.Sqrt(logTotal / visits[i]);
            var prior = EvaluateTacticalBias(candidates[i]) * (0.24f / (1f + visits[i] * 0.12f));
            var score = mean + explore + prior;

            if (score > bestScore || (Mathf.Abs(score - bestScore) < 0.0001f && _random.Next(0, 2) == 0))
            {
                bestScore = score;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    private MoveCandidate SelectPlayoutMove(Piece[,] board, PieceColor turnColor, List<MoveCandidate> moves)
    {
        if (moves.Count == 1)
        {
            return moves[0];
        }

        if (_random.NextDouble() < 0.24d)
        {
            return moves[_random.Next(0, moves.Count)];
        }

        var bestMove = moves[0];
        var bestScore = float.NegativeInfinity;
        foreach (var move in moves)
        {
            var score = EvaluatePlayoutMove(board, move, turnColor);
            if (score > bestScore || (Mathf.Abs(score - bestScore) < 0.0001f && _random.Next(0, 2) == 0))
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestMove;
    }

    private float EvaluatePlayoutMove(Piece[,] board, MoveCandidate move, PieceColor turnColor)
    {
        var mover = board[move.From.x, move.From.y];
        var target = board[move.To.x, move.To.y];

        var score = 0f;
        if (target != null)
        {
            score += 0.22f + target.Rank * 0.06f;
            if (mover != null && mover.Rank == 1 && target.Rank == 7)
            {
                score += 0.18f;
            }
        }
        else
        {
            score += 0.01f;
        }

        if (mover != null && WouldBeCapturable(board, move.From, move.To, turnColor))
        {
            score -= 0.16f;
        }

        var moving = board[move.From.x, move.From.y];
        var captured = board[move.To.x, move.To.y];
        ApplyMove(board, move.From, move.To);
        var mobility = CollectLegalMoves(board, move.To).Count;
        board[move.From.x, move.From.y] = moving;
        board[move.To.x, move.To.y] = captured;

        score += mobility * 0.012f;
        return score;
    }

    private float RunMonteCarloPlayout(Piece[,] board, PieceColor turnColor)
    {
        for (var depth = 0; depth < MonteCarloMaxDepth; depth++)
        {
            var state = EvaluateSimulationState(board);
            if (state != SimulationResult.Ongoing)
            {
                return ScoreSimulation(state);
            }

            var moves = CollectAllMoves(board, turnColor);
            if (moves.Count > 0)
            {
                var move = SelectPlayoutMove(board, turnColor, moves);
                ApplyMove(board, move.From, move.To);
            }
            else
            {
                var hidden = GetHiddenCells(board);
                if (hidden.Count == 0)
                {
                    return turnColor == _aiColor ? -1f : 1f;
                }

                var reveal = hidden[_random.Next(0, hidden.Count)];
                var piece = board[reveal.x, reveal.y];
                if (piece != null)
                {
                    piece.Revealed = true;
                }
            }

            turnColor = Opponent(turnColor);
        }

        return EvaluateMaterialHeuristic(board);
    }

    private SimulationResult EvaluateSimulationState(Piece[,] board)
    {
        var aiCount = CountPieces(board, _aiColor);
        var playerCount = CountPieces(board, _playerColor);
        if (playerCount <= 0)
        {
            return SimulationResult.AiWin;
        }

        if (aiCount <= 0)
        {
            return SimulationResult.PlayerWin;
        }

        if (HiddenCount(board) == 0)
        {
            var aiCanMove = HasAnyLegalMove(board, _aiColor);
            var playerCanMove = HasAnyLegalMove(board, _playerColor);

            if (!aiCanMove && !playerCanMove)
            {
                return SimulationResult.Draw;
            }

            if (!aiCanMove)
            {
                return SimulationResult.PlayerWin;
            }

            if (!playerCanMove)
            {
                return SimulationResult.AiWin;
            }
        }

        return SimulationResult.Ongoing;
    }

    private static float ScoreSimulation(SimulationResult result)
    {
        return result switch
        {
            SimulationResult.AiWin => 1f,
            SimulationResult.PlayerWin => -1f,
            _ => 0f
        };
    }

    private float EvaluateMaterialHeuristic(Piece[,] board)
    {
        var aiPieces = CountPieces(board, _aiColor);
        var playerPieces = CountPieces(board, _playerColor);
        var aiRevealed = CountRevealedPieces(board, _aiColor);
        var playerRevealed = CountRevealedPieces(board, _playerColor);

        var pieceScore = (aiPieces - playerPieces) / 16f;
        var revealScore = ((aiRevealed - playerRevealed) / 16f) * 0.3f;

        return Mathf.Clamp(pieceScore + revealScore, -1f, 1f);
    }

    private static Piece[,] CloneBoard(Piece[,] source)
    {
        var clone = new Piece[Rows, Cols];
        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var piece = source[row, col];
                if (piece == null)
                {
                    continue;
                }

                clone[row, col] = new Piece(piece.Color, piece.Rank)
                {
                    Revealed = piece.Revealed,
                    Alive = piece.Alive
                };
            }
        }

        return clone;
    }

    private static void ApplyMove(Piece[,] board, Vector2Int from, Vector2Int to)
    {
        if (from == to)
        {
            return;
        }

        board[to.x, to.y] = board[from.x, from.y];
        board[from.x, from.y] = null;
    }

    private int CountRevealedPieces(Piece[,] board, PieceColor color)
    {
        var count = 0;
        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var piece = board[row, col];
                if (piece != null && piece.Revealed && piece.Color == color)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private bool WouldBeCapturable(Piece[,] board, Vector2Int from, Vector2Int to, PieceColor moverColor)
    {
        var moving = board[from.x, from.y];
        var captured = board[to.x, to.y];

        board[to.x, to.y] = moving;
        board[from.x, from.y] = null;

        var threatened = IsCellCapturableByColor(board, to, Opponent(moverColor));

        board[from.x, from.y] = moving;
        board[to.x, to.y] = captured;

        return threatened;
    }

    private bool IsCellCapturableByColor(Piece[,] board, Vector2Int target, PieceColor attackerColor)
    {
        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var piece = board[row, col];
                if (piece == null || !piece.Revealed || piece.Color != attackerColor)
                {
                    continue;
                }

                var moves = CollectLegalMoves(board, new Vector2Int(row, col));
                if (ContainsCell(moves, target))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool RevealRandomHidden(bool byPlayer)
    {
        var hidden = GetHiddenCells();
        if (hidden.Count == 0)
        {
            return false;
        }

        var pick = hidden[_random.Next(0, hidden.Count)];
        RevealPiece(pick, byPlayer);
        return true;
    }

    private List<Vector2Int> GetHiddenCells()
    {
        return GetHiddenCells(_board);
    }

    private List<Vector2Int> GetHiddenCells(Piece[,] board)
    {
        var list = new List<Vector2Int>();

        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var piece = board[row, col];
                if (piece != null && !piece.Revealed)
                {
                    list.Add(new Vector2Int(row, col));
                }
            }
        }

        return list;
    }

    private void RevealPiece(Vector2Int cell, bool byPlayer)
    {
        var piece = _board[cell.x, cell.y];
        if (piece == null || piece.Revealed)
        {
            return;
        }

        piece.Revealed = true;
        var colorIdx = ColorToInt(piece.Color);
        if (piece.Rank >= 1 && piece.Rank <= 7 && _backValueNum[colorIdx, piece.Rank] > 0)
        {
            _backValueNum[colorIdx, piece.Rank]--;
        }
        PlaySound("CLICK");
        RefreshCell(cell.x, cell.y, false);

        if (_firstMove)
        {
            if (byPlayer)
            {
                _playerColor = piece.Color;
                _aiColor = Opponent(_playerColor);
                _turn = TurnState.AI;
                ScheduleAI();
            }
            else
            {
                _aiColor = piece.Color;
                _playerColor = Opponent(_aiColor);
                _turn = TurnState.Player;
            }

            _colorsAssigned = true;
            _firstMove = false;
        }
        else
        {
            _turn = byPlayer ? TurnState.AI : TurnState.Player;
            if (_turn == TurnState.AI)
            {
                ScheduleAI();
            }
        }

        EvaluateMaterialResult();
    }

    private void ExecuteMove(Vector2Int from, Vector2Int to, bool byPlayer)
    {
        var moving = _board[from.x, from.y];
        if (moving == null)
        {
            return;
        }
        var moverColor = moving.Color;

        var target = _board[to.x, to.y];
        if (target != null)
        {
            target.Alive = false;
            PlaySound("CAPTURE2");
        }
        else
        {
            PlaySound("MOVE2");
        }

        _board[to.x, to.y] = moving;
        _board[from.x, from.y] = null;

        RecordStep(moverColor, from, to);

        _pieceImages[to.x, to.y].rectTransform.anchoredPosition = _cellCenter[to.x, to.y];
        _pieceImages[from.x, from.y].rectTransform.anchoredPosition = _cellCenter[from.x, from.y];

        RefreshCell(from.x, from.y, false);
        RefreshCell(to.x, to.y, false);

        EvaluateMaterialResult();
        if (_result != GameResult.Playing)
        {
            return;
        }

        _turn = byPlayer ? TurnState.AI : TurnState.Player;

        if (_turn == TurnState.AI)
        {
            ScheduleAI();
        }
    }

    private void ResetMiniMaxState()
    {
        Array.Clear(_backValueNum, 0, _backValueNum.Length);

        for (var color = 0; color <= 1; color++)
        {
            _backValueNum[color, 1] = 5;
            _backValueNum[color, 2] = 2;
            _backValueNum[color, 3] = 2;
            _backValueNum[color, 4] = 2;
            _backValueNum[color, 5] = 2;
            _backValueNum[color, 6] = 2;
            _backValueNum[color, 7] = 1;
        }

        _kingLive[0] = 1;
        _kingLive[1] = 1;

        _comWillEatChess.Clear();
        _willEatEscapeChess.Clear();
        _cannonCor.Clear();
        _comBanStep.Clear();
        _breakLongCaptureDest.Clear();
        _breakLongCaptureOrg.Clear();

        for (var i = 0; i < _moveStep.Length; i++)
        {
            _moveStep[i] = null;
        }

        _sindex = 0;
        _aiMinScore = 2000f;
        _openScore = null;
        _pendingAiRevealCell = null;
    }

    private static int ColorToInt(PieceColor color)
    {
        return color == PieceColor.Black ? 0 : 1;
    }

    private int OpponentColor(int color)
    {
        return 1 - color;
    }

    private void RecordStep(PieceColor moverColor, Vector2Int origin, Vector2Int destination)
    {
        var possibleMoves = CollectLegalMoves(_board, destination);
        _moveStep[_sindex] = new StepRecord(moverColor, origin, destination, possibleMoves);
        _sindex = (_sindex + 1) % 4;

        RemoveBreakLongCaptureByDestination(destination);
        RemoveBreakLongCaptureByOrigin(origin);
    }

    private void RemoveBreakLongCaptureByDestination(Vector2Int destination)
    {
        var idx = 0;
        while (idx < _breakLongCaptureDest.Count)
        {
            var remove = false;
            var list = _breakLongCaptureDest[idx];
            for (var i = 0; i < list.Length; i++)
            {
                if (list[i] == destination)
                {
                    remove = true;
                    break;
                }
            }

            if (remove)
            {
                _breakLongCaptureDest.RemoveAt(idx);
                _breakLongCaptureOrg.RemoveAt(idx);
                _comBanStep.RemoveAt(idx);
                continue;
            }

            idx++;
        }
    }

    private void RemoveBreakLongCaptureByOrigin(Vector2Int origin)
    {
        var idx = 0;
        while (idx < _breakLongCaptureOrg.Count)
        {
            var remove = false;
            var list = _breakLongCaptureOrg[idx];
            for (var i = 0; i < list.Length; i++)
            {
                if (list[i] == origin)
                {
                    remove = true;
                    break;
                }
            }

            if (remove)
            {
                _breakLongCaptureDest.RemoveAt(idx);
                _breakLongCaptureOrg.RemoveAt(idx);
                _comBanStep.RemoveAt(idx);
                continue;
            }

            idx++;
        }
    }

    private void UpdateLongCaptureBan()
    {
        var movePre1 = _moveStep[(_sindex - 1 + 4) % 4];
        var movePre2 = _moveStep[(_sindex - 2 + 4) % 4];
        var movePre3 = _moveStep[(_sindex - 3 + 4) % 4];
        var movePre4 = _moveStep[_sindex];

        if (!movePre1.HasValue || !movePre2.HasValue || !movePre3.HasValue || !movePre4.HasValue)
        {
            return;
        }

        var m1 = movePre1.Value;
        var m2 = movePre2.Value;
        var m3 = movePre3.Value;
        var m4 = movePre4.Value;

        if (m1.Color != _playerColor || m2.Color != _aiColor || m3.Color != _playerColor || m4.Color != _aiColor)
        {
            return;
        }

        if (!ContainsCell(m2.PossibleMoves, m1.Origin) || !ContainsCell(m4.PossibleMoves, m3.Origin))
        {
            return;
        }

        if (m2.Destination != m4.Origin || m1.Destination != m3.Origin)
        {
            return;
        }

        var n1 = m1.Origin;
        var n2 = m2.Origin;
        var p = m1.Destination;
        var c = m2.Destination;

        _breakLongCaptureDest.Add(new[] { n1, n2, p, c });
        _breakLongCaptureOrg.Add(new[] { p, c });
        _comBanStep.Add(m4.Origin);
    }

    private MiniDecision MiniPickAction(Piece[,] board)
    {
        var thinkResult = MiniComThink(board);
        var origin = thinkResult.Origin;
        var destination = thinkResult.Destination;
        var score = thinkResult.Score;
        var backNum = HiddenCount(board);

        if (backNum > 0)
        {
            if (_openScore.HasValue)
            {
                var randomOffset = _random.Next(1, 10);
                if (!origin.HasValue)
                {
                    var revealCell = MiniSelectBackChess(board, null);
                    if (revealCell.HasValue && !IsMoveSentinel(revealCell.Value))
                    {
                        return new MiniDecision(MiniActionKind.Reveal, default, default, revealCell.Value);
                    }
                }
                else if (score > _openScore.Value - randomOffset / 10f)
                {
                    var moveOrigin = origin;
                    if (score > 18f)
                    {
                        moveOrigin = null;
                    }

                    var revealOrMove = MiniSelectBackChess(board, moveOrigin);
                    if (revealOrMove.HasValue)
                    {
                        if (IsMoveSentinel(revealOrMove.Value) && origin.HasValue && destination.HasValue)
                        {
                            return new MiniDecision(MiniActionKind.Move, origin.Value, destination.Value, default);
                        }

                        if (!IsMoveSentinel(revealOrMove.Value))
                        {
                            return new MiniDecision(MiniActionKind.Reveal, default, default, revealOrMove.Value);
                        }
                    }
                }
                else if (Mathf.Abs(score - _openScore.Value) < 0.0001f)
                {
                    if (score >= 0f)
                    {
                        var moveOrigin = origin;
                        if (score > 18f)
                        {
                            moveOrigin = null;
                        }

                        var revealOrMove = MiniSelectBackChess(board, moveOrigin);
                        if (revealOrMove.HasValue)
                        {
                            if (IsMoveSentinel(revealOrMove.Value) && origin.HasValue && destination.HasValue)
                            {
                                return new MiniDecision(MiniActionKind.Move, origin.Value, destination.Value, default);
                            }

                            if (!IsMoveSentinel(revealOrMove.Value))
                            {
                                return new MiniDecision(MiniActionKind.Reveal, default, default, revealOrMove.Value);
                            }
                        }
                    }
                    else if (origin.HasValue && destination.HasValue)
                    {
                        return new MiniDecision(MiniActionKind.Move, origin.Value, destination.Value, default);
                    }
                }
                else if (origin.HasValue && destination.HasValue)
                {
                    return new MiniDecision(MiniActionKind.Move, origin.Value, destination.Value, default);
                }
            }
            else if (origin.HasValue && destination.HasValue)
            {
                return new MiniDecision(MiniActionKind.Move, origin.Value, destination.Value, default);
            }
        }
        else if (origin.HasValue && destination.HasValue)
        {
            return new MiniDecision(MiniActionKind.Move, origin.Value, destination.Value, default);
        }

        if (origin.HasValue && destination.HasValue)
        {
            return new MiniDecision(MiniActionKind.Move, origin.Value, destination.Value, default);
        }

        if (backNum > 0)
        {
            var revealCell = MiniSelectBackChess(board, null);
            if (revealCell.HasValue && !IsMoveSentinel(revealCell.Value))
            {
                return new MiniDecision(MiniActionKind.Reveal, default, default, revealCell.Value);
            }

            var hidden = GetHiddenCells(board);
            if (hidden.Count > 0)
            {
                return new MiniDecision(MiniActionKind.Reveal, default, default, hidden[_random.Next(0, hidden.Count)]);
            }
        }

        return new MiniDecision(MiniActionKind.None, default, default, default);
    }

    private MiniThinkMove MiniComThink(Piece[,] board)
    {
        var moves = new List<MiniThinkMove>();

        var minScore = 1000f;
        var currentScore = 0f;
        Vector2Int? origin = null;
        Vector2Int? destination = null;

        MiniScanKing(board);

        if (HiddenCount(board) > 0)
        {
            _openScore = 0f;
            moves.Add(new MiniThinkMove(null, null, 0f));
            minScore = 0f;
        }
        else
        {
            _openScore = null;
        }

        _aiMinScore = 2000f;

        var aiColorInt = ColorToInt(_aiColor);
        var playerColorInt = ColorToInt(_playerColor);

        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var piece = board[row, col];
                if (piece == null || !piece.Revealed || piece.Color != _aiColor)
                {
                    continue;
                }

                var from = new Vector2Int(row, col);
                var possibleMoves = CollectLegalMoves(board, from);
                foreach (var move in possibleMoves)
                {
                    float score;
                    if (MiniWillDeadPity(from, move, board, aiColorInt) == 0)
                    {
                        score = currentScore - MiniMoveScore(from, move, board, aiColorInt);
                    }
                    else
                    {
                        score = currentScore + 50f - MiniMoveScore(from, move, board, aiColorInt);
                    }

                    moves.Add(new MiniThinkMove(from, move, score));
                    if (score < minScore)
                    {
                        minScore = score;
                        origin = from;
                        destination = move;
                    }
                }
            }
        }

        if (moves.Count > 1)
        {
            var finalMoves = new List<MiniThinkMove>();
            for (var i = 0; i < moves.Count; i++)
            {
                var rootMove = moves[i];
                var nextLayer = MiniOneTurn(board, rootMove, playerColorInt, rootMove.Origin, rootMove.Destination, rootMove.Score, 0.9f);
                if (nextLayer.Count == 0)
                {
                    continue;
                }

                var bestIndex = 0;
                var bestScore = float.NegativeInfinity;
                for (var j = 0; j < nextLayer.Count; j++)
                {
                    if (nextLayer[j].Score > bestScore)
                    {
                        bestScore = nextLayer[j].Score;
                        bestIndex = j;
                    }
                }

                if (!rootMove.Origin.HasValue && !rootMove.Destination.HasValue)
                {
                    _openScore = nextLayer[bestIndex].Score;
                }

                finalMoves.Add(new MiniThinkMove(rootMove.Origin, rootMove.Destination, nextLayer[bestIndex].Score));
                if (nextLayer[bestIndex].Score < _aiMinScore)
                {
                    _aiMinScore = nextLayer[bestIndex].Score;
                }
            }

            if (finalMoves.Count > 0)
            {
                var minIndex = 0;
                var best = float.PositiveInfinity;
                for (var i = 0; i < finalMoves.Count; i++)
                {
                    if (finalMoves[i].Score < best)
                    {
                        best = finalMoves[i].Score;
                        minIndex = i;
                    }
                }

                return finalMoves[minIndex];
            }

            return new MiniThinkMove(origin, destination, minScore);
        }

        if (moves.Count == 1)
        {
            return moves[0];
        }

        return new MiniThinkMove(null, null, 0f);
    }

    private List<MiniTurnMove> MiniOneTurn(Piece[,] board, MiniThinkMove rootMove, int ownerColor, Vector2Int? nextOrigin, Vector2Int? nextDestination, float score, float div)
    {
        var responses = new List<MiniTurnMove>();
        var boardAfterMove = CloneBoard(board);

        if (nextOrigin.HasValue && nextDestination.HasValue)
        {
            ApplyMove(boardAfterMove, nextOrigin.Value, nextDestination.Value);
        }

        var playerColorInt = ColorToInt(_playerColor);
        var aiColorInt = ColorToInt(_aiColor);

        if (ownerColor == playerColorInt && MiniCantMove(boardAfterMove, playerColorInt) == 1)
        {
            responses.Add(new MiniTurnMove(rootMove.Origin, rootMove.Destination, null, null, score));
            return responses;
        }

        if (ownerColor == playerColorInt && score > _aiMinScore)
        {
            responses.Add(new MiniTurnMove(rootMove.Origin, rootMove.Destination, null, null, score));
            return responses;
        }

        if (HiddenCount(boardAfterMove) > 0)
        {
            responses.Add(new MiniTurnMove(rootMove.Origin, rootMove.Destination, null, null, score));
        }

        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var piece = boardAfterMove[row, col];
                if (piece == null || !piece.Revealed || ColorToInt(piece.Color) != ownerColor)
                {
                    continue;
                }

                var from = new Vector2Int(row, col);
                var moves = CollectLegalMoves(boardAfterMove, from);
                foreach (var move in moves)
                {
                    float moveScore;
                    if (MiniWillDeadPity(from, move, boardAfterMove, ownerColor) == 0)
                    {
                        if (ownerColor == playerColorInt)
                        {
                            if (MiniWillDeadPityEvenEqual(from, move, boardAfterMove, ownerColor) == 0)
                            {
                                moveScore = score + div * MiniMoveScore(from, move, boardAfterMove, playerColorInt);
                            }
                            else
                            {
                                moveScore = score;
                            }
                        }
                        else
                        {
                            moveScore = score - div * MiniMoveScore(from, move, boardAfterMove, aiColorInt);
                        }
                    }
                    else
                    {
                        if (ownerColor == aiColorInt)
                        {
                            moveScore = score + 40f - div * MiniMoveScore(from, move, boardAfterMove, aiColorInt);
                        }
                        else
                        {
                            moveScore = score - 8f;
                        }
                    }

                    responses.Add(new MiniTurnMove(rootMove.Origin, rootMove.Destination, from, move, moveScore));
                }
            }
        }

        return responses;
    }

    private int MiniCantMove(Piece[,] board, int ownerColor)
    {
        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var piece = board[row, col];
                if (piece == null || !piece.Revealed || ColorToInt(piece.Color) != ownerColor)
                {
                    continue;
                }

                if (CollectLegalMoves(board, new Vector2Int(row, col)).Count > 0)
                {
                    return 0;
                }
            }
        }

        return 1;
    }

    private void MiniScanKing(Piece[,] board)
    {
        _kingLive[0] = 0;
        _kingLive[1] = 0;

        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var piece = board[row, col];
                if (piece == null || piece.Rank != 7)
                {
                    continue;
                }

                _kingLive[ColorToInt(piece.Color)] = 1;
            }
        }
    }

    private static bool IsMoveSentinel(Vector2Int cell)
    {
        return cell.x == -1 && cell.y == -1;
    }

    private Vector2Int? MiniSelectBackChess(Piece[,] board, Vector2Int? origin = null)
    {
        var bombThreat = MiniScanPlayerBomb(board);
        if (bombThreat.HasValue)
        {
            return bombThreat.Value;
        }

        var bombChance = MiniScanComBomb(board);
        if (bombChance.HasValue)
        {
            return bombChance.Value;
        }

        int y0;
        int y1;
        int yStep;
        if (_random.Next(0, 2) == 0)
        {
            y0 = 0;
            y1 = 4;
            yStep = 1;
        }
        else
        {
            y0 = 3;
            y1 = -1;
            yStep = -1;
        }

        int x0;
        int x1;
        int xStep;
        if (_random.Next(0, 2) == 0)
        {
            x0 = 0;
            x1 = 8;
            xStep = 1;
        }
        else
        {
            x0 = 7;
            x1 = -1;
            xStep = -1;
        }

        var preferred = MiniCalcGoodBackChess(y0, y1, yStep, x0, x1, xStep, board, 0);
        if (preferred.HasValue)
        {
            return preferred.Value;
        }

        if (origin.HasValue)
        {
            return new Vector2Int(-1, -1);
        }

        if (MiniCheckBackExist(board) == 1)
        {
            return MiniCalcGoodBackChess(y0, y1, yStep, x0, x1, xStep, board);
        }

        return null;
    }

    private int MiniCheckBackExist(Piece[,] board)
    {
        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var piece = board[row, col];
                if (piece != null && !piece.Revealed)
                {
                    return 1;
                }
            }
        }

        return 0;
    }

    private Vector2Int? MiniCalcGoodBackChess(int y0, int y1, int yStep, int x0, int x1, int xStep, Piece[,] board, int maxEatNumber = -33)
    {
        Vector2Int? result = null;
        var hiddenCount = HiddenCount(board);
        var playerColorInt = ColorToInt(_playerColor);
        var aiColorInt = ColorToInt(_aiColor);

        for (var y = y0; y != y1; y += yStep)
        {
            for (var x = x0; x != x1; x += xStep)
            {
                var piece = board[y, x];
                if (piece == null || piece.Revealed)
                {
                    continue;
                }

                if (MiniEatByPlayerBomb(new Vector2Int(y, x), board, playerColorInt) == 0)
                {
                    var nearMin = 8;
                    var nearMax = 0;
                    var nearOurMin = 8;
                    var nearOurMax = 0;
                    var nearCells = MiniNear(y, x);
                    foreach (var near in nearCells)
                    {
                        var nearPiece = board[near.x, near.y];
                        if (nearPiece == null || !nearPiece.Revealed)
                        {
                            continue;
                        }

                        var value = nearPiece.Rank;
                        var colorInt = ColorToInt(nearPiece.Color);
                        if (colorInt == playerColorInt)
                        {
                            if (value < nearMin)
                            {
                                nearMin = value;
                            }

                            if (value > nearMax)
                            {
                                nearMax = value;
                            }
                        }
                        else if (colorInt == aiColorInt)
                        {
                            if (value < nearOurMin)
                            {
                                nearOurMin = value;
                            }

                            if (value > nearOurMax)
                            {
                                nearOurMax = value;
                            }
                        }
                    }

                    var eatDiff = MiniCheckEatNumber(board, nearMin, nearMax, nearOurMin, nearOurMax, y, x);
                    if (eatDiff > maxEatNumber)
                    {
                        maxEatNumber = eatDiff;
                        result = new Vector2Int(y, x);
                    }
                }
                else if (maxEatNumber == -33)
                {
                    maxEatNumber = -1 * hiddenCount;
                    result = new Vector2Int(y, x);
                }
            }
        }

        return result;
    }

    private int MiniCheckEatNumber(Piece[,] board, int nMin, int nMax, int noMin, int noMax, int y, int x)
    {
        var aiColorInt = ColorToInt(_aiColor);
        var playerColorInt = ColorToInt(_playerColor);
        var eatPossibleNum = 0;
        var wasAteNum = 0;

        for (var color = 0; color <= 1; color++)
        {
            for (var value = 0; value <= 7; value++)
            {
                var num = _backValueNum[color, value];
                if (num == 0 || value == 0)
                {
                    continue;
                }

                if (aiColorInt == color)
                {
                    if (value == 2 && nMax < 3)
                    {
                        if (MiniIfCannonCanEat(new Vector2Int(y, x), board, aiColorInt) > 0)
                        {
                            eatPossibleNum += num;
                        }
                    }
                    else if (nMax == 0)
                    {
                        continue;
                    }
                    else if (nMax == 1 && value == 7)
                    {
                        wasAteNum += num;
                    }
                    else if (nMax == 7 && value == 1)
                    {
                        if (nMin == 7 || nMin == 2)
                        {
                            eatPossibleNum += num;
                        }
                        else
                        {
                            wasAteNum += num;
                        }
                    }
                    else if (value > nMax)
                    {
                        eatPossibleNum += num;
                    }
                    else if (value != 1 || nMax != 2 || nMin != 2)
                    {
                        wasAteNum += num;
                    }
                }
                else if (value == 2)
                {
                    if (MiniIfCannonCanEat(new Vector2Int(y, x), board, playerColorInt) > 0)
                    {
                        wasAteNum += num;
                    }
                    else if (noMax >= 3)
                    {
                        eatPossibleNum += num;
                    }
                }
                else if (noMin == 8)
                {
                    continue;
                }
                else if (noMin == 1 && value == 7)
                {
                    if (noMax == 1)
                    {
                        eatPossibleNum += num;
                    }
                    else
                    {
                        wasAteNum += num;
                    }
                }
                else if (noMax == 7 && value == 1)
                {
                    wasAteNum += num;
                }
                else if (value >= noMin)
                {
                    wasAteNum += num;
                }
                else if (value != 1 || nMax != 2 || nMin != 2)
                {
                    eatPossibleNum += num;
                }
            }
        }

        return eatPossibleNum - wasAteNum;
    }

    private int MiniIfCannonCanEat(Vector2Int origin, Piece[,] board, int ownerColor)
    {
        var row = origin.x;
        var col = origin.y;
        var jump = false;
        var eatNumber = 0;
        var opponentColor = OpponentColor(ownerColor);

        for (var r = row - 1; r >= 0; r--)
        {
            var piece = board[r, col];
            if (jump && piece != null)
            {
                if (!piece.Revealed)
                {
                    break;
                }

                if (ColorToInt(piece.Color) == opponentColor)
                {
                    eatNumber++;
                    break;
                }
            }
            else if (piece != null)
            {
                jump = true;
            }
        }

        jump = false;
        for (var r = row + 1; r < Rows; r++)
        {
            var piece = board[r, col];
            if (jump && piece != null)
            {
                if (!piece.Revealed)
                {
                    break;
                }

                if (ColorToInt(piece.Color) == opponentColor)
                {
                    eatNumber++;
                    break;
                }
            }
            else if (piece != null)
            {
                jump = true;
            }
        }

        jump = false;
        for (var c = col - 1; c >= 0; c--)
        {
            var piece = board[row, c];
            if (jump && piece != null)
            {
                if (!piece.Revealed)
                {
                    break;
                }

                if (ColorToInt(piece.Color) == opponentColor)
                {
                    eatNumber++;
                    break;
                }
            }
            else if (piece != null)
            {
                jump = true;
            }
        }

        jump = false;
        for (var c = col + 1; c < Cols; c++)
        {
            var piece = board[row, c];
            if (jump && piece != null)
            {
                if (!piece.Revealed)
                {
                    break;
                }

                if (ColorToInt(piece.Color) == opponentColor)
                {
                    eatNumber++;
                    break;
                }
            }
            else if (piece != null)
            {
                jump = true;
            }
        }

        return eatNumber;
    }

    private int MiniEatByPlayerBomb(Vector2Int origin, Piece[,] board, int playerColor)
    {
        _cannonCor.Clear();
        var row = origin.x;
        var col = origin.y;
        var jump = false;
        var wasAte = 0;

        for (var r = row - 1; r >= 0; r--)
        {
            var piece = board[r, col];
            if (jump && piece != null)
            {
                if (!piece.Revealed)
                {
                    break;
                }

                if (piece.Rank == 2 && ColorToInt(piece.Color) == playerColor)
                {
                    _cannonCor.Add(new Vector2Int(r, col));
                    wasAte = 1;
                    break;
                }
            }
            else if (piece != null)
            {
                jump = true;
            }
        }

        jump = false;
        for (var r = row + 1; r < Rows; r++)
        {
            var piece = board[r, col];
            if (jump && piece != null)
            {
                if (!piece.Revealed)
                {
                    break;
                }

                if (piece.Rank == 2 && ColorToInt(piece.Color) == playerColor)
                {
                    _cannonCor.Add(new Vector2Int(r, col));
                    wasAte = 1;
                    break;
                }
            }
            else if (piece != null)
            {
                jump = true;
            }
        }

        jump = false;
        for (var c = col - 1; c >= 0; c--)
        {
            var piece = board[row, c];
            if (jump && piece != null)
            {
                if (!piece.Revealed)
                {
                    break;
                }

                if (piece.Rank == 2 && ColorToInt(piece.Color) == playerColor)
                {
                    _cannonCor.Add(new Vector2Int(row, c));
                    wasAte = 1;
                    break;
                }
            }
            else if (piece != null)
            {
                jump = true;
            }
        }

        jump = false;
        for (var c = col + 1; c < Cols; c++)
        {
            var piece = board[row, c];
            if (jump && piece != null)
            {
                if (!piece.Revealed)
                {
                    break;
                }

                if (piece.Rank == 2 && ColorToInt(piece.Color) == playerColor)
                {
                    _cannonCor.Add(new Vector2Int(row, c));
                    wasAte = 1;
                    break;
                }
            }
            else if (piece != null)
            {
                jump = true;
            }
        }

        return wasAte;
    }

    private List<Vector2Int> MiniNear(int row, int col)
    {
        var neighbors = new List<Vector2Int>(4);

        if (row == 0 && col == 0)
        {
            neighbors.Add(new Vector2Int(1, 0));
            neighbors.Add(new Vector2Int(0, 1));
        }
        else if (row == 3 && col == 0)
        {
            neighbors.Add(new Vector2Int(2, 0));
            neighbors.Add(new Vector2Int(3, 1));
        }
        else if (row == 0 && col == 7)
        {
            neighbors.Add(new Vector2Int(0, 6));
            neighbors.Add(new Vector2Int(1, 7));
        }
        else if (row == 3 && col == 7)
        {
            neighbors.Add(new Vector2Int(3, 6));
            neighbors.Add(new Vector2Int(2, 7));
        }
        else if (col == 0)
        {
            neighbors.Add(new Vector2Int(row - 1, col));
            neighbors.Add(new Vector2Int(row + 1, col));
            neighbors.Add(new Vector2Int(row, col + 1));
        }
        else if (row == 0)
        {
            neighbors.Add(new Vector2Int(row, col - 1));
            neighbors.Add(new Vector2Int(row, col + 1));
            neighbors.Add(new Vector2Int(row + 1, col));
        }
        else if (col == 7)
        {
            neighbors.Add(new Vector2Int(row - 1, col));
            neighbors.Add(new Vector2Int(row + 1, col));
            neighbors.Add(new Vector2Int(row, col - 1));
        }
        else if (row == 3)
        {
            neighbors.Add(new Vector2Int(row, col - 1));
            neighbors.Add(new Vector2Int(row, col + 1));
            neighbors.Add(new Vector2Int(row - 1, col));
        }
        else
        {
            neighbors.Add(new Vector2Int(row - 1, col));
            neighbors.Add(new Vector2Int(row + 1, col));
            neighbors.Add(new Vector2Int(row, col + 1));
            neighbors.Add(new Vector2Int(row, col - 1));
        }

        return neighbors;
    }

    private int MiniNearMaxValue(Vector2Int? openCell, Vector2Int? origin, Piece[,] board)
    {
        if (!openCell.HasValue)
        {
            return 0;
        }

        var max = 0;
        var nearCells = MiniNear(openCell.Value.x, openCell.Value.y);
        for (var i = 0; i < nearCells.Count; i++)
        {
            var near = nearCells[i];
            if (origin.HasValue && near == origin.Value)
            {
                continue;
            }

            var piece = board[near.x, near.y];
            if (piece == null || !piece.Revealed)
            {
                continue;
            }

            if (piece.Color == _aiColor)
            {
                continue;
            }

            if (piece.Rank > max)
            {
                max = piece.Rank;
            }
        }

        return max;
    }

    private int MiniNearMaxValueNotConsiderAiColor(Vector2Int? openCell, Vector2Int? origin, Piece[,] board)
    {
        if (!openCell.HasValue)
        {
            return 0;
        }

        var max = 0;
        var nearCells = MiniNear(openCell.Value.x, openCell.Value.y);
        for (var i = 0; i < nearCells.Count; i++)
        {
            var near = nearCells[i];
            if (origin.HasValue && near == origin.Value)
            {
                continue;
            }

            var piece = board[near.x, near.y];
            if (piece == null || !piece.Revealed)
            {
                continue;
            }

            if (piece.Rank > max)
            {
                max = piece.Rank;
            }
        }

        return max;
    }

    private Vector2Int? MiniScanPlayerBomb(Piece[,] board)
    {
        var playerColorInt = ColorToInt(_playerColor);

        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var piece = board[row, col];
                if (piece == null || !piece.Revealed || piece.Rank != 2 || ColorToInt(piece.Color) != playerColorInt)
                {
                    continue;
                }

                var nearCells = MiniNear(row, col);
                var offset = _random.Next(0, nearCells.Count);
                for (var i = 0; i < nearCells.Count; i++)
                {
                    var idx = (offset + i) % nearCells.Count;
                    var near = nearCells[idx];
                    var target = board[near.x, near.y];
                    if (target == null || target.Revealed)
                    {
                        continue;
                    }

                    if (MiniNearMaxValue(near, new Vector2Int(row, col), board) <= 3 &&
                        MiniEatByPlayerBomb(near, board, playerColorInt) == 0)
                    {
                        return near;
                    }
                }
            }
        }

        return null;
    }

    private Vector2Int? MiniBombMayEat(Vector2Int? origin, Piece[,] board)
    {
        if (!origin.HasValue)
        {
            return null;
        }

        var startDirection = _random.Next(0, 4);

        for (var d = 0; d < 4; d++)
        {
            var direction = (startDirection + d) % 4;
            var row = origin.Value.x;
            var col = origin.Value.y;
            var jump = false;

            if (direction == 0)
            {
                for (var r = row - 1; r >= 0; r--)
                {
                    var piece = board[r, col];
                    if (jump && piece != null)
                    {
                        if (!piece.Revealed && MiniNearMaxValueNotConsiderAiColor(new Vector2Int(r, col), null, board) < 2)
                        {
                            return new Vector2Int(r, col);
                        }

                        break;
                    }

                    if (piece != null)
                    {
                        jump = !piece.Revealed;
                        if (piece.Revealed)
                        {
                            break;
                        }
                    }
                }
            }
            else if (direction == 1)
            {
                for (var r = row + 1; r < Rows; r++)
                {
                    var piece = board[r, col];
                    if (jump && piece != null)
                    {
                        if (!piece.Revealed && MiniNearMaxValueNotConsiderAiColor(new Vector2Int(r, col), null, board) < 2)
                        {
                            return new Vector2Int(r, col);
                        }

                        break;
                    }

                    if (piece != null)
                    {
                        jump = !piece.Revealed;
                        if (piece.Revealed)
                        {
                            break;
                        }
                    }
                }
            }
            else if (direction == 2)
            {
                for (var c = col - 1; c >= 0; c--)
                {
                    var piece = board[row, c];
                    if (jump && piece != null)
                    {
                        if (!piece.Revealed && MiniNearMaxValueNotConsiderAiColor(new Vector2Int(row, c), null, board) < 2)
                        {
                            return new Vector2Int(row, c);
                        }

                        break;
                    }

                    if (piece != null)
                    {
                        jump = !piece.Revealed;
                        if (piece.Revealed)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                for (var c = col + 1; c < Cols; c++)
                {
                    var piece = board[row, c];
                    if (jump && piece != null)
                    {
                        if (!piece.Revealed && MiniNearMaxValueNotConsiderAiColor(new Vector2Int(row, c), null, board) < 2)
                        {
                            return new Vector2Int(row, c);
                        }

                        break;
                    }

                    if (piece != null)
                    {
                        jump = !piece.Revealed;
                        if (piece.Revealed)
                        {
                            break;
                        }
                    }
                }
            }
        }

        return null;
    }

    private Vector2Int? MiniScanComBomb(Piece[,] board)
    {
        Vector2Int? result = null;
        var aiColorInt = ColorToInt(_aiColor);

        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var piece = board[row, col];
                if (piece == null || !piece.Revealed || piece.Rank != 2 || ColorToInt(piece.Color) != aiColorInt)
                {
                    continue;
                }

                var candidate = MiniBombMayEat(new Vector2Int(row, col), board);
                if (candidate.HasValue)
                {
                    result = candidate;
                }
            }
        }

        return result;
    }

    private int MiniCanBeAteEqual(int smallValue, int bigValue)
    {
        if (bigValue == 2)
        {
            return 0;
        }

        if (bigValue == 1 && smallValue == 7)
        {
            return 1;
        }

        if (bigValue == 7 && smallValue == 1)
        {
            return 0;
        }

        if (bigValue > smallValue)
        {
            return 1;
        }

        if (bigValue == smallValue)
        {
            return 2;
        }

        return 0;
    }

    private int MiniCanBeAte(int smallValue, int bigValue)
    {
        if (bigValue == 2)
        {
            return 0;
        }

        if (bigValue == 1 && smallValue == 7)
        {
            return 1;
        }

        if (bigValue == 7 && smallValue == 1)
        {
            return 0;
        }

        return bigValue > smallValue ? 1 : 0;
    }

    private int MiniOppCannonCanEat(Vector2Int origin, Vector2Int destination, Piece[,] board)
    {
        var moving = board[origin.x, origin.y];
        if (moving == null)
        {
            return 0;
        }

        var ownerColor = moving.Color;
        Piece ch1 = null;
        Piece ch2 = null;

        if (origin.x == destination.x)
        {
            for (var row = destination.x + 1; row < Rows; row++)
            {
                if (board[row, destination.y] != null)
                {
                    ch1 = board[row, destination.y];
                    break;
                }
            }

            for (var row = destination.x - 1; row >= 0; row--)
            {
                if (board[row, destination.y] != null)
                {
                    ch2 = board[row, destination.y];
                    break;
                }
            }
        }
        else
        {
            for (var col = destination.y + 1; col < Cols; col++)
            {
                if (board[destination.x, col] != null)
                {
                    ch1 = board[destination.x, col];
                    break;
                }
            }

            for (var col = destination.y - 1; col >= 0; col--)
            {
                if (board[destination.x, col] != null)
                {
                    ch2 = board[destination.x, col];
                    break;
                }
            }
        }

        if (ch1 == null || ch2 == null || !ch1.Revealed || !ch2.Revealed)
        {
            return 0;
        }

        if ((ch1.Color == ownerColor && ch2.Color != ownerColor && ch2.Rank == 2) ||
            (ch1.Color != ownerColor && ch2.Color == ownerColor && ch1.Rank == 2))
        {
            return 1;
        }

        return 0;
    }

    private int MiniEatByBomb(Vector2Int origin, Piece[,] board)
    {
        _cannonCor.Clear();
        var moving = board[origin.x, origin.y];
        if (moving == null)
        {
            return 0;
        }

        var wasAte = 0;
        var jump = false;

        for (var row = origin.x - 1; row >= 0; row--)
        {
            var piece = board[row, origin.y];
            if (jump && piece != null)
            {
                if (!piece.Revealed || piece.Color == moving.Color)
                {
                    break;
                }

                if (piece.Rank == 2)
                {
                    _cannonCor.Add(new Vector2Int(row, origin.y));
                    wasAte = 1;
                    break;
                }
            }
            else if (piece != null)
            {
                jump = true;
            }
        }

        jump = false;
        for (var row = origin.x + 1; row < Rows; row++)
        {
            var piece = board[row, origin.y];
            if (jump && piece != null)
            {
                if (!piece.Revealed || piece.Color == moving.Color)
                {
                    break;
                }

                if (piece.Rank == 2)
                {
                    _cannonCor.Add(new Vector2Int(row, origin.y));
                    wasAte = 1;
                    break;
                }
            }
            else if (piece != null)
            {
                jump = true;
            }
        }

        jump = false;
        for (var col = origin.y - 1; col >= 0; col--)
        {
            var piece = board[origin.x, col];
            if (jump && piece != null)
            {
                if (!piece.Revealed || piece.Color == moving.Color)
                {
                    break;
                }

                if (piece.Rank == 2)
                {
                    _cannonCor.Add(new Vector2Int(origin.x, col));
                    wasAte = 1;
                    break;
                }
            }
            else if (piece != null)
            {
                jump = true;
            }
        }

        jump = false;
        for (var col = origin.y + 1; col < Cols; col++)
        {
            var piece = board[origin.x, col];
            if (jump && piece != null)
            {
                if (!piece.Revealed || piece.Color == moving.Color)
                {
                    break;
                }

                if (piece.Rank == 2)
                {
                    _cannonCor.Add(new Vector2Int(origin.x, col));
                    wasAte = 1;
                    break;
                }
            }
            else if (piece != null)
            {
                jump = true;
            }
        }

        return wasAte;
    }

    private int MiniNextCannonCanEatMore(Vector2Int origin, Vector2Int destination, Piece[,] board)
    {
        var moving = board[origin.x, origin.y];
        if (moving == null)
        {
            return 0;
        }

        if (MiniEatByBomb(origin, board) == 0)
        {
            return 0;
        }

        var after = CloneBoard(board);
        ApplyMove(after, origin, destination);

        for (var i = 0; i < _cannonCor.Count; i++)
        {
            var cannonCell = _cannonCor[i];
            var cannon = after[cannonCell.x, cannonCell.y];
            if (cannon == null)
            {
                continue;
            }

            var cannonMoves = CollectLegalMoves(after, cannonCell);
            for (var m = 0; m < cannonMoves.Count; m++)
            {
                var targetCell = cannonMoves[m];
                var target = after[targetCell.x, targetCell.y];
                if (target == null)
                {
                    continue;
                }

                if (targetCell.x == origin.x || targetCell.y == origin.y)
                {
                    var capturedScore = MiniEatingValueToScore(target.Rank, _kingLive, ColorToInt(target.Color));
                    var selfScore = MiniEatingValueToScore(moving.Rank, _kingLive, ColorToInt(moving.Color));
                    if (capturedScore > selfScore)
                    {
                        return 1;
                    }
                }
            }
        }

        return 0;
    }

    private int MiniShortDist(int row, int col, int dist, Piece[,] board)
    {
        var shortest = 0;
        var nearCells = MiniNear(row, col);
        for (var i = 0; i < nearCells.Count; i++)
        {
            var near = nearCells[i];
            var marked = _evalMark[near.x, near.y];
            if (marked != 0 && marked < dist)
            {
                if (shortest == 0)
                {
                    shortest = marked + 1;
                }
                else if (marked + 1 < shortest)
                {
                    shortest = marked + 1;
                }
            }
        }

        if (shortest == 0 && board[row, col] == null)
        {
            shortest = dist;
        }

        return shortest;
    }

    private float MiniCalcMoveScore(float maxValue, int maxDist, int myValue)
    {
        var valueAdjust = myValue / 11f;

        if (Mathf.Abs(maxValue - 9f) < 0.0001f)
        {
            if (maxDist != 0)
            {
                if (3.5f > 0.2f * maxDist)
                {
                    return 4.2f - 0.2f * maxDist;
                }

                return 0.7f - maxDist / 1000f;
            }

            return 0f;
        }

        if (Mathf.Abs(maxValue) > 0.0001f)
        {
            if (maxValue / 2f > 0.2f * maxDist)
            {
                return maxValue / 2f - 0.2f * maxDist + valueAdjust;
            }

            return valueAdjust - maxDist / 1000f;
        }

        return -0.1f;
    }

    private void MiniMoveMaxValue(int originCol, int originRow, int destCol, int destRow, Piece[,] board, int originValue, int ownerColor, int row, int col, int dist = 1)
    {
        if (row < 0 || col < 0 || row >= Rows || col >= Cols)
        {
            return;
        }

        if (row == originRow && col == originCol)
        {
            return;
        }

        if (_evalMark[row, col] > 0 || _evalCannonMark[row, col] > 0)
        {
            return;
        }

        var nearCells = MiniNear(row, col);
        for (var i = 0; i < nearCells.Count; i++)
        {
            var near = nearCells[i];
            var piece = board[near.x, near.y];
            if (piece == null || !piece.Revealed)
            {
                continue;
            }

            if (ColorToInt(piece.Color) != ownerColor &&
                MiniCanBeAte(piece.Rank, board[originRow, originCol].Rank) == 1)
            {
                return;
            }
        }

        var current = board[row, col];
        if (current != null && !current.Revealed)
        {
            return;
        }

        var opponentColor = OpponentColor(ownerColor);
        var currentDist = 32;

        if (current != null)
        {
            currentDist = MiniShortDist(row, col, dist, board);
        }
        else
        {
            _evalMark[row, col] = MiniShortDist(row, col, dist, board);
        }

        if (current != null)
        {
            if (ColorToInt(current.Color) == opponentColor)
            {
                if (originValue == 7)
                {
                    if (current.Rank == 1)
                    {
                        return;
                    }

                    if (current.Rank == 2 && _evalMaxValue <= 5.5f)
                    {
                        if (_evalMaxValue < 5.5f)
                        {
                            _evalMaxValue = 5.5f;
                            _evalMaxDist = currentDist;
                        }
                        else if (currentDist < _evalMaxDist)
                        {
                            _evalMaxDist = currentDist;
                        }

                        return;
                    }

                    if (_evalMaxValue <= current.Rank)
                    {
                        if (_evalMaxValue < current.Rank)
                        {
                            _evalMaxValue = current.Rank;
                            _evalMaxDist = currentDist;
                        }
                        else if (currentDist < _evalMaxDist)
                        {
                            _evalMaxDist = currentDist;
                        }

                        return;
                    }
                }
                else if (originValue == 1)
                {
                    if (current.Rank == 7)
                    {
                        if (Mathf.Abs(_evalMaxValue - 9f) > 0.0001f)
                        {
                            _evalMaxValue = 9f;
                            _evalMaxDist = currentDist;
                        }
                        else if (currentDist < _evalMaxDist)
                        {
                            _evalMaxDist = currentDist;
                        }

                        return;
                    }

                    if (current.Rank == 1)
                    {
                        if (Mathf.Abs(_evalMaxValue - 1f) > 0.0001f)
                        {
                            _evalMaxValue = 1f;
                            _evalMaxDist = currentDist;
                        }
                        else if (currentDist < _evalMaxDist)
                        {
                            _evalMaxDist = currentDist;
                        }

                        return;
                    }
                }
                else if (current.Rank == 2 && originValue > 2 && _evalMaxValue <= 5.5f)
                {
                    if (_evalMaxValue < 5.5f)
                    {
                        _evalMaxValue = 5.5f;
                        _evalMaxDist = currentDist;
                    }
                    else if (currentDist < _evalMaxDist)
                    {
                        _evalMaxDist = currentDist;
                    }

                    return;
                }
                else if (_evalMaxValue <= current.Rank && current.Rank <= originValue)
                {
                    if (_evalMaxValue < current.Rank)
                    {
                        _evalMaxValue = current.Rank;
                        _evalMaxDist = currentDist;
                    }
                    else if (currentDist < _evalMaxDist)
                    {
                        _evalMaxDist = currentDist;
                    }

                    return;
                }
            }
        }
        else if (originRow == destRow && originCol + 1 == destCol)
        {
            MiniMoveMaxValue(originCol, originRow, destCol, destRow, board, originValue, ownerColor, row, col + 1, dist + 1);
            MiniMoveMaxValue(originCol, originRow, destCol, destRow, board, originValue, ownerColor, row, col - 1, dist + 1);
            MiniMoveMaxValue(originCol, originRow, destCol, destRow, board, originValue, ownerColor, row + 1, col, dist + 1);
            MiniMoveMaxValue(originCol, originRow, destCol, destRow, board, originValue, ownerColor, row - 1, col, dist + 1);
        }
        else if (originRow == destRow && originCol - 1 == destCol)
        {
            MiniMoveMaxValue(originCol, originRow, destCol, destRow, board, originValue, ownerColor, row, col - 1, dist + 1);
            MiniMoveMaxValue(originCol, originRow, destCol, destRow, board, originValue, ownerColor, row, col + 1, dist + 1);
            MiniMoveMaxValue(originCol, originRow, destCol, destRow, board, originValue, ownerColor, row + 1, col, dist + 1);
            MiniMoveMaxValue(originCol, originRow, destCol, destRow, board, originValue, ownerColor, row - 1, col, dist + 1);
        }
        else if (originRow + 1 == destRow && originCol == destCol)
        {
            MiniMoveMaxValue(originCol, originRow, destCol, destRow, board, originValue, ownerColor, row + 1, col, dist + 1);
            MiniMoveMaxValue(originCol, originRow, destCol, destRow, board, originValue, ownerColor, row - 1, col, dist + 1);
            MiniMoveMaxValue(originCol, originRow, destCol, destRow, board, originValue, ownerColor, row, col + 1, dist + 1);
            MiniMoveMaxValue(originCol, originRow, destCol, destRow, board, originValue, ownerColor, row, col - 1, dist + 1);
        }
        else if (originRow - 1 == destRow && originCol == destCol)
        {
            MiniMoveMaxValue(originCol, originRow, destCol, destRow, board, originValue, ownerColor, row - 1, col, dist + 1);
            MiniMoveMaxValue(originCol, originRow, destCol, destRow, board, originValue, ownerColor, row + 1, col, dist + 1);
            MiniMoveMaxValue(originCol, originRow, destCol, destRow, board, originValue, ownerColor, row, col + 1, dist + 1);
            MiniMoveMaxValue(originCol, originRow, destCol, destRow, board, originValue, ownerColor, row, col - 1, dist + 1);
        }
    }

    private int MiniCaca(Vector2Int? origin, Vector2Int destination, Piece[,] board, int ownerColor)
    {
        if (!origin.HasValue || ownerColor == ColorToInt(_playerColor))
        {
            return 0;
        }

        var moving = board[origin.Value.x, origin.Value.y];
        if (moving == null || moving.Rank == 2)
        {
            return 0;
        }

        var checks = new[]
        {
            new Vector2Int(destination.x - 1, destination.y - 1),
            new Vector2Int(destination.x - 1, destination.y + 1),
            new Vector2Int(destination.x + 1, destination.y - 1),
            new Vector2Int(destination.x + 1, destination.y + 1)
        };

        for (var i = 0; i < checks.Length; i++)
        {
            var cell = checks[i];
            if (!InBounds(cell.x, cell.y))
            {
                continue;
            }

            var piece = board[cell.x, cell.y];
            if (piece == null || !piece.Revealed || piece.Color == moving.Color)
            {
                continue;
            }

            if (moving.Rank == 7 && piece.Rank == 1)
            {
                continue;
            }

            var eatValue = MiniCanBeAteEqual(piece.Rank, moving.Rank);
            if (eatValue == 1 || eatValue == 2)
            {
                return eatValue;
            }
        }

        return 0;
    }

    private int MiniNear2HaveSameValue(Vector2Int? origin, Piece[,] board, int ownerColor)
    {
        if (!origin.HasValue || ownerColor == ColorToInt(_playerColor))
        {
            return 0;
        }

        var moving = board[origin.Value.x, origin.Value.y];
        if (moving == null || moving.Rank == 2)
        {
            return 0;
        }

        var candidates = new[]
        {
            new Vector2Int(origin.Value.x - 2, origin.Value.y),
            new Vector2Int(origin.Value.x + 2, origin.Value.y),
            new Vector2Int(origin.Value.x, origin.Value.y - 2),
            new Vector2Int(origin.Value.x, origin.Value.y + 2),
            new Vector2Int(origin.Value.x - 1, origin.Value.y - 1),
            new Vector2Int(origin.Value.x - 1, origin.Value.y + 1),
            new Vector2Int(origin.Value.x + 1, origin.Value.y - 1),
            new Vector2Int(origin.Value.x + 1, origin.Value.y + 1)
        };

        for (var i = 0; i < candidates.Length; i++)
        {
            var cell = candidates[i];
            if (!InBounds(cell.x, cell.y))
            {
                continue;
            }

            var piece = board[cell.x, cell.y];
            if (piece == null || !piece.Revealed || piece.Color == moving.Color)
            {
                continue;
            }

            if (moving.Rank == 7 && piece.Rank == 1)
            {
                continue;
            }

            if (piece.Rank == moving.Rank)
            {
                return 1;
            }
        }

        return 0;
    }

    private void MiniCalcCannonMark(Piece[,] board, int ownerColor)
    {
        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                _evalCannonMark[row, col] = 0;
            }
        }

        if (ownerColor == ColorToInt(_playerColor))
        {
            return;
        }

        var findPlayerCannons = 0;
        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                if (findPlayerCannons == 2)
                {
                    break;
                }

                var piece = board[row, col];
                if (piece == null || !piece.Revealed || piece.Color != _playerColor || piece.Rank != 2)
                {
                    continue;
                }

                findPlayerCannons++;
                var jump = false;

                for (var r = row - 1; r >= 0; r--)
                {
                    if (jump && board[r, col] != null)
                    {
                        break;
                    }

                    if (jump && board[r, col] == null)
                    {
                        _evalCannonMark[r, col] = 1;
                    }
                    else if (board[r, col] != null)
                    {
                        jump = true;
                    }
                }

                jump = false;
                for (var r = row + 1; r < Rows; r++)
                {
                    if (jump && board[r, col] != null)
                    {
                        break;
                    }

                    if (jump && board[r, col] == null)
                    {
                        _evalCannonMark[r, col] = 1;
                    }
                    else if (board[r, col] != null)
                    {
                        jump = true;
                    }
                }

                jump = false;
                for (var c = col - 1; c >= 0; c--)
                {
                    if (jump && board[row, c] != null)
                    {
                        break;
                    }

                    if (jump && board[row, c] == null)
                    {
                        _evalCannonMark[row, c] = 1;
                    }
                    else if (board[row, c] != null)
                    {
                        jump = true;
                    }
                }

                jump = false;
                for (var c = col + 1; c < Cols; c++)
                {
                    if (jump && board[row, c] != null)
                    {
                        break;
                    }

                    if (jump && board[row, c] == null)
                    {
                        _evalCannonMark[row, c] = 1;
                    }
                    else if (board[row, c] != null)
                    {
                        jump = true;
                    }
                }
            }
        }
    }

    private float MiniMoveScore(Vector2Int origin, Vector2Int destination, Piece[,] board, int ownerColor)
    {
        var moving = board[origin.x, origin.y];
        if (moving == null)
        {
            return 0f;
        }

        if (board[destination.x, destination.y] == null)
        {
            for (var i = 0; i < _comBanStep.Count; i++)
            {
                if (_comBanStep[i] == origin)
                {
                    return -0.2f;
                }
            }

            if (MiniOwnerNextCanEatDeadP(origin, destination, board, ownerColor) == 1)
            {
                if (MiniOppCannonCanEat(origin, destination, board) == 1)
                {
                    return 7.5f;
                }

                if (moving.Rank == 3)
                {
                    return 7f;
                }

                return 10f;
            }

            if (MiniWillEat2More(origin, destination, board, ownerColor) == 1)
            {
                return 8f;
            }

            if (ownerColor == ColorToInt(_playerColor))
            {
                return 0f;
            }

            if (MiniDestWillDeadOwnerWontEat(origin, destination, _board, ColorToInt(_playerColor)) == 0 &&
                MiniStandWillDeadPity(origin, _board, ColorToInt(_aiColor)) == 1)
            {
                if (MiniNextCannonCanEatMore(origin, destination, board) == 1)
                {
                    return -8f;
                }

                return 9f;
            }

            if (moving.Rank == 2)
            {
                var after = CloneBoard(board);
                ApplyMove(after, origin, destination);
                var cannon = after[destination.x, destination.y];
                if (cannon != null)
                {
                    var cannonMoves = CollectLegalMoves(after, destination);
                    for (var i = 0; i < cannonMoves.Count; i++)
                    {
                        var target = after[cannonMoves[i].x, cannonMoves[i].y];
                        if (target == null)
                        {
                            continue;
                        }

                        if (target.Rank > 5)
                        {
                            return 7.3f;
                        }
                    }
                }

                return 0f;
            }

            _evalMaxValue = 0f;
            _evalMaxDist = 32;
            for (var row = 0; row < Rows; row++)
            {
                for (var col = 0; col < Cols; col++)
                {
                    _evalMark[row, col] = 0;
                }
            }

            MiniCalcCannonMark(board, ownerColor);
            MiniMoveMaxValue(origin.y, origin.x, destination.y, destination.x, board, moving.Rank, ownerColor, destination.x, destination.y);

            var cacaValue = MiniCaca(origin, destination, board, ownerColor);
            if (cacaValue == 1)
            {
                return MiniCalcMoveScore(_evalMaxValue, _evalMaxDist, moving.Rank) + 0.1f;
            }

            if (cacaValue == 2)
            {
                return MiniCalcMoveScore(_evalMaxValue, _evalMaxDist, moving.Rank) + 0.3f;
            }

            if (MiniNear2HaveSameValue(origin, board, ownerColor) == 1 &&
                MiniWillDeadPityEvenEqual(origin, destination, board, ownerColor) == 0)
            {
                return -0.1f;
            }

            var nearCells = MiniNear(origin.x, origin.y);
            for (var i = 0; i < nearCells.Count; i++)
            {
                var nearPiece = board[nearCells[i].x, nearCells[i].y];
                if (nearPiece == null || !nearPiece.Revealed)
                {
                    continue;
                }

                if (nearPiece.Color == _playerColor && MiniCanBeAte(nearPiece.Rank, moving.Rank) == 1)
                {
                    return -0.1f;
                }
            }

            return MiniCalcMoveScore(_evalMaxValue, _evalMaxDist, moving.Rank);
        }

        var targetPiece = board[destination.x, destination.y];
        if (targetPiece == null)
        {
            return 0f;
        }

        if (ownerColor == ColorToInt(_aiColor) &&
            MiniOwnerNextCanEatDeadP(origin, origin, board, ownerColor) == 1 &&
            MiniStandWillDeadPity(origin, board, ColorToInt(_aiColor)) == 0)
        {
            if (ContainsCell(_willEatEscapeChess, origin))
            {
                return MiniEatingValueToScore(targetPiece.Rank, _kingLive, ColorToInt(moving.Color));
            }

            if (!ContainsCell(_comWillEatChess, origin))
            {
                _comWillEatChess.Add(origin);
            }

            if (moving.Rank != 2)
            {
                return 10f + MiniEatingValueToScore(targetPiece.Rank, _kingLive, ColorToInt(moving.Color)) / 5f;
            }

            return MiniEatingValueToScore(targetPiece.Rank, _kingLive, ColorToInt(moving.Color));
        }

        if (ContainsCell(_comWillEatChess, origin))
        {
            if (moving.Rank != 2)
            {
                return 10f + MiniEatingValueToScore(targetPiece.Rank, _kingLive, ColorToInt(moving.Color)) / 5f;
            }

            return MiniEatingValueToScore(targetPiece.Rank, _kingLive, ColorToInt(moving.Color));
        }

        if (!ContainsCell(_willEatEscapeChess, origin))
        {
            _willEatEscapeChess.Add(origin);
        }

        return MiniEatingValueToScore(targetPiece.Rank, _kingLive, ColorToInt(moving.Color));
    }

    private int MiniDestWillDeadOwnerWontEat(Vector2Int origin, Vector2Int destination, Piece[,] board, int opponentColor)
    {
        var originPiece = board[origin.x, origin.y];
        if (originPiece == null || board[destination.x, destination.y] != null)
        {
            return 0;
        }

        var after = CloneBoard(board);
        ApplyMove(after, origin, destination);

        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var piece = after[row, col];
                if (piece == null || !piece.Revealed || ColorToInt(piece.Color) != opponentColor)
                {
                    continue;
                }

                var moves = CollectLegalMoves(after, new Vector2Int(row, col));
                if (ContainsCell(moves, destination))
                {
                    return 1;
                }
            }
        }

        return 0;
    }

    private int MiniWillDead(Vector2Int origin, Piece[,] board, int opponentColor)
    {
        var piece = board[origin.x, origin.y];
        if (piece == null)
        {
            return 0;
        }

        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var attacker = board[row, col];
                if (attacker == null || !attacker.Revealed || ColorToInt(attacker.Color) != opponentColor)
                {
                    continue;
                }

                if (ContainsCell(CollectLegalMoves(board, new Vector2Int(row, col)), origin))
                {
                    return 1;
                }
            }
        }

        return 0;
    }

    private int MiniWillEat2More(Vector2Int? nextOrigin, Vector2Int? nextDestination, Piece[,] board, int ownerColor)
    {
        var opponentColor = OpponentColor(ownerColor);
        var canEat = 0;
        var after = CloneBoard(board);

        if (nextOrigin.HasValue && nextDestination.HasValue)
        {
            ApplyMove(after, nextOrigin.Value, nextDestination.Value);
        }

        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var piece = after[row, col];
                if (piece == null || !piece.Revealed || ColorToInt(piece.Color) != ownerColor)
                {
                    continue;
                }

                var from = new Vector2Int(row, col);
                var moves = CollectLegalMoves(after, from);
                for (var i = 0; i < moves.Count; i++)
                {
                    var move = moves[i];
                    var target = after[move.x, move.y];
                    if (target != null && target.Rank == piece.Rank)
                    {
                        continue;
                    }

                    if (MiniStandWillDeadPity(move, after, opponentColor) == 1)
                    {
                        canEat++;
                    }
                }
            }
        }

        return canEat >= 2 ? 1 : 0;
    }

    private int MiniOwnerNextCanEatDeadP(Vector2Int? nextOrigin, Vector2Int? nextDestination, Piece[,] board, int ownerColor)
    {
        if (!nextDestination.HasValue)
        {
            return 0;
        }

        var opponentColor = OpponentColor(ownerColor);
        var after = CloneBoard(board);
        if (nextOrigin.HasValue && nextDestination.HasValue)
        {
            ApplyMove(after, nextOrigin.Value, nextDestination.Value);
        }

        var myPiece = after[nextDestination.Value.x, nextDestination.Value.y];
        if (myPiece == null)
        {
            return 0;
        }

        var myMoves = CollectLegalMoves(after, nextDestination.Value);
        for (var i = 0; i < myMoves.Count; i++)
        {
            var eatTargetCell = myMoves[i];
            var target = after[eatTargetCell.x, eatTargetCell.y];
            if (target == null || !target.Revealed || ColorToInt(target.Color) != opponentColor)
            {
                continue;
            }

            if (target.Rank == myPiece.Rank)
            {
                continue;
            }

            if (MiniStandWillDeadPity(eatTargetCell, after, opponentColor) == 1)
            {
                var eatStep = 0;
                var targetMoves = CollectLegalMoves(after, eatTargetCell);
                for (var m = 0; m < targetMoves.Count; m++)
                {
                    if (MiniWillDeadPityUncheckWillDead(eatTargetCell, targetMoves[m], after, opponentColor) == 1)
                    {
                        eatStep++;
                    }
                }

                if (eatStep == targetMoves.Count)
                {
                    return 1;
                }
            }
        }

        return 0;
    }

    private int MiniStandWillDeadPity(Vector2Int origin, Piece[,] board, int ownerColor)
    {
        var opponentColor = OpponentColor(ownerColor);
        if (board[origin.x, origin.y] == null)
        {
            return 0;
        }

        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var piece = board[row, col];
                if (piece == null || !piece.Revealed || ColorToInt(piece.Color) != opponentColor)
                {
                    continue;
                }

                var from = new Vector2Int(row, col);
                var moves = CollectLegalMoves(board, from);
                for (var i = 0; i < moves.Count; i++)
                {
                    if (moves[i] == origin && MiniWillDeadPity(from, moves[i], board, opponentColor) == 0)
                    {
                        return 1;
                    }
                }
            }
        }

        return 0;
    }

    private int MiniWillDeadPityUncheckWillDead(Vector2Int nextOrigin, Vector2Int? nextDestination, Piece[,] board, int ownerColor)
    {
        var moving = board[nextOrigin.x, nextOrigin.y];
        if (moving == null)
        {
            return 0;
        }

        var captured = nextDestination.HasValue ? board[nextDestination.Value.x, nextDestination.Value.y] : null;
        if (captured != null)
        {
            if (moving.Rank == 2 && captured.Rank > 4)
            {
                return 0;
            }

            if (captured.Rank == moving.Rank)
            {
                return 0;
            }
        }

        var after = CloneBoard(board);
        if (nextDestination.HasValue)
        {
            ApplyMove(after, nextOrigin, nextDestination.Value);
        }

        var opponentColor = OpponentColor(ownerColor);
        var pity = 0;
        Vector2Int? i2 = null;
        Vector2Int? j2 = null;

        for (var row = 0; row < Rows; row++)
        {
            if (pity == 1)
            {
                break;
            }

            for (var col = 0; col < Cols; col++)
            {
                var piece = after[row, col];
                if (piece == null || !piece.Revealed || ColorToInt(piece.Color) != opponentColor)
                {
                    continue;
                }

                var from = new Vector2Int(row, col);
                var moves = CollectLegalMoves(after, from);
                for (var m = 0; m < moves.Count; m++)
                {
                    if (!nextDestination.HasValue || moves[m] != nextDestination.Value)
                    {
                        continue;
                    }

                    if (captured == null)
                    {
                        i2 = from;
                        j2 = moves[m];
                        pity = 1;
                        break;
                    }

                    var moveValue = MiniEatingValueToScore(moving.Rank, _kingLive, OpponentColor(ownerColor));
                    var capturedValue = MiniEatingValueToScore(captured.Rank, _kingLive, ownerColor);
                    if (moveValue > capturedValue)
                    {
                        i2 = from;
                        j2 = moves[m];
                        pity = 1;
                        break;
                    }
                }

                if (pity == 1)
                {
                    break;
                }
            }
        }

        if (i2.HasValue && j2.HasValue)
        {
            var after2 = CloneBoard(after);
            ApplyMove(after2, i2.Value, j2.Value);
            var nowAtJ2 = after2[j2.Value.x, j2.Value.y];
            if (nowAtJ2 == null)
            {
                return pity;
            }

            for (var row = 0; row < Rows; row++)
            {
                if (pity == 0)
                {
                    break;
                }

                for (var col = 0; col < Cols; col++)
                {
                    var piece = after2[row, col];
                    if (piece == null || !piece.Revealed || ColorToInt(piece.Color) != ownerColor)
                    {
                        continue;
                    }

                    var from = new Vector2Int(row, col);
                    var moves = CollectLegalMoves(after2, from);
                    for (var m = 0; m < moves.Count; m++)
                    {
                        if (moves[m] != j2.Value)
                        {
                            continue;
                        }

                        var moveValue = MiniEatingValueToScore(moving.Rank, _kingLive, OpponentColor(ownerColor));
                        var nowValue = MiniEatingValueToScore(nowAtJ2.Rank, _kingLive, ownerColor);
                        if (moveValue <= nowValue)
                        {
                            pity = 0;
                            break;
                        }
                    }
                }
            }
        }

        return pity;
    }

    private int MiniWillDeadPityEvenEqual(Vector2Int nextOrigin, Vector2Int? nextDestination, Piece[,] board, int ownerColor)
    {
        var moving = board[nextOrigin.x, nextOrigin.y];
        if (moving == null)
        {
            return 0;
        }

        var captured = nextDestination.HasValue ? board[nextDestination.Value.x, nextDestination.Value.y] : null;
        if (captured != null && moving.Rank == 2 && captured.Rank > 4)
        {
            return 0;
        }

        var after = CloneBoard(board);
        if (nextDestination.HasValue)
        {
            ApplyMove(after, nextOrigin, nextDestination.Value);
        }

        var opponentColor = OpponentColor(ownerColor);
        var pity = 0;
        Vector2Int? i2 = null;
        Vector2Int? j2 = null;

        for (var row = 0; row < Rows; row++)
        {
            if (pity == 1)
            {
                break;
            }

            for (var col = 0; col < Cols; col++)
            {
                var piece = after[row, col];
                if (piece == null || !piece.Revealed || ColorToInt(piece.Color) != opponentColor)
                {
                    continue;
                }

                var from = new Vector2Int(row, col);
                var moves = CollectLegalMoves(after, from);
                for (var m = 0; m < moves.Count; m++)
                {
                    if (!nextDestination.HasValue || moves[m] != nextDestination.Value)
                    {
                        continue;
                    }

                    if (captured == null)
                    {
                        i2 = from;
                        j2 = moves[m];
                        pity = 1;
                        break;
                    }

                    var moveValue = MiniEatingValueToScore(moving.Rank, _kingLive, OpponentColor(ownerColor));
                    var capturedValue = MiniEatingValueToScore(captured.Rank, _kingLive, ownerColor);
                    if (moveValue >= capturedValue)
                    {
                        i2 = from;
                        j2 = moves[m];
                        pity = 1;
                        break;
                    }
                }

                if (pity == 1)
                {
                    break;
                }
            }
        }

        if (i2.HasValue && j2.HasValue)
        {
            var after2 = CloneBoard(after);
            ApplyMove(after2, i2.Value, j2.Value);
            var nowAtJ2 = after2[j2.Value.x, j2.Value.y];
            if (nowAtJ2 == null)
            {
                return pity;
            }

            for (var row = 0; row < Rows; row++)
            {
                if (pity == 0)
                {
                    break;
                }

                for (var col = 0; col < Cols; col++)
                {
                    var piece = after2[row, col];
                    if (piece == null || !piece.Revealed || ColorToInt(piece.Color) != ownerColor)
                    {
                        continue;
                    }

                    var from = new Vector2Int(row, col);
                    var moves = CollectLegalMoves(after2, from);
                    for (var m = 0; m < moves.Count; m++)
                    {
                        if (moves[m] != j2.Value)
                        {
                            continue;
                        }

                        var moveValue = MiniEatingValueToScore(moving.Rank, _kingLive, OpponentColor(ownerColor));
                        var nowValue = MiniEatingValueToScore(nowAtJ2.Rank, _kingLive, ownerColor);
                        if (moveValue < nowValue)
                        {
                            pity = 0;
                            break;
                        }
                    }
                }
            }
        }

        return pity;
    }

    private int MiniWillDeadPity(Vector2Int nextOrigin, Vector2Int? nextDestination, Piece[,] board, int ownerColor)
    {
        if (MiniWillDead(nextOrigin, board, OpponentColor(ownerColor)) == 1)
        {
            return 0;
        }

        var moving = board[nextOrigin.x, nextOrigin.y];
        if (moving == null)
        {
            return 0;
        }

        var captured = nextDestination.HasValue ? board[nextDestination.Value.x, nextDestination.Value.y] : null;
        if (captured != null)
        {
            if (moving.Rank == 2 && captured.Rank > 5)
            {
                return 0;
            }

            if (captured.Rank == moving.Rank)
            {
                return 0;
            }
        }

        var after = CloneBoard(board);
        if (nextDestination.HasValue)
        {
            ApplyMove(after, nextOrigin, nextDestination.Value);
        }

        var opponentColor = OpponentColor(ownerColor);
        var pity = 0;
        Vector2Int? i2 = null;
        Vector2Int? j2 = null;
        Vector2Int? i3 = null;
        Vector2Int? j3 = null;

        for (var row = 0; row < Rows; row++)
        {
            if (pity == 1)
            {
                break;
            }

            for (var col = 0; col < Cols; col++)
            {
                var piece = after[row, col];
                if (piece == null || !piece.Revealed || ColorToInt(piece.Color) != opponentColor)
                {
                    continue;
                }

                var from = new Vector2Int(row, col);
                var moves = CollectLegalMoves(after, from);
                for (var m = 0; m < moves.Count; m++)
                {
                    if (!nextDestination.HasValue || moves[m] != nextDestination.Value)
                    {
                        continue;
                    }

                    if (captured == null)
                    {
                        i2 = from;
                        j2 = moves[m];
                        pity = 1;
                        break;
                    }

                    var moveValue = MiniEatingValueToScore(moving.Rank, _kingLive, OpponentColor(ownerColor));
                    var capturedValue = MiniEatingValueToScore(captured.Rank, _kingLive, ownerColor);
                    if (moveValue > capturedValue)
                    {
                        i2 = from;
                        j2 = moves[m];
                        pity = 1;
                        break;
                    }
                }

                if (pity == 1)
                {
                    break;
                }
            }
        }

        Piece[,] after2 = null;
        if (i2.HasValue && j2.HasValue)
        {
            after2 = CloneBoard(after);
            ApplyMove(after2, i2.Value, j2.Value);
            var nowAtJ2 = after2[j2.Value.x, j2.Value.y];
            if (nowAtJ2 == null)
            {
                return pity;
            }

            for (var row = 0; row < Rows; row++)
            {
                if (pity == 0)
                {
                    break;
                }

                for (var col = 0; col < Cols; col++)
                {
                    var piece = after2[row, col];
                    if (piece == null || !piece.Revealed || ColorToInt(piece.Color) != ownerColor)
                    {
                        continue;
                    }

                    var from = new Vector2Int(row, col);
                    var moves = CollectLegalMoves(after2, from);
                    for (var m = 0; m < moves.Count; m++)
                    {
                        if (moves[m] != j2.Value)
                        {
                            continue;
                        }

                        var moveValue = MiniEatingValueToScore(moving.Rank, _kingLive, OpponentColor(ownerColor));
                        var nowValue = MiniEatingValueToScore(nowAtJ2.Rank, _kingLive, ownerColor);
                        if (moveValue <= nowValue)
                        {
                            i3 = from;
                            j3 = moves[m];
                            pity = 0;
                            break;
                        }
                    }
                }
            }
        }

        if (after2 != null && i3.HasValue && j3.HasValue)
        {
            var after3 = CloneBoard(after2);
            ApplyMove(after3, i3.Value, j3.Value);

            for (var row = 0; row < Rows; row++)
            {
                if (pity == 1)
                {
                    break;
                }

                for (var col = 0; col < Cols; col++)
                {
                    var piece = after3[row, col];
                    if (piece == null || !piece.Revealed || ColorToInt(piece.Color) != opponentColor)
                    {
                        continue;
                    }

                    var from = new Vector2Int(row, col);
                    var moves = CollectLegalMoves(after3, from);
                    for (var m = 0; m < moves.Count; m++)
                    {
                        if (moves[m] == j3.Value)
                        {
                            pity = 1;
                            break;
                        }
                    }

                    if (pity == 1)
                    {
                        break;
                    }
                }
            }
        }

        return pity;
    }

    private float MiniEatingValueToScore(int value, int[] king, int ownerColor)
    {
        if (value == 1)
        {
            return king[ownerColor] == 1 ? 24f : 20f;
        }

        if (value == 2)
        {
            return 150f;
        }

        if (value == 3)
        {
            return 23f;
        }

        if (value == 4)
        {
            return 49f;
        }

        if (value == 5)
        {
            return 99f;
        }

        if (value == 6)
        {
            return 300f;
        }

        if (value == 7)
        {
            return 599f;
        }

        return 0f;
    }

    private void ScheduleAI()
    {
        _aiActionTime = Time.unscaledTime + 0.35f;
    }

    private void EvaluateMaterialResult()
    {
        var playerCount = CountPieces(_playerColor);
        var aiCount = CountPieces(_aiColor);

        if (playerCount <= 0)
        {
            SetResult(GameResult.PlayerLose);
            return;
        }

        if (aiCount <= 0)
        {
            SetResult(GameResult.PlayerWin);
            return;
        }
    }

    private void EvaluateNoMoveForCurrentTurn()
    {
        if (_result != GameResult.Playing || _firstMove || !_colorsAssigned)
        {
            return;
        }

        if (HiddenCount() == 0)
        {
            var playerCanMove = HasAnyLegalMove(_playerColor);
            var aiCanMove = HasAnyLegalMove(_aiColor);

            if (!playerCanMove && !aiCanMove)
            {
                SetResult(GameResult.Draw);
                return;
            }

            if (_turn == TurnState.Player && !playerCanMove)
            {
                SetResult(GameResult.PlayerLose);
                return;
            }

            if (_turn == TurnState.AI && !aiCanMove)
            {
                SetResult(GameResult.PlayerWin);
            }
        }
    }

    private bool HasAnyLegalMove(PieceColor color)
    {
        return HasAnyLegalMove(_board, color);
    }

    private bool HasAnyLegalMove(Piece[,] board, PieceColor color)
    {
        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var piece = board[row, col];
                if (piece == null || !piece.Revealed || piece.Color != color)
                {
                    continue;
                }

                if (CollectLegalMoves(board, new Vector2Int(row, col)).Count > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private int CountPieces(PieceColor color)
    {
        return CountPieces(_board, color);
    }

    private int CountPieces(Piece[,] board, PieceColor color)
    {
        var count = 0;
        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var piece = board[row, col];
                if (piece != null && piece.Color == color)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private int HiddenCount()
    {
        return HiddenCount(_board);
    }

    private int HiddenCount(Piece[,] board)
    {
        var count = 0;
        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var piece = board[row, col];
                if (piece != null && !piece.Revealed)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private List<Vector2Int> CollectLegalMoves(Vector2Int origin)
    {
        return CollectLegalMoves(_board, origin);
    }

    private List<Vector2Int> CollectLegalMoves(Piece[,] board, Vector2Int origin)
    {
        var moves = new List<Vector2Int>();
        var piece = board[origin.x, origin.y];

        if (piece == null || !piece.Revealed)
        {
            return moves;
        }

        foreach (var dir in CardinalDirections)
        {
            var nr = origin.x + dir.x;
            var nc = origin.y + dir.y;
            if (!InBounds(nr, nc))
            {
                continue;
            }

            var target = board[nr, nc];
            if (target == null)
            {
                moves.Add(new Vector2Int(nr, nc));
                continue;
            }

            if (!target.Revealed || target.Color == piece.Color)
            {
                continue;
            }

            if (piece.Rank != 2 && CanCapture(piece.Rank, target.Rank))
            {
                moves.Add(new Vector2Int(nr, nc));
            }
        }

        if (piece.Rank == 2)
        {
            AddCannonCaptures(board, origin, piece.Color, moves);
        }

        return moves;
    }

    private void AddCannonCaptures(Piece[,] board, Vector2Int origin, PieceColor ownerColor, List<Vector2Int> moves)
    {
        foreach (var dir in CardinalDirections)
        {
            var jumped = false;
            var nr = origin.x + dir.x;
            var nc = origin.y + dir.y;

            while (InBounds(nr, nc))
            {
                var piece = board[nr, nc];
                if (piece != null)
                {
                    if (!jumped)
                    {
                        jumped = true;
                    }
                    else
                    {
                        if (piece.Revealed && piece.Color != ownerColor)
                        {
                            moves.Add(new Vector2Int(nr, nc));
                        }

                        break;
                    }
                }

                nr += dir.x;
                nc += dir.y;
            }
        }
    }

    private static bool CanCapture(int attackerRank, int defenderRank)
    {
        if (attackerRank == 1 && defenderRank == 7)
        {
            return true;
        }

        if (attackerRank == 7 && defenderRank == 1)
        {
            return false;
        }

        return attackerRank >= defenderRank;
    }

    private static bool InBounds(int row, int col)
    {
        return row >= 0 && row < Rows && col >= 0 && col < Cols;
    }

    private static bool ContainsCell(List<Vector2Int> list, Vector2Int target)
    {
        foreach (var cell in list)
        {
            if (cell == target)
            {
                return true;
            }
        }

        return false;
    }

    private static PieceColor Opponent(PieceColor color)
    {
        return color == PieceColor.Black ? PieceColor.Red : PieceColor.Black;
    }

    private void RefreshAllCells()
    {
        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                RefreshCell(row, col, false);
            }
        }
    }

    private void RefreshCell(int row, int col, bool isSelected)
    {
        var image = _pieceImages[row, col];
        var piece = _board[row, col];

        if (piece == null)
        {
            image.enabled = false;
            return;
        }

        image.enabled = true;
        image.rectTransform.sizeDelta = new Vector2(_cellWidth, _cellHeight);

        if (!piece.Revealed)
        {
            image.sprite = GetSprite("back");
        }
        else
        {
            image.sprite = GetPieceSprite(piece, isSelected);
        }

        if (!_isDragging || !_selectedCell.HasValue || _selectedCell.Value.x != row || _selectedCell.Value.y != col)
        {
            image.rectTransform.anchoredPosition = _cellCenter[row, col];
        }
    }

    private Sprite GetPieceSprite(Piece piece, bool selected)
    {
        var baseName = PieceName(piece);
        if (selected && _sprites.TryGetValue(baseName + "S", out var selectedSprite) && selectedSprite != null)
        {
            return selectedSprite;
        }

        if (_sprites.TryGetValue(baseName, out var normal) && normal != null)
        {
            return normal;
        }

        return GetSprite("back");
    }

    private static string PieceName(Piece piece)
    {
        var colorPrefix = piece.Color == PieceColor.Black ? "B" : "R";
        var rankCode = piece.Rank switch
        {
            1 => "P",
            2 => "C",
            3 => "N",
            4 => "R",
            5 => "B",
            6 => "A",
            7 => "K",
            _ => "P"
        };

        return colorPrefix + rankCode;
    }

    private Sprite GetSprite(string name)
    {
        _sprites.TryGetValue(name, out var sprite);
        return sprite;
    }

    private void PlaySound(string key)
    {
        if (_audioSource == null)
        {
            return;
        }

        if (_clips.TryGetValue(key, out var clip) && clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    private void PlayEndSoundIfNeeded()
    {
        if (_endSoundPlayed)
        {
            return;
        }

        if (_result == GameResult.PlayerWin)
        {
            PlaySound("WIN");
            _endSoundPlayed = true;
        }
        else if (_result == GameResult.PlayerLose || _result == GameResult.Draw)
        {
            PlaySound("LOSS");
            _endSoundPlayed = true;
        }
    }

    private void SetResult(GameResult result)
    {
        if (_result != GameResult.Playing)
        {
            return;
        }

        _result = result;
        PlayEndSoundIfNeeded();
    }

    private void UpdateStatusText()
    {
        if (_statusText == null)
        {
            return;
        }

        if (_result == GameResult.Playing)
        {
            var colorText = _colorsAssigned ? (_playerColor == PieceColor.Black ? "Black" : "Red") : "Unknown";
            string turnText;

            if (_firstMove)
            {
                turnText = _playerFirst
                    ? "Your first move: tap any covered piece."
                    : "AI moves first: waiting for opening flip.";
            }
            else
            {
                turnText = _turn == TurnState.Player ? "Your turn." : "AI thinking (up to ~10s)...";
            }

            _statusText.text =
                "Taiwan Dark Chess\n" +
                $"Player Color: {colorText}\n" +
                $"{turnText}";

            return;
        }

        if (_result == GameResult.PlayerWin)
        {
            _statusText.text = "You Win!";
        }
        else if (_result == GameResult.PlayerLose)
        {
            _statusText.text = "You Lose.";
        }
        else
        {
            _statusText.text = "Draw game.";
        }
    }

    private Image CreateImage(string name, Transform parent, Sprite sprite)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);

        var image = go.GetComponent<Image>();
        image.sprite = sprite;
        if (_uiImageMaterial != null)
        {
            image.material = _uiImageMaterial;
        }

        return image;
    }

    private void EnsureUIMaterials()
    {
        if (_uiImageMaterial != null && _uiTextMaterial != null)
        {
            return;
        }

        var shader = Resources.Load<Shader>("Shaders/DarkChessUnlit");
        if (shader == null)
        {
            shader = Shader.Find("DarkChess/UnlitSprite");
        }

        if (shader == null)
        {
            shader = Shader.Find("UI/Default");
        }

        if (shader == null)
        {
            Debug.LogWarning("No compatible UI shader found. UI may render incorrectly.");
            return;
        }

        if (_uiImageMaterial == null)
        {
            _uiImageMaterial = new Material(shader)
            {
                name = "DarkChess_RuntimeUIMaterial"
            };
        }

        if (_uiTextMaterial == null)
        {
            _uiTextMaterial = new Material(shader)
            {
                name = "DarkChess_RuntimeUITextMaterial"
            };
        }
    }

    private void SetElementAtTopLeft(RectTransform rect, float x, float y, float width, float height)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = TopLeftToAnchored(new Vector2(x, y), width, height);
    }

    private Vector2 CellTopLeft(int row, int col)
    {
        var x = col < 4
            ? LeftStartX + col * _cellWidth
            : RightStartX + (col - 4) * _cellWidth;
        var y = StartY + row * _cellHeight;

        return new Vector2(x, y);
    }

    private static Vector2 TopLeftToAnchored(Vector2 topLeft, float width, float height)
    {
        var cx = topLeft.x + width * 0.5f - DesignWidth * 0.5f;
        var cy = DesignHeight * 0.5f - (topLeft.y + height * 0.5f);

        return new Vector2(cx, cy);
    }

    private bool TryScreenToLocal(Vector2 screen, out Vector2 local)
    {
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(_boardRoot, screen, null, out local);
    }

    private bool TryScreenToPixel(Vector2 screen, out Vector2 pixel)
    {
        if (!TryScreenToLocal(screen, out var local))
        {
            pixel = default;
            return false;
        }

        pixel = new Vector2(local.x + DesignWidth * 0.5f, DesignHeight * 0.5f - local.y);
        return pixel.x >= 0f && pixel.x <= DesignWidth && pixel.y >= 0f && pixel.y <= DesignHeight;
    }

    private bool TryPixelToCell(Vector2 pixel, out Vector2Int cell)
    {
        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Cols; col++)
            {
                var topLeft = _cellTopLeft[row, col];
                if (pixel.x >= topLeft.x && pixel.x <= topLeft.x + _cellWidth &&
                    pixel.y >= topLeft.y && pixel.y <= topLeft.y + _cellHeight)
                {
                    cell = new Vector2Int(row, col);
                    return true;
                }
            }
        }

        cell = default;
        return false;
    }

    private static bool ContainsTopLeftRect(Rect rect, Vector2 pixel)
    {
        return pixel.x >= rect.xMin && pixel.x <= rect.xMin + rect.width &&
               pixel.y >= rect.yMin && pixel.y <= rect.yMin + rect.height;
    }

    private static bool TryGetPointerDown(out Vector2 screenPos)
    {
        foreach (var touch in InputSystemTouch.activeTouches)
        {
            if (touch.phase == InputSystemTouchPhase.Began)
            {
                screenPos = touch.screenPosition;
                return true;
            }
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            screenPos = Mouse.current.position.ReadValue();
            return true;
        }

        screenPos = default;
        return false;
    }

    private static bool TryGetPointerHeld(out Vector2 screenPos)
    {
        foreach (var touch in InputSystemTouch.activeTouches)
        {
            if (touch.phase == InputSystemTouchPhase.Moved || touch.phase == InputSystemTouchPhase.Stationary)
            {
                screenPos = touch.screenPosition;
                return true;
            }
        }

        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            screenPos = Mouse.current.position.ReadValue();
            return true;
        }

        screenPos = default;
        return false;
    }

    private static bool TryGetPointerUp(out Vector2 screenPos)
    {
        foreach (var touch in InputSystemTouch.activeTouches)
        {
            if (touch.phase == InputSystemTouchPhase.Ended || touch.phase == InputSystemTouchPhase.Canceled)
            {
                screenPos = touch.screenPosition;
                return true;
            }
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            screenPos = Mouse.current.position.ReadValue();
            return true;
        }

        screenPos = default;
        return false;
    }
}
