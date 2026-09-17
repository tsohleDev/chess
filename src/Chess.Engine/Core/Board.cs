using System.Text.RegularExpressions;

namespace ChessEngine.Core;

public class Board
{
    public readonly ulong[] Pieces = new ulong[6];
    public readonly ulong[] Colors = new ulong[2];
    public (PieceType, Player)[] Captured = [];

    public Player Turn { get; private set; }
    public int EnPassantFile { get; private set; } = -1;
    public CastlingRights Castling { get; set; } = CastlingRights.NONE;
    public CastlingFlags CastlingFlag { get; set; } = CastlingFlags.NONE;
    public int HalfMoveClock { get; private set; } = -1;

    public ulong Occupancy => Colors[(byte)Player.WHITE] | Colors[(byte)Player.BLACK];
    public ulong Empty => ~Occupancy;

    public Board Clone() => (Board)this.MemberwiseClone();

    public Board()
    {
        LoadStandardPosition();
    }

    public void LoadStandardPosition()
    {
        Pieces[(int)PieceType.PAWN] = 0x00FF00000000FF00;
        Pieces[(int)PieceType.KNIGHT] = 0x4200000000000042;
        Pieces[(int)PieceType.BISHOP] = 0x2400000000000024;
        Pieces[(int)PieceType.ROOK] = 0x8100000000000081;
        Pieces[(int)PieceType.QUEEN] = 0x0800000000000008;
        Pieces[(int)PieceType.KING] = 0x1000000000000010;

        Colors[(int)Player.WHITE] = 0x000000000000FFFF;
        Colors[(int)Player.BLACK] = 0xFFFF000000000000;

        Turn = Player.WHITE;
    }

    public void MakeMove(Move move)
    {
        int color = (int)Turn;
        int oppColor = color ^ 1;

        HandleHalfClock(move.MovedPiece, move.CapturedPiece);

        Capture(move.CapturedPiece, move.IsEnPassant, move.To, (Player)oppColor);

        if (move.CastleType == CastlingRights.NONE 
            && move.CrownPiece == PieceType.NONE)
        {
            RemovePiece(move.MovedPiece, Turn, move.From);
            PutPiece(move.MovedPiece, Turn, move.To);
        }
       
        HandleEnPassant(move.IsEnPassant, move.To, Turn, (Player)oppColor);
        UpdateEnPassantTarget(move.MovedPiece, move.To, move.From);

        HandleCastling(move.CastleType);

        HandleCrowning(move.From, move.To, Turn, move.CapturedPiece, move.CrownPiece);

        AddCastlingRight();
        AddCastlingFlags(move.From);
        
        Turn = (Player)oppColor;
    }



    public void HandleCastling(CastlingRights castleType)
    {
        CastlingFlags kingFlag = Turn == Player.WHITE ? CastlingFlags.WHITE_KING_MOVED : CastlingFlags.BLACK_KING_MOVED;

        if (CastlingFlag.HasFlag(kingFlag)) throw new Exception("Castling Attempt failed, King has moved");

        switch (castleType)
        {
            case CastlingRights.WHITE_QUEEN_SIDE:
                CastleShufle(CastlingRights.WHITE_QUEEN_SIDE, (Square.e1, Square.c1, Square.a1, Square.d1));
                break;
            case CastlingRights.BLACK_QUEEN_SIDE:
                CastleShufle(CastlingRights.BLACK_QUEEN_SIDE, (Square.e8, Square.c8, Square.a8, Square.d8));
                break;
            case CastlingRights.WHITE_KING_SIDE:
                CastleShufle(CastlingRights.WHITE_KING_SIDE, (Square.e1, Square.g1, Square.h1, Square.f1));
                break;
            case CastlingRights.BLACK_KING_SIDE:
                CastleShufle(CastlingRights.BLACK_KING_SIDE, (Square.e8, Square.g8, Square.h8, Square.f8));
                break;
            default:
                throw new Exception("Invalid Castling");
        }
    }

    private void CastleShufle(CastlingRights right, (Square, Square, Square, Square) squares)
    {
        if (!Castling.HasFlag(right)) throw new Exception("Castling Attempt failed");

        var (kingFrom, kingTo, rookFrom, rookTo) = squares;

        RemovePiece(PieceType.KING, Turn, kingFrom);
        PutPiece(PieceType.KING, Turn, kingTo);

        RemovePiece(PieceType.ROOK, Turn, rookFrom);
        PutPiece(PieceType.ROOK, Turn, rookTo);
    }


    private void RemovePiece(PieceType piecetype, Player colour, Square from)
    {
        if (piecetype == PieceType.NONE) return;

        Pieces[(int)piecetype] = Bitboard.ClearBit(Pieces[(int)piecetype], from);
        Colors[(int)colour] = Bitboard.ClearBit(Colors[(int)colour], from);
    }

    private void PutPiece(PieceType piecetype, Player colour, Square to)
    {
        Pieces[(int)piecetype] = Bitboard.SetBit(Pieces[(int)piecetype], to);
        Colors[(int)colour] = Bitboard.SetBit(Colors[(int)colour], to);
    }


