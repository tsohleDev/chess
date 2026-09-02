namespace ChessEngine.Core;

public readonly struct Move
{
    public Square From { get; }
    public Square To { get; }
    public PieceType MovedPiece { get; }
    public PieceType CapturedPiece { get; }
    public bool IsEnPassant { get; }
    public bool IsCastling { get; }

    public Move(Square from, Square to, PieceType moved, PieceType captured = PieceType.NONE, bool isEp = false, bool isCastle = false)
    {
        From = from;
        To = to;
        MovedPiece = moved;
        CapturedPiece = captured;
        IsEnPassant = isEp;
        IsCastling = isCastle;
    }
}