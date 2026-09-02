namespace ChessEngine;

public record struct GameState(ulong Squares, bool Check, Piece[] Pieces, Player Turn);