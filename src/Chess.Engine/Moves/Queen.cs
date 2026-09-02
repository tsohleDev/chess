using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine.Moves;

internal class Queen(GameState State, Piece Piece) : Moves(State)
{
    public override IEnumerable<Square> GetMoveSquares()
    {
        ulong current = (ulong)Piece.Current;
        var pieceRank = GetRankRange(current);

        return LegalDiagonals(current)
            .Union(LegalFile(current))
            .Union(LegalRank((current, pieceRank.Item1, pieceRank.Item2)));
    }

    public override IEnumerable<Square> GetAttackSquares()
    {
        ulong current = (ulong)Piece.Current;
        var pieceRank = GetRankRange(current);

        return LegalDiagonals(current, true)
            .Union(LegalFile(current, true))
            .Union(LegalRank((current, pieceRank.Item1, pieceRank.Item2)));
    }
}
