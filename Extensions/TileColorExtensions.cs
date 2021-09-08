using BlazorChess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Extensions
{
    public static class TileColorExtensions
    {
        public static TileColor NextColor(this TileColor tileColor)
        {
           return (tileColor == TileColor.Black ? TileColor.White : TileColor.Black);       
        }
    }
}
