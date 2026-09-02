namespace ChessEngine;
public class PiecesFactory
{
    public PiecesFactory() 
    {
        Pieces = [];

        PieceType[] backRank =
            [PieceType.ROOK,
            PieceType.KNIGHT,
            PieceType.BISHOP,
            PieceType.QUEEN,
            PieceType.KING,
            PieceType.BISHOP,
            PieceType.KNIGHT,
            PieceType.ROOK
        ];


        for (int i = 0; i< 8; i++)
        {
            Piece userpiece = new Piece((Square)(1 << i), Player.WHITE, backRank[i]);
            Pieces.Add(userpiece);

            Piece userPawn = new Piece((Square)(256 << i), Player.WHITE, PieceType.PAWN);
            Pieces.Add(userPawn);

            Piece opponentpiece = new Piece((Square)(72057594037927936 << i), Player.BLACK, backRank[i]);
            Pieces.Add(opponentpiece);

            Piece opponentPawn = new Piece((Square)(281474976710656 << i), Player.BLACK, PieceType.PAWN);
            Pieces.Add(opponentPawn);
        }
    }

    public List<Piece> Pieces { get; set; }
}
