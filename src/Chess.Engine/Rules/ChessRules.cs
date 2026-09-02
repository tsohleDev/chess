using ChessEngine.Moves;

namespace ChessEngine.Rules;

public partial class ChessRules(GameState State, AlgebraicNotation Score) : IChessRules
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
            && !IsSameSquare(from, to)
            && !IsKingCapture(to);
    }

    public bool IsCheckMate(Square[] attackProfile)
    {
        Piece king = default;
        foreach (Piece p in State.Pieces)
        {
            if (p.Type == PieceType.KING
                 && p.Colour != State.Turn)
            {
                king = p;
                break;
            }
        }

        if (king == default) throw new Exception();

        IEnumerable<Square> kingMoves = MoveFactory.AttackProfile(State, king, Score);

        foreach (Square ks in kingMoves)
        {
            bool found = false;
            foreach (Square s in attackProfile)
            {
                if (s == ks)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsCheck(Square[] attackProfile)
    {
        // king is in attack profile of opponent pieces
        Square king = default;
        foreach(Piece p in State.Pieces)
        {
           if (p.Type == PieceType.KING
                && p.Colour != State.Turn)
           {
                king = p.Current;
                break;
           }
        }

        if (king == default) throw new Exception();

        foreach (Square s in attackProfile)
        {
           if (s == king)
           {  
                return true; 
           }
        }

        return false;
    }

    public bool IsDraw()
    {
        // repeatinmg moves

        // move count 

        // still mate ( function that checks if any move that exists that does not create check )

        throw new NotImplementedException();
    }

    public bool IsStaleMate(Square[] attackProfile)
    {
        // 1. get user valid move not attack
        Square[] squares = UserMoveProfile();

        // 2. check if each move causes check
        return false;

    }

    public Square[] OpponentAttackProfile()
    {
        Square[] squares = [];
        foreach(Piece p in State.Pieces)
        {
            if (p.Colour != State.Turn)
            {
                squares.Union(MoveFactory.AttackProfile(State, p, Score));
            }
        }

        return squares;
    }

    private Square[] UserMoveProfile()
    {
        Square[] squares = [];
        foreach (Piece p in State.Pieces)
        {
            if (p.Colour == State.Turn)
            {
                squares.Union(MoveFactory.AttackProfile(State, p, Score));
            }
        }

        return squares;
    }

    public static bool IsSameColour(Piece from, Piece to)
    {
        return from.Colour == to.Colour;
    }

    public static bool InsideBoard(Piece to)
    {
        return to.Current != Square.NONE;
    }

    public static bool IsSameSquare(Piece from, Piece to)
    {
        return from.Current == to.Current;
    }

    public static bool IsKingCapture(Piece to)
    {
        return to.Type == PieceType.KING;
    }
}
