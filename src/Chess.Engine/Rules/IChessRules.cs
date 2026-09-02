namespace ChessEngine.Rules;

internal interface IChessRules
{
    public bool IsMoveValid(Piece from, Piece to);
    public bool IsCheckMate(Square[] attackProfile);
    public bool IsCheck(Square[] attackProfile);
    public bool IsDraw();
}
