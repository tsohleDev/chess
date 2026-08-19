using System;
using System.Collections.Generic;
using System.Text;

namespace ChessEngine
{
    internal class ChessExecption(String message) : Exception(message) { }


    public record Errors
    {
        public static readonly Dictionary<int, string> Messages = new () {{ 1, "Invalid Move" }};
    };
}
