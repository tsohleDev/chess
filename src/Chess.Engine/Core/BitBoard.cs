using ChessEngine.Moves;
using System.Runtime.CompilerServices;

namespace ChessEngine.Core;

public static class Bitboard
{
    public const ulong FileA = 0x0101010101010101;
    public const ulong FileH = 0x8080808080808080;
    public const ulong Rank1 = 0x00000000000000FF;
    public const ulong Rank2 = 0x000000000000FF00;
    public const ulong Rank3 = 0x0000000000FF0000;
    public const ulong Rank8 = 0xFF00000000000000;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong SetBit(ulong bitboard, Square square) => bitboard | (ulong)square;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ClearBit(ulong bitboard, Square square) => bitboard & ~(ulong)square;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBitSet(ulong bitboard, Square square) => (bitboard & (ulong)square) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetLSB(ulong bitboard) => 1UL << System.Numerics.BitOperations.TrailingZeroCount(bitboard);

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