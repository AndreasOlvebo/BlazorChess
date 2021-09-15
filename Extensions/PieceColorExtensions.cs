using BlazorChess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Extensions
{
    public static class PieceColorExtensions
    {
        public static PieceColor NextColor(this PieceColor tileColor)
        {
            return (tileColor == PieceColor.Black ? PieceColor.White : PieceColor.Black);
        }
    }
}
