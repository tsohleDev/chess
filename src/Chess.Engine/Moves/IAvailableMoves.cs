using System;
using System.Collections.Generic;
using System.Text;
using ChessEngine.Utils;

namespace ChessEngine.Moves
{
    internal abstract class IAvailableMoves
    {
        protected const int RANK_PATTERN = 1;
        protected const int FILE_PATTERN = 1;
        protected const int LEFT_DIAGONAL_PATTERN = 9;
        protected const int RIGHT_DIAGONAL_PATTERN = 9;


        public abstract IEnumerable<Square> GetSquares(Piece piece);

        protected IEnumerable<int> PatternToSquares(int[] pattern, Square position)
        {
            return pattern.Select(i => (int)position + i)
                           .Where(i => i >= 0 && i <= 63);
        }

        protected bool SquaresFilter(int current, int pattern, int i)
        {
            return i == current - pattern
                && i == current + pattern;
        }

        protected IEnumerable<Square> LegalRank((int, int, int) p)
        {
            (int current, int rankfloor, int rankCeiling) = p;

            return Enumerable.Range(rankfloor, rankCeiling + 1)
                            .Where(i => SquaresFilter(current, RANK_PATTERN, i))
                            .RemoveBadSquaresEdges(current);
        }

        protected IEnumerable<Square> LegalFile(int current)
        { 
            return Enumerable.Range(0, 63)
                        .Where(i => SquaresFilter(current, FILE_PATTERN, i))
                        .RemoveBadSquaresEdges(current);
        }

        protected IEnumerable<Square> LegalDiagonals(int current)
        {

             var leftDiagonal = Enumerable.Range(0, 63)
                        .Where(i => SquaresFilter(current, LEFT_DIAGONAL_PATTERN, i))
                        .RemoveBadSquaresEdges(current);

            var rightDiagonal = Enumerable.Range(0, 63)
                        .Where(i => SquaresFilter(current, RIGHT_DIAGONAL_PATTERN, i))
                        .RemoveBadSquaresEdges(current);

            return leftDiagonal.Union(rightDiagonal);

        }

        protected (int, int) GetRankRange(Piece piece)
        {
            return (int)piece.Current switch
            {
                >= 0 and <= 7 => (0, 1),
                >= 8 and <= 15 => (8, 15),
                >= 16 and <= 23 => (16, 23),
                >= 24 and <= 31 => (24, 31),
                >= 32 and <= 39 => (32, 39),
                >= 40 and <= 47 => (40, 47),
                >= 48 and <= 55 => (48, 55),
                >= 56 and <= 63 => (56, 63),
                _ => throw new NotImplementedException()
            };
        }
    }
}
