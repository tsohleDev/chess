using ChessEngine.MovesGenerator;

namespace ChessEngine.Core;

public static class AttackScanner
{
    public static bool IsSquareAttacked(Board board, Square square, Player attackerColor)
    {
        int opp = (int)attackerColor;

        // 1. Pawn attacks (Reverse the attack direction)
        ulong pawnMask = (ulong)square;
        if (attackerColor == Player.WHITE)
        {
            ulong attacks = ((pawnMask >> 7) & ~Bitboard.FileA) | ((pawnMask >> 9) & ~Bitboard.FileH);
            if ((attacks & board.Pieces[(int)PieceType.PAWN] & board.Colors[opp]) != 0) return true;
        }
        else
        {
            ulong attacks = ((pawnMask << 7) & ~Bitboard.FileH) | ((pawnMask << 9) & ~Bitboard.FileA);
            if ((attacks & board.Pieces[(int)PieceType.PAWN] & board.Colors[opp]) != 0) return true;
        }

        // 2. Knight Attacks
        if ((MoveGenerator.GetKnightAttacks(square) & board.Pieces[(int)PieceType.KNIGHT] & board.Colors[opp]) != 0) return true;

        // 3. King Attacks
        if ((MoveGenerator.GetKingAttacks(square) & board.Pieces[(int)PieceType.KING] & board.Colors[opp]) != 0) return true;

        // Note: For Rooks, Bishops, Queens in a fully optimized engine, you use Magic Bitboards.
        // For this snippet, you would implement ray-casting outward from the square.
        // If a ray hits an enemy sliding piece, return true.
        return false;
    }

    public static bool IsInCheck(Board board, Player color)
    {
        ulong kingBitboard = board.Pieces[(int)PieceType.KING] & board.Colors[(int)color];
        Square kingSquare = (Square)Bitboard.GetLSB(kingBitboard);
        return IsSquareAttacked(board, kingSquare, color == Player.WHITE ? Player.BLACK : Player.WHITE);
    }
}