using System.Runtime.CompilerServices;

namespace ChessEngine.Core;

public static class Bitboard
{
    public const ulong FileA = 0x0101010101010101;
    public const ulong FileB = 0x0202020202020202;
    public const ulong FileC = 0x0303030303030303;
    public const ulong FileD = 0x0404040404040404;
    public const ulong FileE = 0x0505050505050505;
    public const ulong FileF = 0x0606060606060606;
    public const ulong FileG = 0x0707070707070707;
    public const ulong FileH = 0x8080808080808080;
    public const ulong Rank1 = 0x00000000000000FF;
    public const ulong Rank2 = 0x000000000000FF00;
    public const ulong Rank3 = 0x0000000000FF0000;
    public const ulong Rank4 = 0x00000000FF000000;
    public const ulong Rank5 = 0x000000FF00000000;
    public const ulong Rank6 = 0x0000FF0000000000;
    public const ulong Rank7 = 0x00FF000000000000;
    public const ulong Rank8 = 0xFF00000000000000;
    public const ulong WholeBoard = 0xFFFFFFFFFFFFFFFF;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong SetBit(ulong bitboard, Square square) => bitboard | (ulong)square;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ClearBit(ulong bitboard, Square square) => bitboard & ~(ulong)square;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBitSet(ulong bitboard, Square square) => (bitboard & (ulong)square) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetLSB(ulong bitboard) => 1UL << System.Numerics.BitOperations.TrailingZeroCount(bitboard);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Normalize(ulong bitboard) => System.Numerics.BitOperations.TrailingZeroCount(bitboard);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SquareToFile(Square square) {
        if (square == Square.NONE) return -1;

        return (int)(GetLSB((ulong)square) % 8);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Square LeftShift(Square s, int i) => (Square)((ulong)s << i);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Square RightShift(Square s, int i) => (Square)((ulong)s >> i);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong PopLSB(ref ulong bitboard)
    {
        ulong lsb = GetLSB(bitboard);
        bitboard &= bitboard - 1;
        return lsb;
    }
}