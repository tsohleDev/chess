using System.Collections;

namespace ChessEngine.Rules;

public partial class ChessRules(GameState State) : IChessRules
{
   /*
        * if same colour capture
        * if outside board
        * if same square 
        * if king capture
        * if move makes check *
        * if check must protect the king from threat *
        * if checkmate *
        * if valid castling *
        * if draw *
        * if stale mate *
        * if valid empersand 
        * if no path is clear
    */

    public bool IsMoveValid(Piece from, Piece to)
    {
        return IsSameColour(from, to)
            && InsideBoard(to)
            && IsSameSquare(from, to)
            && !IsKingCapture(from, to);
    }

    public bool IsCheckMate()
    {
        throw new NotImplementedException();
    }

    public bool IsDraw()
    {
        throw new NotImplementedException();
    }

    public static bool IsSameColour(Piece from, Piece to)
    {
        return from.Colour == to.Colour;
    }

    public static bool InsideBoard(Piece to)
    {
        return (int)to.Current >= 0 || (int)to.Current <= 63;
    }

    public static bool IsSameSquare(Piece from, Piece to)
    {
        return from.Current == to.Current;
    }

    public static bool IsKingCapture(Piece from, Piece to)
    {
        return to.Type == PieceType.King;
    }


    // checking if its check helper
    public IEnumerable<Piece> AttackProfile(Player player)
    {
        IEnumerable<Piece> pieces = [];
        Player opponent = player == Player.WHITE ? Player.BLACK : Player.WHITE;

        var playerPieces = State.ActivePieces.Where(p => p.Colour == player);
        var opponentPieces = State.ActivePieces.Where(p => p.Colour == opponent);

        List<Square> opponentAttack = new List<Square>();
        foreach (var piece in opponentPieces)
        {
            opponentAttack.Union(AvailableMoves(piece));
        }
        return pieces;
    }

    public IEnumerable<Square> AvailableMoves(Piece piece)
    {
        IEnumerable<Square> openSquares = new List<Square>();

        int[] movePattern = piece.Type switch
        {
            PieceType.King => [-9, -8, -7, -1, 1, 7, 8, 9],
            PieceType.Knight => [-13, -15, -10, -6, 6, 10, 13, 15],
            PieceType.Rook => RookStraights(piece),
            PieceType.Bishop => BishopDiaogonals(piece),
            PieceType.Queen => RookStraights(piece).Union(BishopDiaogonals(piece)).ToArray(),
            PieceType.PAWN => PawnWalk(piece, State.Squares),
            _ => throw new NotImplementedException()
        };

        
        foreach (var square in movePattern)
        {
            int possibleMove = (int)piece.Current + square;
            if (possibleMove < 0 || possibleMove > 63) continue;
            // if self check or in check
            if (!State.Squares[possibleMove]) openSquares.Append((Square)possibleMove);
        }

        return openSquares;
    }

    private int[] BishopDiaogonals(Piece piece)
    {
        int position = (int)piece.Current;

        /* Get diagonal line */
        int[] leftDiagonal = Enumerable.Range(0, 63)
                        .Where(i => i == position - 9
                                 && i == position + 9)
                        .Order()
                        .ToArray();
        // end


        /* Method Get Diagonal Path including two pieces at the end*/
        int i = 1, j = 1, k = leftDiagonal.IndexOf(position);
        bool continueI = true, continueJ = false;
        while(true)
        {
            if (!continueI && !continueJ) break;

            if (continueI && k - i >= 0 && State.Squares[leftDiagonal[k - i]])
                continueI = false;
            else i--;

            if (continueJ && k + j < leftDiagonal.Length && State.Squares[k + j])
                continueJ = false;
            else j++;
        }
        //end

        /* Filter Diagonal ends */
        leftDiagonal = leftDiagonal.Where((v, k) => k >= k - i && k - i >= 0
                                    && k <= k + j && k + j < leftDiagonal.Length)
                                    .ToArray();

        if (State.Squares[leftDiagonal[0]] == true)
        {
            leftDiagonal = leftDiagonal.Where((i, idx) => idx != 0)
                        .ToArray();
        }

        int endIndex = leftDiagonal.Length - 1;
        bool playerPieceInPath = State.ActivePieces.Any(p => p.Current == (Square)leftDiagonal[endIndex] && p.Colour == State.Turn);
        if (State.Squares[leftDiagonal[endIndex]] == true && !playerPieceInPath)
        {
            leftDiagonal = leftDiagonal.Where((i, idx) => idx != endIndex)
                        .ToArray();
        }
        // end

        return leftDiagonal;
    }

    private static int[] RookStraights(Piece piece)
    {
        Func<int, bool> fileFilterClause = (int i)
            => i == (int)piece.Current - 8
            && i == (int)piece.Current + 8;

        Func<int, bool> rankFilterClause = (int i)
            => i == (int)piece.Current - 1
            && i == (int)piece.Current + 1;


        int[] file = Enumerable.Range(0, 63)
                        .Where(i => fileFilterClause(i))
                        .ToArray();

        var pieceRank = GetRankRange(piece);

        int[] rank = Enumerable.Range(pieceRank.Item1, pieceRank.Item2+1)
                        .Where(i => rankFilterClause(i))
                        .ToArray();

        return rank.Union(file).ToArray();
    }

    private static (int, int) GetRankRange(Piece piece)
        {
            return  (int)piece.Current switch
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
    private static int[] PawnWalk(Piece piece, BitArray squares)
        {

            int[] moveAndTakes = [8, 9, 7];
            int colourDirection = (int)piece.Colour;
            var pieceRank = GetRankRange(piece);

            int secondRank = piece.Colour == Player.WHITE
                            ? 8
                            : 48;

            int fifthRank = piece.Colour == Player.WHITE
                            ? 32
                            : 24;

            moveAndTakes.Select(x => x * colourDirection);


            // First Move
            if (pieceRank.Item1 == secondRank) moveAndTakes.Append(16 * colourDirection);

            // En Passant
            if (pieceRank.Item1 == fifthRank)
            { 
                int rightHorizontal = ((int)piece.Current + 1);
                int leftHorizontal = (int)piece.Current - 1;
                
                if (squares[rightHorizontal] == true)
                {
                    moveAndTakes.Append(9 * (int)piece.Colour);
                }

                if (squares[leftHorizontal] == true)
                {
                    moveAndTakes.Append(7 * (int)piece.Colour);
                }
            }

            return moveAndTakes;
        }

}
