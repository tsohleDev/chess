using System.Collections;

namespace ChessEngine;

public sealed class GameState
{
    private static readonly Lazy<GameState> _instance
        = new Lazy<GameState>(() => new GameState());

    private GameState()
    {
        Squares = new BitArray(64);
        PiecesFactory pieceFactory = new PiecesFactory();
        ActivePieces = pieceFactory.Pieces;
        CapturedPieces = [];
        AlgebraicNotation = [];
        Squares.SetAll(false);
    }

    public static GameState Instance
    {
        get { return _instance.Value; }
    }

    public BitArray Squares { get; private set; }
    public List<Piece> ActivePieces { get; private set; }

    public List<Piece> CapturedPieces { get; private set; }
    public List<(Square, Square)> AlgebraicNotation { get; private set; }

    public bool CheckMate { get; set; }

    public bool Check { get; set; }

    public Player Turn { get; set; }
}