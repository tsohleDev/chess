global using ChessEngine.Enums;
using ChessEngine.Rules;

namespace ChessEngine;

public class Board
{

    AlgebraicNotation _score;
    ChessRules _rules;
    public Board(AlgebraicNotation Score, ChessRules Rules)
    {
        _score = Score;
        _rules = Rules;

        var state = Score.States[Score.States.Length - 1];
        Squares = state.Squares;
        Pieces = state.Pieces;
        Turn = state.Turn;
    }

    public ulong Squares { get; set; }
    public Player Turn { get; set; }
    public Piece[] Pieces { get; set; }

    public void Move(Player player, Piece from, Square to)
    {        

        if (player != Turn) { return; }
        
        CaptureSquare(from, to);
        RemovePiece(from);
        PutPiece(from, to);

        // is it check


        // is it checkmate

        // opponent's turn
        Player nextTurn = (Player)((int)Turn * -1);
        GameState newState = new GameState(Squares, Pieces, nextTurn);
       


        // append algebraic notation
        _score.AddState(newState, from.Current, to);
    }

    private void PutPiece(Piece piece, Square position)
    {
        Squares |= (ulong)position; // update to at squares
        piece.Current = position;
    }

    private void CaptureSquare(Piece from, Square to)
    {
        ulong x = Squares & (ulong)to;

        // To Square is not empty
        if (x > 0)
        {
            Piece toPiece = default;

            for (int i = 0; i < Pieces.Length; i++)
            {
                if (Pieces[i].Current == to)
                {
                    toPiece = Pieces[i];
                }
            }

            if (!_rules.IsMoveValid(from, toPiece))
            {
                throw new ApplicationException(Errors.Messages[3]);
            }

            RemovePiece(toPiece);
        }

    }

    private void RemovePiece(Piece piece)
    {
        for (int i = 0; i < Pieces.Length; i++)
        {
            if (Pieces[i] == piece)
            {
                Pieces[i] = default;
            }
        }

        Squares ^= (ulong)piece.Current; // update to at squares
    }
}