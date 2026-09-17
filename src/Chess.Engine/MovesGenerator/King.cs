using ChessEngine.Core;

namespace ChessEngine.MovesGenerator;

public static partial class MoveGenerator
{
    public static readonly (CastlingRights, Square, Square)[] WhiteCastle = [
        (CastlingRights.WHITE_KING_SIDE, Square.e1, Square.g1),
        (CastlingRights.WHITE_QUEEN_SIDE, Square.e1, Square.b1)
    ];

    public static readonly (CastlingRights, Square, Square)[] BlackCastle = [
        (CastlingRights.BLACK_KING_SIDE, Square.e1, Square.g1),
        (CastlingRights.BLACK_QUEEN_SIDE, Square.e1, Square.b1)
    ];


    private static void GenerateKingMoves(Board board, ref MoveList moves)
    {
        ulong kings = board.Pieces[(int)PieceType.KING] & board.Colors[(int)board.Turn];
        ulong validTargets = ~board.Colors[(int)board.Turn];

        ulong kingSquare = Bitboard.PopLSB(ref kings);
        ulong attackSquares = GetKingAttacks((Square)kingSquare) & validTargets;
        GetKingCastling(board, ref moves);

        while (attackSquares != 0)
        {
            ulong attack = Bitboard.PopLSB(ref attackSquares);
            PieceType captured = board.GetPieceAt((Square)attack);
            moves.Add(new Move((Square)kingSquare, (Square)attack, PieceType.KING, captured));
        }

        if (kings != 0) throw new Exception("More than one kings for one playe");
    }

    public static ulong GetKingAttacks(Square square)
    {
        ulong king = (ulong)square;
        ulong attacks = ((king << 1) & ~Bitboard.FileA) | ((king >> 1) & ~Bitboard.FileH);
        ulong kingAndFlanks = king | attacks;
        attacks |= (kingAndFlanks << 8) | (kingAndFlanks >> 8);
        return attacks;
    }

    public static void GetKingCastling(Board board, ref MoveList moves)
    {
        var castling = board.Turn == Player.WHITE ? WhiteCastle : BlackCastle;

        foreach (var (right, from, to) in castling)
        {
            if (board.Castling.HasFlag(right))
            {
                moves.Add(new Move(from, to, PieceType.KING, PieceType.NONE, false, right));
            }
        }
    }
}
