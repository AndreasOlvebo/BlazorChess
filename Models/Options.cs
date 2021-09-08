using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Models
{
    public class Options
    {
        public bool EnableTurns { get; set; } = true;
        public bool HumanVsComputer { get; set; } = false;
        public PieceColor PlayerColor { get; set; } = PieceColor.White;

        public Options()
        {

        }
    }
}
