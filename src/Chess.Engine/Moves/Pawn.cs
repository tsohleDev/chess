using ChessEngine.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine.Moves;

internal class Pawn(GameState State, AlgebraicNotation Score, Piece Piece) : Moves(State)
{
    public override IEnumerable<Square> GetMoveSquares()
    {
        int[] pattern = [8];

        DoubleJump(ref pattern);
        EnPassant( ref pattern);
        Takes(ref pattern);

        return PatternToSquares(pattern, Piece.Current, Piece.Colour == Player.BLACK, Piece.Colour == Player.WHITE);
    }

    public override IEnumerable<Square> GetAttackSquares()
    {
        int[] pattern = [];

        Takes(ref pattern);

        return PatternToSquares(pattern, Piece.Current, Piece.Colour == Player.BLACK, Piece.Colour == Player.WHITE);
    }

    protected void DoubleJump(ref int[] result)
    {
        int direction = (int)Piece.Colour;
        int current = (int)Piece.Current;

        Square[] homeRank = Piece.Colour == Player.WHITE ?
            [Square.a2, Square.b2, Square.c2, Square.d2, Square.e2, Square.f2, Square.h2, Square.g2]
            : [Square.a2, Square.b2, Square.c2, Square.d2, Square.e2, Square.f2, Square.h2, Square.g2];

        bool firstMove = false;
        bool nextSquareEmpty = false;
        foreach (Square s in homeRank)
        {
            if (s == Piece.Current)
            {
                firstMove = true;

                Func<ulong, ulong> nextPiece = c => Piece.Colour == Player.WHITE ?
                    c << 8 : c >> 8;


                nextSquareEmpty = (State.Squares ^ nextPiece((ulong)Piece.Current)) > 0;
            }
        }

        if (firstMove && nextSquareEmpty) 
        { 
            result.Append(16); 
        }
    }
    protected void EnPassant(ref int[] result)
    {
        ulong current = (ulong)Piece.Current;
        int direction = (int)Piece.Colour;
        int[] leftAndRight = [-1, 1];

        foreach (int i in leftAndRight)
        {
            ulong adjecentSquare = i == -1 ? current >> 1 : current << 1;
            bool adjecentOccupied = (State.Squares ^ adjecentSquare) > 0;

            if (!adjecentOccupied) continue;

            ulong targetSquare = (ulong)Piece.Colour == 1
            ? (i == -1 ? current << 7 : current << 9)
            : (i == -1 ? current >> 9 : current >> 7);

            bool targetOccupied = (State.Squares ^ targetSquare) > 0;

            if (targetOccupied) continue;

            // last move should be a pawn to target
            int length = Score.States.Length;
            Square lastMove = Score.TOs
                              .ElementAt(length - 1);

            bool lastMoveIsAdjecent = (ulong)lastMove == adjecentSquare;

            bool adjecentIsPawn = false;
            foreach (Piece p in State.Pieces)
            {
                if ((ulong)p.Current == adjecentSquare
                    && p.Type == PieceType.PAWN
                    && (int)p.Colour == direction * -1)
                {
                    adjecentIsPawn = true;
                }
            }


            if (!lastMoveIsAdjecent || !adjecentIsPawn) continue;

            result.Append((int)(targetSquare - current));
        }
    }
    protected void Takes(ref int[] result)
    {
        ulong current = (ulong)Piece.Current;
        int[] takesPositons = [9, 7];
        int direction = (int)Piece.Colour;

        foreach (int i in takesPositons)
        {
            ulong nextSquare = Piece.Colour == Player.WHITE
               ? current << i
                : current >> i;

            bool nextOccupied = (State.Squares ^ nextSquare) > 0;
            if (!nextOccupied) continue;


            bool oppositePlayerPiece = false;
            foreach (Piece p in State.Pieces)
            {
                if ((ulong)p.Current == nextSquare
                        && (int)p.Colour == direction * -1)
                {
                    oppositePlayerPiece = true;
                }
            }

            if (oppositePlayerPiece) { result.Append((int)(nextSquare - current)); }
        }
    }
}
