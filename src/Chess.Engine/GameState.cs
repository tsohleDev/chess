namespace ChessEngine;

public record struct GameState(ulong Squares, Piece[] Pieces, Player Turn);