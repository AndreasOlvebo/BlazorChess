using BlazorChess.Debugging;
using BlazorChess.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Models.PieceTypes
{
    public class Queen : Piece
    {
        public Queen(PieceColor pieceColor, Position piecePosition, Func<Position, Position, bool> collisionBetween) : base(pieceColor, piecePosition, collisionBetween)
        {
            
        }

        public Queen(Queen queen, Board board) : base(queen, board)
        {

        }

        public override MoveAllowed AllowMove(Position currentPosition, Position newPosition)
        {
            Position difference = currentPosition.Difference(newPosition);
            if (MoveIsStraight(difference))
            {
                //TaggedWriter.WriteLine($"move from {currentPosition} to {newPosition} with difference {difference} is allowed", this);
                return MoveAllowed.Yes;
            }
            //TaggedWriter.WriteLine($"move from {currentPosition} to {newPosition} with difference {difference} is NOT allowed", this);
            return MoveAllowed.No;
        }

        private bool MoveIsStraight(Position difference)
        {
            Position absoluteDifference = difference.Absolute();
            return (absoluteDifference.X == 0 || absoluteDifference.Y == 0 || absoluteDifference.X == absoluteDifference.Y);
        }

        public override Piece Clone(Board board)
        {
            return new Queen(this, board);
        }
    }
}
