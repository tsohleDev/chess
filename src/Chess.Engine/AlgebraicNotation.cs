using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine;

public class AlgebraicNotation
{
    public AlgebraicNotation()
    {
        States = [];
        Froms = [];
        TOs = [];
    }

    public void AddState(GameState state, Square from, Square to)
    {
        States.Append(state);
        Froms.Append(from);
        TOs.Append(to);
    }

    public GameState[] States { get; set; }
    public Square[] Froms { get; set; }
    public Square[] TOs { get; set; }
}
