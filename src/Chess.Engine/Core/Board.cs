

using System.Drawing;

namespace ChessEngine.Core;

public class Board
{
    public readonly ulong[] Pieces = new ulong[6];
    public readonly ulong[] Colors = new ulong[2];

    public Player Turn { get; private set; }

    // Check and Draw and state mate

    public Square EnPassantSquare { get; private set; } = Square.NONE;
    public int HalfMoveClock { get; private set; } = -1;

    // Derived properties for easy access
    public ulong Occupancy => Colors[(int)Player.WHITE] | Colors[(int)Player.BLACK];
    public ulong Empty => ~Occupancy;

    public Board()
    {
        LoadStandardPosition();
    }

    public void LoadStandardPosition()
    {
        // Hexadecimal representation of standard chess starting squares
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

    // Fast copy for move simulation (Check detection)
    public Board Clone() => (Board)this.MemberwiseClone();

    public void MakeMove(Move move)
    {
        int color = (int)Turn;
        int oppColor = color ^ 1;

        // Reset Half-Move clock for Pawn moves or Captures, otherwise increment
        if (move.MovedPiece == PieceType.PAWN || move.CapturedPiece != PieceType.NONE)
            HalfMoveClock = 0;
        else
            HalfMoveClock++;

        // 1. Move the piece
        Pieces[(int)move.MovedPiece] = Bitboard.ClearBit(Pieces[(int)move.MovedPiece], move.From);
        Colors[color] = Bitboard.ClearBit(Colors[color], move.From);

        Pieces[(int)move.MovedPiece] = Bitboard.SetBit(Pieces[(int)move.MovedPiece], move.To);
        Colors[color] = Bitboard.SetBit(Colors[color], move.To);

        // 2. Handle Standard Captures
        if (move.CapturedPiece != PieceType.NONE && !move.IsEnPassant)
        {
            Pieces[(int)move.CapturedPiece] = Bitboard.ClearBit(Pieces[(int)move.CapturedPiece], move.To);
            Colors[oppColor] = Bitboard.ClearBit(Colors[oppColor], move.To);
        }

        // 3. Handle En Passant Capture
        if (move.IsEnPassant)
        {
            // The captured pawn is on the same rank as the 'From' square, but the 'To' file
            Square captureSquare = color == (byte)Player.WHITE ? Bitboard.RightShift(move.To, 8) : Bitboard.LeftShift(move.To, 8);
            Pieces[(int)PieceType.PAWN] = Bitboard.ClearBit(Pieces[(int)PieceType.PAWN], captureSquare);
            Colors[oppColor] = Bitboard.ClearBit(Colors[oppColor], captureSquare);
        }

        // 4. Update En Passant Target Square for next turn
        if (move.MovedPiece == PieceType.PAWN && ((ulong)move.To >> 16 == (ulong)move.From || (ulong)move.From >> 16 == (ulong)move.To))
        {
            EnPassantSquare = color == (byte)Player.WHITE ? Bitboard.LeftShift(move.From, 8) : Bitboard.RightShift(move.From, 8);
        }
        else
        {
            EnPassantSquare = Square.NONE;
        }

        Turn = (Player)oppColor;
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