using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine.Rules
{
    internal interface IChessRules
    {
        public bool IsMoveValid(Piece from, Piece to);
        public bool IsCheckMate();
        public bool IsDraw();
    }
}
