using BlazorChess.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Models
{
    public class TurnHistory
    {
        public List<Turn> Turns { get; private set; }
        public int CurrentTurn { get; set; } = 0; //TODO: Set current turn and add or subtract when adding or going back in list. Also remove up to certain point when first undone and then make new move.
        public TurnHistory()
        {
            Turns = new List<Turn>();
        }
        public Board AddTurnAndReturnBoardState(Board boardState, Piece movedPiece, Position originalPosition, Position newPosition, Piece defeatedPiece = null)
        {
            CurrentTurn += 1;
            Turn turn = new()
            {
                BoardState = boardState,
                MovedPiece = movedPiece,
                OriginalPosition = originalPosition,
                NewPosition = newPosition,
                DefeatedPiece = defeatedPiece
            };
            Turns.Add(turn);
            return boardState.Clone();
        }

        public Board AddTurnAndReturnBoardState(Turn turn)
        {
            CurrentTurn += 1;
            if (Turns.Count > CurrentTurn)
            {
                Console.WriteLine($"Turns.Count ({Turns.Count}) >= CurrentTurn ({CurrentTurn})");
                Turns.RemoveRange(CurrentTurn, Turns.Count - (CurrentTurn + 1)); 
            }
            Turns.Add(turn);
            return turn.BoardState.Clone();
        }

        public Turn LastTurn()
        {
            return Turns.ElementAtOrDefault(CurrentTurn);
        }

        public Turn UndoTurn()
        {
            if (Turns.Count > 1)
            {
                //Turns.Remove(LastTurn());
                CurrentTurn -= 1;
            }
            else
            {
                return Turns.FirstOrDefault();
            }

            return LastTurn();
        }

        public Turn RedoTurn()
        {
            if (Turns.Count >= CurrentTurn + 1)
            {
                CurrentTurn += 1;
                return Turns.ElementAtOrDefault(CurrentTurn);
            }
            return LastTurn();
        }
    }
}
