namespace ChessEngine.Moves;

using ChessEngine.Utils;

internal class KingAvailbleMoves : IAvailableMoves
{
    public override IEnumerable<Square> GetSquares(Piece piece)
    {
        int[] movePattern = [-9, -8, -7, -1, 1, 7, 8, 9];
        return PatternToSquares(movePattern, piece.Current)
                .Order()
                .RemoveBadSquares();
    }
}
