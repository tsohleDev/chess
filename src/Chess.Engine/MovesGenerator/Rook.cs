using ChessEngine.Core;

namespace ChessEngine.MovesGenerator;

public static partial class MoveGenerator
{
    private static void GenerateRookMoves(Board board, ref MoveList moves)
    {
        ulong rooks = board.Pieces[(int)PieceType.ROOK] & board.Colors[(int)board.Turn];
        ulong validTargets = ~board.Colors[(int)board.Turn];
        ulong occupancy = board.Occupancy;

        while (rooks != 0)
        {
            ulong from = Bitboard.PopLSB(ref rooks);
            ulong attacks = GetRookAttacks((Square)from, occupancy) & validTargets;

            while (attacks != 0)
            {
                ulong to = Bitboard.PopLSB(ref attacks);
                PieceType captured = board.GetPieceAt((Square)to);
                moves.Add(new Move((Square)from, (Square)to, PieceType.ROOK, captured));
            }
        }
    }

    public static ulong GetRookAttacks(Square square, ulong occupancy)
    {
        ulong attacks = 0;
        ulong current = (ulong)square;

        // North
        TraceRay(ref attacks, occupancy, current, 8, Bitboard.WholeBoard, (x, y) => x << y);
        // South
        TraceRay(ref attacks, occupancy, current, 8, Bitboard.WholeBoard, (x, y) => x >> y);
        // East
        TraceRay(ref attacks, occupancy, current, 1, Bitboard.WholeBoard, (x, y) => x << y);
        // West
        TraceRay(ref attacks, occupancy, current, 1, Bitboard.WholeBoard, (x, y) => x >> y);

        return attacks;
    }
}
