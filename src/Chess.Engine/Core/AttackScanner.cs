using ChessEngine.MovesGenerator;

namespace ChessEngine.Core;

public static class AttackScanner
{
    public static bool IsSquareAttacked(Board board, Square square, Player attackerColor)
    {

        if (IsAttackedBy(PieceType.PAWN, MoveGenerator.GetPawnAttacks(square, attackerColor), board, square, attackerColor)) return true;
        if (IsAttackedBy(PieceType.KING, MoveGenerator.GetKingAttacks(square), board, square, attackerColor)) return true;
        if (IsAttackedBy(PieceType.KNIGHT, MoveGenerator.GetKingAttacks(square), board, square, attackerColor)) return true;
        if (IsAttackedBy(PieceType.BISHOP, MoveGenerator.GetBishopAttacks(square, board.Occupancy), board, square, attackerColor)) return true;
        if (IsAttackedBy(PieceType.ROOK, MoveGenerator.GetRookAttacks(square, board.Occupancy), board, square, attackerColor)) return true;

        if (IsAttackedBy(PieceType.QUEEN, MoveGenerator.GetBishopAttacks(square, board.Occupancy), board, square, attackerColor)) return true;
        if (IsAttackedBy(PieceType.QUEEN, MoveGenerator.GetRookAttacks(square, board.Occupancy), board, square, attackerColor)) return true;

        return false;
    }

    private static ulong OpponentPieceSquares(Board board, PieceType piece)
    {
        var opponent = (ulong)(board.Turn) ^ 1;
        return board.Pieces[(int)piece] & board.Colors[opponent];
    }

    private static bool IsAttackedBy(PieceType piece, ulong attacks, Board board, Square square, Player attackerColor)
    {
        if ((attacks & OpponentPieceSquares(board, piece)) != 0) return true;

        return false;
    }

    public static bool IsInCheck(Board board, Player color)
    {
        ulong kingBitboard = board.Pieces[(int)PieceType.KING] & board.Colors[(int)color];
        Square kingSquare = (Square)Bitboard.GetLSB(kingBitboard);
        return IsSquareAttacked(board, kingSquare, color == Player.WHITE ? Player.BLACK : Player.WHITE);
    }
}