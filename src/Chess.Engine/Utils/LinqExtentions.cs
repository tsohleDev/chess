using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine.Utils;

internal static class LinqExtentions
{
    public static IEnumerable<Square> RemoveBadSquares(this IEnumerable<int> source)
    {
        GameState state = GameState.Instance;
        IEnumerable<Square> result = [];

        for(int i = 0; i < source.Count(); i++)
        {
            if (!state.ActivePieces.Any(p => (Square)source.ElementAt(i) == p.Current && p.Colour == state.Turn))
            {
                 result.Append((Square)i);
            }
        }

        return result;
    }

    public static IEnumerable<Square> RemoveBadSquaresEdges(this IEnumerable<int> source, int k)
    {
        GameState state = GameState.Instance;
        IEnumerable<Square> result = [];

        int i = 1, j = 1;
        bool continueI = true, continueJ = false;
        while (true)
        {
            if (!continueI && !continueJ) break;

            checkSquareAndIterate(k - i, ref i, ref source, ref result, ref continueI, false);
            checkSquareAndIterate(k + j, ref j, ref source, ref result, ref continueJ, true);
        }

        return result;
    }

    private static void checkSquareAndIterate(int idx, ref int i, ref IEnumerable<int> source, ref IEnumerable<Square> result, ref bool continueX, bool increment)
    {
        GameState state = GameState.Instance;
        int squreIndex = source.ElementAt(idx);
        Piece squrePiece = state.ActivePieces.First(p => (int)p.Current == idx);

        bool previousSquareOccupied = state.Squares[squreIndex];

        if (continueX && idx >= 0 && previousSquareOccupied)
        {
            continueX = false;
        }
        else
        {
            result.Append((Square)squreIndex);
            if (increment) i++; else i--;
        }
    }
}
