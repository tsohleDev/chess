using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine.Moves;

internal class Bishop(GameState State, Piece Piece) : Moves(State)
{
    public override IEnumerable<Square> GetMoveSquares()
    {
        ulong current = (ulong)Piece.Current;

        return LegalDiagonals(current);
    }

    public override IEnumerable<Square> GetAttackSquares()
    {
        ulong current = (ulong)Piece.Current;

        return LegalDiagonals(current, true);
    }
}
