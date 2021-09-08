using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Models
{
    public class Turn
    {
        public Board BoardState { get; set; }
        public Piece MovedPiece { get; set; }
        public Position OriginalPosition { get; set; }
        public Position NewPosition { get; set; }
        public Piece DefeatedPiece { get; set; } = null;
    }
}
