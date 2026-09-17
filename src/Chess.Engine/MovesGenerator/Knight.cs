using ChessEngine.Core;

namespace ChessEngine.MovesGenerator;

public static partial class MoveGenerator
{
    private static void GenerateKnightMoves(Board board, ref MoveList moves)
    {
        ulong knights = board.Pieces[(int)PieceType.KING] & board.Colors[(int)board.Turn];
        ulong validTargets = ~board.Colors[(int)board.Turn];

        while (knights != 0)
        {
            ulong from = Bitboard.PopLSB(ref knights);
            ulong attacks = GetKnightAttacks((Square)from) & validTargets;

            while (attacks != 0)
            {
                ulong to = Bitboard.PopLSB(ref attacks);
                PieceType captured = board.GetPieceAt((Square)to);
                moves.Add(new Move((Square)from, (Square)to, PieceType.KNIGHT, captured));
            }
        }
    }

    public static ulong GetKnightAttacks(Square square)
    {
        ulong knight = (ulong)square;
        ulong attacks = (knight << 6 & ~Bitboard.FileH & ~Bitboard.FileG)
                | (knight >> 6 & ~Bitboard.Rank8 & ~Bitboard.FileA & ~Bitboard.FileB)
                | (knight << 10 & ~Bitboard.FileA & ~Bitboard.FileB)
                | (knight >> 10 & ~Bitboard.FileH & ~Bitboard.FileG)
                | (knight >> 17 & ~Bitboard.FileH)
                | (knight >> 15 & ~Bitboard.FileA)
                | (knight << 17 & ~Bitboard.FileA)
                | (knight << 15 & ~Bitboard.FileH);

        return attacks;
    }
}
