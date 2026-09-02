using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine.Moves;

internal class Knight(GameState State, Piece Piece) : Moves(State)
{
    public override IEnumerable<Square> GetMoveSquares()
    {
        int[] movePattern = [6, 10, 13, 15];
        return PatternToSquares(movePattern, Piece.Current);
    }

    public override IEnumerable<Square> GetAttackSquares()
    {
        int[] movePattern = [6, 10, 13, 15];
        return PatternToSquares(movePattern, Piece.Current, true);
    }
}
