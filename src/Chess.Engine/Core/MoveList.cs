using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine.Core;

// 'ref struct' ensures this can only live on the stack. No heap allocations!
public ref struct MoveList
{
    private Span<Move> _moves;
    public int Count { get; private set; }

    public MoveList(Span<Move> buffer)
    {
        _moves = buffer;
        Count = 0;
    }

    public void Add(Move move)
    {
        _moves[Count++] = move;
    }

    public ReadOnlySpan<Move> AsSpan() => _moves.Slice(0, Count);
}