using BlazorChess.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Models
{
    public class TurnHistory
    {
        public List<Turn> Turns { get; set; }
        public TurnHistory()
        {
            Turns = new List<Turn>();
        }

        public void AddTurn(Board boardState, Piece movedPiece, Position originalPosition, Position newPosition, Piece defeatedPiece = null)
        {
            Turn turn = new()
            {
                BoardState = boardState.Clone(),
                MovedPiece = movedPiece.Clone(),
                OriginalPosition = originalPosition,
                NewPosition = newPosition,
                DefeatedPiece = defeatedPiece?.Clone()
            };
            Turns.Add(turn);
        }

        public Turn LastTurn()
        {
            return Turns.LastOrDefault();
        }
    }
}
