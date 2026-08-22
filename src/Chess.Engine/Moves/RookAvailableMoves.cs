using ChessEngine.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine.Moves
{
    internal class RookAvailableMoves : IAvailableMoves
    {
        public override IEnumerable<Square> GetSquares(Piece piece)
        {
            var pieceRank = GetRankRange(piece);
            int current = (int)piece.Current;

            var rank = LegalRank((current, pieceRank.Item1, pieceRank.Item2));
            var file = LegalFile(current);

            return rank.Union(file);
        }
    }
}
