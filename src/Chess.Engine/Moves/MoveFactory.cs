namespace ChessEngine.Moves;

public static class MoveFactory
{
    public static IEnumerable<Square> AttackProfile(GameState state, Piece piece, AlgebraicNotation score)
    {
        return piece.Type switch
        {
            PieceType.PAWN => new Pawn(state, score, piece).GetAttackSquares(),
            PieceType.ROOK => new Rook(state, piece).GetAttackSquares(),
            PieceType.BISHOP => new Bishop(state, piece).GetAttackSquares(),
            PieceType.QUEEN => new Queen(state, piece).GetAttackSquares(),
            PieceType.KING => new King(state, piece).GetAttackSquares(),
            PieceType.KNIGHT => new Knight(state, piece).GetAttackSquares(),
            _ => throw new NotImplementedException()
        };
    }
}
