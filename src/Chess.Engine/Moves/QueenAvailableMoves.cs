using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine.Moves
{
    internal class QueenAvailableMoves : IAvailableMoves
    {
        public override IEnumerable<Square> GetSquares(Piece piece)
        {
            int current = (int)piece.Current;
            var pieceRank = GetRankRange(piece);

            return LegalDiagonals(current)
                .Union(LegalFile(current))
                .Union(LegalRank((current, pieceRank.Item1, pieceRank.Item2)));
        }
    }
}
