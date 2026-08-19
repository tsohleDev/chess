namespace ChessEngine;
internal class PiecesFactory
{
    public PiecesFactory(Player player) 
    {
        Pieces = [];

        Player opponent = player == Player.WHITE ? Player.BLACK : player;

        PieceType[] whitePieceOrder =
            [PieceType.Rook,
            PieceType.Knight,
            PieceType.Bishop,
            PieceType.Queen,
            PieceType.King,
            PieceType.Bishop,
            PieceType.Knight,
            PieceType.Rook
        ];

        PieceType[] blackPieceOrder =
           [PieceType.Rook,
            PieceType.Knight,
            PieceType.Bishop,
            PieceType.King,
            PieceType.Queen,
            PieceType.Bishop,
            PieceType.Knight,
            PieceType.Rook
       ];


        for (int i = 0; i< 8; i++)
        {
            PieceType pieceType = player == Player.WHITE ? whitePieceOrder[i] : blackPieceOrder[i];

            Piece userpiece = new Piece((Square)i, player, pieceType);
            Pieces.Add(userpiece);

            Piece userPawn = new Piece((Square)i + 8, player, PieceType.Pawn);
            Pieces.Add(userPawn);

            Piece opponentpiece = new Piece((Square)i + 56, opponent, pieceType);
            Pieces.Add(opponentpiece);

            Piece opponentPawn = new Piece((Square)i + 8, player, PieceType.Pawn);
            Pieces.Add(opponentPawn);
        }
    }

    List<Piece> Pieces { get; set; }
}
