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

        public override MoveAllowed AllowMove(Position currentPosition, Position newPosition)
        {
            Position difference = currentPosition.Difference(newPosition);
            if (MoveIsStraight(difference))
            {
                return MoveAllowed.Yes;
            }
            return MoveAllowed.No;
        }

        private bool MoveIsStraight(Position difference)
        {
            Position absoluteDifference = difference.Absolute();
            return (absoluteDifference.X == 0 || absoluteDifference.Y == 0 || absoluteDifference.X == absoluteDifference.Y);
        }

        public override Piece Clone()
        {
            return new Queen(this.PieceColor, new Position(this.PiecePosition), this.CollisionBetween)
            {
                FirstMove = this.FirstMove,
                Defeated = this.Defeated
            };
        }
    }
}
