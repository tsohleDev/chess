namespace ChessEngine.Enums;

[Flags]
public enum CastlingRights: byte
{
    NONE = 0,
    WHITE_QUEEN_SIDE = 1,
    WHITE_KING_SIDE = 1 << 1,
    BLACK_QUEEN_SIDE = 1 << 2,
    BLACK_KING_SIDE = 1 << 3,
}
