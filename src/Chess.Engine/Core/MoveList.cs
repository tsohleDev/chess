namespace ChessEngine.Core;

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

    public readonly ReadOnlySpan<Move> AsSpan() => _moves[0..Count];
}