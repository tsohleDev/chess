using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine.Moves;

internal class Rook(GameState State, Piece Piece) : Moves(State)
{
    public override IEnumerable<Square> GetMoveSquares()
    {
        ulong current = (ulong)Piece.Current;
        var pieceRank = GetRankRange(current);

        var rank = LegalRank((current, pieceRank.Item1, pieceRank.Item2));
        var file = LegalFile(current);

        return rank.Union(file);
    }

    public override IEnumerable<Square> GetAttackSquares()
    {
        ulong current = (ulong)Piece.Current;
        var pieceRank = GetRankRange(current);

        var rank = LegalRank((current, pieceRank.Item1, pieceRank.Item2), true);
        var file = LegalFile(current, true);

        return rank.Union(file);
    }
}
