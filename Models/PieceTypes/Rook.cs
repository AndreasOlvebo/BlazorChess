using BlazorChess.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Models.PieceTypes
{
    public class Rook : Piece
    {
        public Rook(PieceColor pieceColor, Position piecePosition, Func<Position, Position, bool> collisionBetween) : base(pieceColor, piecePosition, collisionBetween)
        {
            MinRange = new Position(0, 0);
            MaxRange = new Position(0, 0);
        }

        public override MoveAllowed AllowMove(Position currentPosition, Position newPosition)
        {
            Position difference = newPosition.Subtract(currentPosition);
            difference = difference.Absolute(Axis.X);
            if (OnlyOneDirection(difference))
            {
                return MoveAllowed.Yes; 
            }
            return MoveAllowed.No;
        }

        private static bool OnlyOneDirection(Position difference)
        {
            return difference.X == 0 || difference.Y == 0;
        }

        public override Piece Clone()
        {
            return new Rook(this.PieceColor, new Position(this.PiecePosition), this.CollisionBetween)
            {
                FirstMove = this.FirstMove,
                Defeated = this.Defeated
            };
        }
    }
}
