using ChessEngine.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine.Moves
{
    internal class KnightAvailableMoves: IAvailableMoves
    {
        public override IEnumerable<Square> GetSquares(Piece piece)
        {
            int[] movePattern = [-13, -15, -10, -6, 6, 10, 13, 15];
            return PatternToSquares(movePattern, piece.Current)
                    .RemoveBadSquares();
        }
    }
}
