global using ChessEngine.Enums;
using ChessEngine.Core;
using ChessEngine.MovesGenerator;
using System.Numerics;

namespace ChessEngine;

public class Game
{
    public Board Board { get; }
    public GameResult GameResults { get; private set; }
    private readonly List<ulong> _positionHistory = [];

    public Game()
    {
        Board = new Board();
        _positionHistory.Add(Zobrist.HashBoard(Board));
    }


    /// <summary>
    /// Attemps to make a move in the board if move is valid returns true otherwise it returns false
    /// </summary>
    /// <param name="move">A move struct containing information about a move</param>
    /// <returns>the validity of the move as a boolean</returns>
    /// <see cref="Move"/>
    public bool TryMove(Move move)
    {
        var legalMoves = GenerateLegalMoves(stackalloc Move[256]);

        bool isLegal = false;
        foreach (var m in legalMoves.AsSpan())
        {
            if (m.From == move.From && m.To == move.To)
            {
                isLegal = true;
                break;
            }
        }

        if (!isLegal) return false;

        Board.MakeMove(move);

        GetGameState(ref legalMoves);

        _positionHistory.Add(Zobrist.HashBoard(Board));

        return true;
    }

    /// <summary>
    /// Checks if the game is in play or not and updates the GameState property
    /// namely: 
    /// 1. CheckMate (Player in check loses)
    /// 2. StaleMate (Draw)
    /// 3. HalfMoveClock, which is a fifty turns rule per player where no pawn captures or checks have been made : 100 half move where a half is a turn for one player (Draw)
    /// 4. Insufficient material to make a checkmate (Draw)
    /// 5. Threefolds repetition, where two board states following each other are repeated three times contiguesly in the game score (Draw)
    /// </summary>
    /// <param name="legalMoves">a collection of legal moves as calculated by the engine as a reference to a stack span</param>
    /// <returns>void</returns>
    private void GetGameState(ref MoveList legalMoves)
    {
        bool inCheck = AttackScanner.IsInCheck(Board, Board.Turn);

        if (legalMoves.Count == 0)
        {
            if (inCheck)
                GameResults = Board.Turn == Player.WHITE ? GameResult.BLACK_WINS : GameResult.WHITE_WINS;
            else
                GameResults = GameResult.DRAW;
        }

        if (Board.HalfMoveClock >= 100)
            GameResults = GameResult.DRAW;

        if (IsInsufficientMaterial())
            GameResults = GameResult.DRAW;

        if (IsThreefoldRepetition())
            GameResults = GameResult.DRAW;

        GameResults = GameResult.IN_PROGRESS;
    }

    /// <summary>
    /// Checks if the board has sufficient material to make a checkmate
    /// a check mate can be made if a player has:
    /// 1. either pawns, rooks, or queens
    /// 2. atleast two, three point pieces (knignt, bishop)
    /// </summary>
    /// <returns>if the board has insufficient material as a bool</returns>
    private bool IsInsufficientMaterial()
    {
        if ((Board.Pieces[(int)PieceType.PAWN] |
             Board.Pieces[(int)PieceType.ROOK] |
             Board.Pieces[(int)PieceType.QUEEN]) != 0)
            return false;

        int whiteKnights = BitOperations.PopCount(Board.Pieces[(int)PieceType.KNIGHT] & Board.Colors[(int)Player.WHITE]);
        int blackKnights = BitOperations.PopCount(Board.Pieces[(int)PieceType.KNIGHT] & Board.Colors[(int)Player.BLACK]);
        int whiteBishops = BitOperations.PopCount(Board.Pieces[(int)PieceType.BISHOP] & Board.Colors[(int)Player.WHITE]);
        int blackBishops = BitOperations.PopCount(Board.Pieces[(int)PieceType.BISHOP] & Board.Colors[(int)Player.BLACK]);

        int totalMinors = whiteKnights + blackKnights + whiteBishops + blackBishops;

        if (totalMinors <= 1)
            return true;

        return false;
    }

    private bool IsThreefoldRepetition()
    {
        var lastHashes = _positionHistory[^6..];
        var matches = 0;
        for (int i = 0; i < 3; i+=2)
        {
            if (lastHashes[i] == lastHashes[i+1])
            {
                matches++;
            }
        }

        return matches == 3;
    }

    private MoveList GenerateLegalMoves(Span<Move> buffer)
    {
        var legalMoves = new MoveList(buffer);
        MoveGenerator.GenerateLegalMoves(Board, ref legalMoves);
        return legalMoves;
    }
}