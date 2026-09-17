namespace ChessEngine.Enums;

public enum CastlingFlags : byte
{
    NONE = 0,
    WHITE_KING_MOVED = 1,
    BLACK_KING_MOVED = 1 << 1,
    WHITE_QUEEN_SIDE_ROOK_MOVED = 1 << 2,
    WHITE_KING_SIDE_ROOK_MOVED = 1 << 3,
    BLACK_QUEEN_SIDE_ROOK_MOVED = 1 << 4,
    BLACK_KING_SIDE_ROOK_MOVED = 1 << 5,
}
