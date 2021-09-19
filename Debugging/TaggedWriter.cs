using BlazorChess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Debugging
{
    public static class TaggedWriter
    {
        public static void WriteLine(string input, Piece piece = null, Position pos = null)
        {
            if (piece is not null && piece.Tracked && (pos is null || pos.Equals(new Position(7,4))))
            {
                Console.WriteLine(input);
            }
        }
    }
}
