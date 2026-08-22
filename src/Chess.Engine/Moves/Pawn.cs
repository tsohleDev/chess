using ChessEngine.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine.Moves
{
    internal class Pawn : IAvailableMoves
    {
        public override IEnumerable<Square> GetSquares(Piece piece)
        {
            int[] pattern = [8];

            GameState state = GameState.Instance;

            DoubleJump(piece, ref pattern);
            EnPassant(piece, ref pattern);
            Takes(piece, ref pattern);

            return PatternToSquares(pattern, piece.Current)
                
                .RemoveBadSquares();
        }

        protected void DoubleJump(Piece piece, ref int[] result)
        {
            GameState state = GameState.Instance;
            int direction = (int)piece.Colour;
            int current = (int)piece.Current;

            bool firstMove = state.AlgebraicNotation.Any(move => move.Item1 == piece.Current);
            bool nextSquareEmpty = state.Squares[current + 8 * direction];

            if (firstMove && nextSquareEmpty) { result.Append(16 * direction); }
        }

        protected void Takes(Piece piece, ref int[] result)
        {
            GameState state = GameState.Instance;
            int current = (int)piece.Current;
            int[] takesPositons = [9, 7];
            int direction = (int)piece.Colour;

            foreach (int i in takesPositons)
            {
                int nextSquare = current + i * direction;

                bool nextOccupied = state.Squares[nextSquare];
                if (!nextOccupied) continue;

                bool oppositePlayerPiece = state.ActivePieces.Any(p =>
                            (int)p.Current == nextSquare
                            && (int)p.Colour == direction * -1
                );

                if (oppositePlayerPiece) { result.Append(nextSquare - current); }
            }
        }


        protected void EnPassant(Piece piece, ref int[] result)
        {
            GameState state = GameState.Instance;
            int current = (int)piece.Current;
            int direction = (int)piece.Colour;
            int[] leftAndRight = [-1, 1];

            foreach (int i in leftAndRight)
            {
                int adjecentSquare = current + i;
                bool adjecentOccupied = state.Squares[adjecentSquare];

                if (!adjecentOccupied) continue;

                int targetSquare = (int)piece.Colour == 1
                ? (i == -1 ? current + 7 : current + 9)
                : (i == -1 ? current - 9 : current - 7);
                bool targetOccupied = state.Squares[targetSquare];

                if (targetOccupied) continue;

                // last move should be a pawn to target
                int length = state.AlgebraicNotation.Count;
                Square lastMove = state.AlgebraicNotation
                                           .ElementAt(length - 1)
                                           .Item2;

                bool lastMoveIsAdjecent = (int)lastMove == adjecentSquare;
                bool adjecentIsPawn = state.ActivePieces
                                        .Any(p => (int)p.Current == adjecentSquare
                                            && p.Type == PieceType.PAWN
                                            && (int)p.Colour == direction * -1);


                if (!lastMoveIsAdjecent || !adjecentIsPawn) continue;
  
                result.Append(targetSquare - current);
            }
        }
    }
}
