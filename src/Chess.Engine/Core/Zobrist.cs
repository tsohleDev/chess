namespace ChessEngine.Core
{
    internal static class Zobrist
    {
        public static ulong[,,] PieceKeys { get; private set; } = new ulong[6, 2, 64];
        public static ulong[] CastlingRightKeys { get; } = new ulong[16];
        public static ulong[] EnPassantFileKeys { get; } = new ulong[8];
        public static ulong[] TurnKeys { get; } = new ulong[2];

        static Zobrist()
        {
            var random = new Random();

            for (var i = 0; i < 6; i++)
            {
                for(var j = 0; j < 2; j++)
                {
                    for (var k = 0; k < 64; k++)
                    {
                        PieceKeys[i, j, k] = NextRandomUlong(random);
                    }
                }
            }

            for (var i = 0; i < 16; i++)
            {
                CastlingRightKeys[i] = NextRandomUlong(random);
            }

            for (var i = 0; i < 8; i++)
            {
                EnPassantFileKeys[i] = NextRandomUlong(random);
            }

            for (var i = 0; i < 2 ; i++)
            {
                TurnKeys[i] = NextRandomUlong(random);
            }    
        }

        internal static ulong HashBoard(Board board)
        {
            ulong hash = 0;
            for (var i = 0; i < 6; i++)
            {
                var mutating_bitboard = board.Pieces[i];
                while (mutating_bitboard != 0)
                {
                    ulong idx = Bitboard.PopLSB(ref mutating_bitboard);
                    int colour = (idx & board.Colors[0]) == 0 ? 1 : 0;
                    int idxNormalized = Bitboard.Normalize(idx);

                    hash ^= PieceKeys[i, colour, idxNormalized];
                }
            }

            hash ^= CastlingRightKeys[(int)board.Castling];

            hash ^= TurnKeys[(int)board.Turn];

            hash ^= EnPassantFileKeys[(int)board.EnPassantFile];
            return hash;
        }

        private static ulong NextRandomUlong(Random random)
        {
            Span<byte> buffer = stackalloc byte[8];
            random.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer);
        }
    }
}