    private void AddCastlingRight()
    {
        (Square[], CastlingFlags, CastlingRights)[] tuples = [
            ([Square.b1, Square.c1, Square.d1], CastlingFlags.WHITE_QUEEN_SIDE_ROOK_MOVED, CastlingRights.WHITE_QUEEN_SIDE),
            ([Square.f1, Square.h1], CastlingFlags.WHITE_KING_SIDE_ROOK_MOVED, CastlingRights.WHITE_KING_SIDE),
            ([Square.b8, Square.c8, Square.d8], CastlingFlags.BLACK_QUEEN_SIDE_ROOK_MOVED, CastlingRights.BLACK_QUEEN_SIDE),
            ([Square.f8, Square.h8], CastlingFlags.BLACK_KING_SIDE_ROOK_MOVED, CastlingRights.BLACK_KING_SIDE)
            ];

        foreach (var (squares, flag, right) in tuples)
        {
            ulong path = 0;
            foreach (var s in squares)
            {
                path |= (ulong)s;
            }

            var kingFlag = Turn == Player.WHITE ? CastlingFlags.WHITE_KING_MOVED : CastlingFlags.BLACK_KING_MOVED;
            if ((Occupancy & path) == 0 && !CastlingFlag.HasFlag(flag) && !CastlingFlag.HasFlag(kingFlag))
            {
                Castling |= right;
            }
        }
    }

    private void AddCastlingFlags(Square from)
    {
        if (from == Square.e1)
        {
            Castling &= ~CastlingRights.WHITE_QUEEN_SIDE;
            Castling &= ~CastlingRights.WHITE_KING_SIDE;

            return;
        }
        else if (from == Square.e8)
        {
            Castling &= ~CastlingRights.BLACK_QUEEN_SIDE;
            Castling &= ~CastlingRights.BLACK_KING_SIDE;

            return;
        }


        (Square, CastlingFlags, CastlingRights)[] tuples = [
            (Square.e1, CastlingFlags.WHITE_KING_MOVED, CastlingRights.NONE),
            (Square.e8, CastlingFlags.BLACK_KING_MOVED, CastlingRights.NONE),
            (Square.a1, CastlingFlags.WHITE_QUEEN_SIDE_ROOK_MOVED, CastlingRights.WHITE_QUEEN_SIDE),
            (Square.g1, CastlingFlags.WHITE_KING_SIDE_ROOK_MOVED, CastlingRights.WHITE_KING_SIDE),
            (Square.a8, CastlingFlags.BLACK_QUEEN_SIDE_ROOK_MOVED, CastlingRights.BLACK_QUEEN_SIDE),
            (Square.g8, CastlingFlags.BLACK_KING_SIDE_ROOK_MOVED, CastlingRights.BLACK_KING_SIDE)
            ];

        foreach (var (square, flag, right) in tuples)
        {
            if (from == square && !CastlingFlag.HasFlag(flag))
            {
                CastlingFlag |= flag;
            
                if (Castling.HasFlag(right) && right != CastlingRights.NONE)
                {
                    Castling &= ~right;
                }  
            }
        }
    }
 
    private void Capture(PieceType captured, bool isEnPassant, Square to, Player opponent)
    {
        if (captured == PieceType.NONE || isEnPassant) return;


        Pieces[(int)captured] = Bitboard.ClearBit(Pieces[(int)captured], to);
        Colors[(int)opponent] = Bitboard.ClearBit(Colors[(int)opponent], to);

        Captured.Append((captured, opponent));
    }


    private void HandleEnPassant(bool isEnPassant, Square to, Player color, Player opponent)
    {
        if (!isEnPassant) return;
        
        Square captureSquare = color == (byte)Player.WHITE ? Bitboard.RightShift(to, 8) : Bitboard.LeftShift(to, 8);

        RemovePiece(PieceType.PAWN, opponent, captureSquare);

        Captured.Append((PieceType.PAWN, opponent));
    }

    private void UpdateEnPassantTarget(PieceType moved, Square to, Square from)
    {
        if (moved == PieceType.PAWN && ((ulong)to >> 16 == (ulong)from || (ulong)from >> 16 == (ulong)to))
        {
            EnPassantFile = Bitboard.SquareToFile(from);
        }
        else
        {
            EnPassantFile = -1;
        }
    }
    
    private void HandleHalfClock(PieceType moved, PieceType captured)
    {
        if (moved == PieceType.PAWN || captured != PieceType.NONE)
        {
            HalfMoveClock = 0;
        }
        else
        {
            HalfMoveClock++;
        }
    }



    private void HandleCrowning(Square from, Square to, Player color, PieceType captured, PieceType crown)
    {
        if (crown == PieceType.NONE) return;

        var opponent = (Player)((int)color ^ 1);

        RemovePiece(captured, opponent, to);
        
        RemovePiece(PieceType.PAWN, color, from);
        PutPiece(crown, color, to);
    }



    public PieceType GetPieceAt(Square square)
    {
        for (int i = 0; i < 6; i++)
        {
            if (Bitboard.IsBitSet(Pieces[i], square))
                return (PieceType)i;
        }

        return PieceType.NONE;
    }
}