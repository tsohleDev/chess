using ChessEngine.Core;

namespace ChessEngine.MovesGenerator;

public static partial class MoveGenerator
{
    public static void GenerateLegalMoves(Board board, ref MoveList legalMoves)
    {
        Span<Move> pseudoMovesBuffer = stackalloc Move[256];
        var pseudoMoves = new MoveList(pseudoMovesBuffer);

        GeneratePseudoLegalMoves(board, ref pseudoMoves);

        foreach (var move in pseudoMoves.AsSpan())
        {
            Board simulationBoard = board.Clone();
            simulationBoard.MakeMove(move);

            if (!AttackScanner.IsInCheck(simulationBoard, board.Turn))
            {
                legalMoves.Add(move);
            }
        }
    }

    private static void GeneratePseudoLegalMoves(Board board, ref MoveList moves)
    {
        GeneratePawnMoves(board, ref moves);
        GenerateKnightMoves(board, ref moves);
        GenerateBishopMoves(board, ref moves);
        GenerateQueenMoves(board, ref moves);
        GenerateRookMoves(board, ref moves);
        GenerateKingMoves(board, ref moves);
    }
}
