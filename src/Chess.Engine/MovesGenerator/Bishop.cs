using ChessEngine.Core;

namespace ChessEngine.MovesGenerator;

public static partial class MoveGenerator
{
    private static void GenerateBishopMoves(Board board, ref MoveList moves)
    {
        ulong bishops = board.Pieces[(int)PieceType.BISHOP] & board.Colors[(int)board.Turn];
        ulong validTargets = ~board.Colors[(int)board.Turn];
        ulong occupancy = board.Occupancy;

        while (bishops != 0)
        {
            ulong from = Bitboard.PopLSB(ref bishops);
            ulong attacks = GetBishopAttacks((Square)from, occupancy) & validTargets;

            while (attacks != 0)
            {
                ulong to = Bitboard.PopLSB(ref attacks);
                PieceType captured = board.GetPieceAt((Square)to);
                moves.Add(new Move((Square)from, (Square)to, PieceType.BISHOP, captured));
            }
        }
    }

    public static ulong GetBishopAttacks(Square square, ulong occupancy)
    {
        ulong attacks = 0;
        ulong current = (ulong)square;

        // North-East
        TraceRay(ref attacks, occupancy, current, 9, Bitboard.FileA , (x, y) => x << y);
        // North-West
        TraceRay(ref attacks, occupancy, current, 7, Bitboard.FileH, (x, y) => x << y);
        // South-East
        TraceRay(ref attacks, occupancy, current, 7, Bitboard.FileA, (x, y) => x >> y);
        // South-West
        TraceRay(ref attacks, occupancy, current, 9, Bitboard.FileH, (x, y) => x >> y);
        return attacks;
    }

    private static void TraceRay(ref ulong attacks, ulong occupancy, ulong current, int offset, ulong FallOfffile, Func<ulong, int, ulong> shift)
    {
        ulong ray = current;
        while ((ray = shift(ray, offset) & ~FallOfffile) != 0)
        {
            attacks |= ray;
            if ((ray & occupancy) != 0) break;
        }
    }
}