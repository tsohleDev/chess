using ChessEngine.Core;

namespace ChessEngine.MovesGenerator;

public static partial class MoveGenerator
{
    private static void GenerateQueenMoves(Board board, ref MoveList moves)
    {
        ulong queens = board.Pieces[(int)PieceType.QUEEN] & board.Colors[(int)board.Turn];
        ulong validTargets = ~board.Colors[(int)board.Turn];
        ulong occupancy = board.Occupancy;

        while (queens != 0)
        {
            ulong from = Bitboard.PopLSB(ref queens);
            // Queen attacks are just Rook attacks + Bishop attacks combined
            ulong attacks = (GetRookAttacks((Square)from, occupancy) | GetBishopAttacks((Square)from, occupancy)) & validTargets;

            while (attacks != 0)
            {
                ulong to = Bitboard.PopLSB(ref attacks);
                PieceType captured = board.GetPieceAt((Square)to);
                moves.Add(new Move((Square)from, (Square)to, PieceType.QUEEN, captured));
            }
        }
    }
}
