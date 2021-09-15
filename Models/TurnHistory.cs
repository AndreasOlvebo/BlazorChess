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

        public Board AddTurn(Board boardState, Piece movedPiece, Position originalPosition, Position newPosition, Piece defeatedPiece = null)
        {
            Turn turn = new()
            {
                BoardState = boardState,
                MovedPiece = movedPiece?.Clone(boardState),
                OriginalPosition = originalPosition,
                NewPosition = newPosition,
                DefeatedPiece = defeatedPiece?.Clone(boardState)
            };
            Turns.Add(turn);
            return boardState.Clone();
        }

        public Turn LastTurn()
        {
            return Turns.LastOrDefault();
        }

        public Turn UndoTurn()
        {
            //Console.WriteLine($"Undoing turn {Turns.Count}");
            if (Turns.Count > 1)
            {
                Turns.Remove(LastTurn()); 
            }
            else
            {
                Turn firstTurn = Turns.FirstOrDefault();
                Console.WriteLine($"Turn count {Turns.Count}, Original position: {firstTurn.OriginalPosition}, BoardId: {firstTurn.BoardState.BoardId}");
                return Turns.FirstOrDefault();
            }
            //Console.WriteLine($"Undid turn. Now at {Turns.Count}");
            return LastTurn();
        }
    }
}
