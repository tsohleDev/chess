
namespace ChessEngine.Moves;

internal abstract class Moves(GameState State)
{
    protected const int RANK_PATTERN = 1;
    protected const int FILE_PATTERN = 8;
    protected const int LEFT_DIAGONAL_PATTERN = 9;
    protected const int RIGHT_DIAGONAL_PATTERN = 7;

    public abstract IEnumerable<Square> GetMoveSquares();

    public abstract IEnumerable<Square> GetAttackSquares();

    protected IEnumerable<Square> PatternToSquares(int[] pattern, Square position, bool rightShift = true, bool leftShift = true)
    {
        Square[] result = [];

        foreach (int i in pattern) 
        {
            ulong target = (ulong)position >> i;
            if (target >= (ulong)Square.a1 && rightShift)
            {
                AppendValidSquare(ref result, target);
            }

            target = (ulong)position << i;
            if (target <= (ulong)Square.g8 && leftShift)
            {
                AppendValidSquare(ref result, target);
            }
        }

        return result;
    }

    private bool AppendValidSquare(ref Square[] result, ulong target)
    {
        bool lineContinues = true;
        // 1, check if there is a piece there
        ulong targetSquare = State.Squares ^ target;
        if (targetSquare == 0) // there is a piece
        {
            // 2. we are stoping the line
            lineContinues = false;

            // 3. check if its same colour
            foreach (Piece p in State.Pieces)
            {
                if (p.Current == (Square)target && State.Turn != p.Colour)
                {
                    result.Append((Square)target);
                    break;
                }
            }
        }
        else
        {
            result.Append((Square)target);
        }

        return lineContinues;
    }

    private bool AppendAttackSquare(ref Square[] result, ulong target)
    {
        bool lineContinues = true;
        // 1, check if there is a piece there
        ulong targetSquare = State.Squares ^ target;
        if (targetSquare == 0) // there is a piece
        {
            // 2. append and stop line
            result.Append((Square)target);
            lineContinues = false;
        }
        else
        {
            result.Append((Square)target);
        }

        return lineContinues;
    }

    private void GetLine(ref Square[] result, ulong current, int pattern, bool leftShift, bool attack)
    {
        Func<ulong, int, ulong> shift = (i, p) =>
        {
            if (leftShift) return i << p;
            else return i >> p;
        };

        for (ulong i = current; i <= (ulong)Square.g8; i = shift(i, pattern))
        {
            bool lineContinues = attack 
                ? AppendAttackSquare(ref result, i)
                : AppendValidSquare(ref result, i);

            if (!lineContinues) break;
        }
    }
    protected bool SquaresFilter(ulong current, int pattern, ulong i)
    {
        return i == current << pattern
            || i == current >> pattern;
    }

    protected IEnumerable<Square> LegalRank((ulong, ulong, ulong) p, bool attack = false)
    {
        (ulong current, ulong rankfloor, ulong rankCeiling) = p;
        Square[] result = [];

        GetLine(ref result, current, RANK_PATTERN, true, attack);
        GetLine(ref result, current, RANK_PATTERN, false, attack);
        
        return result;
    }

    protected IEnumerable<Square> LegalFile(ulong current, bool attack = false)
    {
        Square[] result = [];

        GetLine(ref result, current, FILE_PATTERN, true, attack);
        GetLine(ref result, current, FILE_PATTERN, false, attack);

        return result;
    }

    protected IEnumerable<Square> LegalDiagonals(ulong current, bool attack = false)
    {
        Square[] result = [];

        GetLine(ref result, current, LEFT_DIAGONAL_PATTERN, true, attack);
        GetLine(ref result, current, LEFT_DIAGONAL_PATTERN, false, attack);
        GetLine(ref result, current, RIGHT_DIAGONAL_PATTERN, true, attack);
        GetLine(ref result, current, RIGHT_DIAGONAL_PATTERN, false, attack);

        return result;
    }

    protected (ulong, ulong) GetRankRange(ulong current)
    {
        return current switch
        {
            <= (ulong)Square.h1 => ((ulong)Square.a1, (ulong)Square.h1),
            <= (ulong)Square.h2 => ((ulong)Square.a2, (ulong)Square.h2),
            <= (ulong)Square.h3 => ((ulong)Square.a3, (ulong)Square.h3),
            <= (ulong)Square.h4 => ((ulong)Square.a4, (ulong)Square.h4),
            <= (ulong)Square.h5 => ((ulong)Square.a5, (ulong)Square.h5),
            <= (ulong)Square.h6 => ((ulong)Square.a6, (ulong)Square.h6),
            <= (ulong)Square.h7 => ((ulong)Square.a7, (ulong)Square.h7),
            <= (ulong)Square.h8 => ((ulong)Square.a8, (ulong)Square.h8),

            _ => throw new Exception()
        };
    }
}
