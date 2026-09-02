namespace ChessEngine.Moves;

internal class King(GameState State, Piece Piece) : Moves(State)
{
    public override IEnumerable<Square> GetMoveSquares()
    {
        int[] movePattern = [1, 7, 8, 9];
        return PatternToSquares(movePattern, Piece.Current);
    }

    public override IEnumerable<Square> GetAttackSquares()
    {
        int[] movePattern = [1, 7, 8, 9];
        return PatternToSquares(movePattern, Piece.Current, true);
    }
}
