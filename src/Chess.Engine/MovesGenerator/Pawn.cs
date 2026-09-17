using ChessEngine.Core;

namespace ChessEngine.MovesGenerator;

public static partial class MoveGenerator
{
    private static ulong WhiteRankFilter(bool crown) => crown
            ? ~(ulong)0
            : ~Bitboard.Rank8;

    private static ulong BlackRankFilter(bool crown) => crown
            ? ~(ulong)0
            : ~Bitboard.Rank1;

    private static ulong SinglePush(Player turn, ulong pawns, bool crown = false) => turn == Player.WHITE
        ? pawns << 8 & WhiteRankFilter(crown)
        : pawns >> 8 & BlackRankFilter(crown);
        
    private static ulong DoublePush(Player turn, ulong singlePush) => turn == Player.WHITE
            ? (singlePush & Bitboard.Rank3) << 8
            : (singlePush & Bitboard.Rank6) >> 8;

    private static ulong LeftAttack(Player turn, ulong pawns, bool crown = false) => turn == Player.WHITE
            ? (pawns << 7) & ~Bitboard.FileH & WhiteRankFilter(crown)
            : (pawns >> 9) & ~Bitboard.FileH & BlackRankFilter(crown);

    private static ulong RightAttack(Player turn, ulong pawns, bool crown = false) => turn == Player.WHITE
            ? (pawns << 9) & ~Bitboard.FileA & WhiteRankFilter(crown)
            : (pawns >> 7) & ~Bitboard.FileA & BlackRankFilter(crown);

   
    public static void GeneratePawnMoves(Board board, ref MoveList moves)
    {
        AddPushes(board, ref moves);
        AddCaptures(board, ref moves);
        AddEnPassant(board, ref moves);
        AddCrowning(board, ref moves);
    }

    public static ulong GetPawnAttacks(Square square, Player attackerColor)
    {
        ulong pawnMask = (ulong)square;
        ulong attacks;

        if (attackerColor == Player.WHITE)
        {
            attacks = ((pawnMask >> 7) & ~Bitboard.FileA) | ((pawnMask >> 9) & ~Bitboard.FileH);
        }
        else
        {
            attacks = ((pawnMask << 7) & ~Bitboard.FileH) | ((pawnMask << 9) & ~Bitboard.FileA);
        }

        return attacks;
    }

    private static void AddPushes(Board board, ref MoveList moves)
    {
        ulong pawns = board.Pieces[(int)PieceType.PAWN] & board.Colors[(int)board.Turn];
        int opponent = (int)board.Turn ^ 1;
        ulong empty = board.Empty;

        ulong singlePushes = SinglePush(board.Turn, pawns) & empty;
        ulong doublePushes = DoublePush(board.Turn, singlePushes) & empty;

        AddMoves(singlePushes, opponent, ref moves, SinglePush);
        AddMoves(doublePushes, opponent, ref moves, (p, u, _) => SinglePush(p, SinglePush(p, u)));
    }

    private static void AddCaptures(Board board, ref MoveList moves)
    {
        ulong pawns = board.Pieces[(int)PieceType.PAWN] & board.Colors[(int)board.Turn];
        int opponent = (int)board.Turn ^ 1;
        ulong enemies = board.Colors[(int)board.Turn ^ 1];

        ulong attacksLeft = LeftAttack(board.Turn, pawns) & enemies;
        AddMoves(attacksLeft, opponent, ref moves, RightAttack, board.GetPieceAt);

        ulong attacksRight = RightAttack(board.Turn, pawns) & enemies;
        AddMoves(attacksRight, opponent, ref moves, LeftAttack, board.GetPieceAt);
    }

    private static void AddEnPassant(Board board, ref MoveList moves)
    {
        ulong pawns = board.Pieces[(int)PieceType.PAWN] & board.Colors[(int)board.Turn];
        
        if (board.EnPassantFile == -1) return;

        ulong rankSquare = board.Turn == Player.WHITE ? (ulong)Square.a5 : (ulong)Square.a4;
        ulong rank = board.Turn == Player.WHITE ? Bitboard.Rank5 : Bitboard.Rank4;
        ulong square = rankSquare<< board.EnPassantFile;

        var target = board.Turn == Player.WHITE
            ? square << 8
            : square >> 8;


        if ((square >> 1 & pawns & rank) != 0) 
        {
            moves.Add(new Move((Square)(square >> 1), (Square)target, PieceType.PAWN, PieceType.PAWN, true));
        }
        
        if ((square << 1 & pawns & rank) != 0)
        {
            moves.Add(new Move((Square)(square << 1), (Square)target, PieceType.PAWN, PieceType.PAWN, true));
        }
    }
    
    private static void AddCrowning(Board board, ref MoveList moves)
    {
        int opponent = (int)board.Turn ^ 1;

        ulong crownContenders = board.Pieces[(int)PieceType.PAWN]
            & board.Colors[(int)board.Turn]
            & Bitboard.Rank7
            & Bitboard.Rank2;
        
        ulong empty = board.Empty;
        ulong enemies = board.Colors[opponent];

        var validPushCrowns = SinglePush(board.Turn, crownContenders, true) & empty;
        var validLeftCaptureCrowns = LeftAttack(board.Turn, crownContenders, true) & enemies;
        var validRightCaptureCrowns = RightAttack(board.Turn, crownContenders, true) & enemies;


        ReadOnlySpan<PieceType> crowns = [PieceType.BISHOP, PieceType.KNIGHT, PieceType.ROOK, PieceType.QUEEN];

        foreach (PieceType pieceType in crowns)
        {
            AddMoves(validPushCrowns, opponent, ref moves, SinglePush, null, pieceType);
            AddMoves(validLeftCaptureCrowns, opponent, ref moves, RightAttack, null, pieceType);
            AddMoves(validRightCaptureCrowns, opponent, ref moves, LeftAttack, null, pieceType);
        }
    }

    private static void AddMoves(ulong squares, int opponent, ref MoveList moves, Func<Player, ulong, bool, ulong> getOriginSquare, Func<Square, PieceType>? getPiece = null, PieceType crown= PieceType.NONE)
    {
        while (squares != 0)
        {
            var to = (Square)Bitboard.PopLSB(ref squares);
            var from = (Square)getOriginSquare((Player)opponent, (ulong)to, crown != PieceType.NONE);
            var moved = PieceType.PAWN;
            var captured = getPiece != null ? getPiece((Square)to) : PieceType.NONE;
            var castling = CastlingRights.NONE;

            moves.Add(new Move(from, to, moved, captured, false, castling, crown));

        }
    }
}
