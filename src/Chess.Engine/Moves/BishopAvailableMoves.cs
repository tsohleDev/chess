using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine.Moves
{
    internal class BishopAvailableMoves : IAvailableMoves
    {
        public override IEnumerable<Square> GetSquares(Piece piece)
        {
            int current = (int)piece.Current;

            return LegalDiagonals(current);
        }
    }
}
