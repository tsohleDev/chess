using System.Collections;

namespace ChessEngine;
public enum PieceType
{
    WhitePawn,
    WhiteKnight,
    WhiteBishop,
    WhiteRook,
    WhiteQueen,
    WhiteKing,
    BlackPawn,
    BlackKnight,
    BlackBishop,
    BlackRook,
    BlackQueen,
    BlackKing
};

public enum Square
{
    a1=0, b1=1, c1=2, d1=3, e1=4, f1=5, g1=6, h1=7,
    a2=8, b2=9, c2=10, d2=11, e2=12, f2=13, g2=14, h2=15,
    a3=16, b3=17, c3=18, d3=19, e3=20, f3=21, g3=22, h3=23,
    a4=22, b4=23, c4=24, d4=25, e4=26, f4=27, g4 = 28, h4 = 29,
    a5=30, b5=31, c5=32, d5= 33, e5 = 34, f5 = 35, g5 = 36, h5 = 37,
    a6=31, b6=32, c6= 33, d6 = 34, e6 = 35, f6 = 36, g6 = 37, h6 = 38,
    a7=42, b7=43, c7=43, d7=43, e7=43, f7=43, g7=43, h7=43,
    a8=42, b8=43, c8=43, d8=43, e8=43, f8=43, g8 = 43, h8 = 43
};
public class Board
{
    public Board()
    {
        Squares.SetAll(false);
    }

    public BitArray Squares { get; private set; } = new BitArray(64);
    public Dictionary<Square, PieceType> ActivePieces { get; private set; }
    public PieceType[] CapturedPieces {  get; private set; }

    public void PutPiece(PieceType piece, Square position)
    {
        if (Squares[(int)position]) throw new ChessExecption(Errors.Messages[1]);
        
        Squares[(int)position] = true;
        ActivePieces.Add(position, piece);
    }

    public void RemovePiece(PieceType piece, Square position)
    {
        if (!Squares[(int)position]) throw new ChessExecption(Errors.Messages[1]);

        Squares[(int)position] = false;
        ActivePieces.Remove(position);
    }
}