using BlazorChess.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Models.PieceTypes
{
    public class Bishop : Piece
    {
        public Bishop(PieceColor pieceColor, Position piecePosition, Func<Position, Position, bool> collisionBetween) : base(pieceColor, piecePosition, collisionBetween)
        {
            MinRange = new Position(1, 1);
            MaxRange = new Position(0, 0);
        }

        public override MoveAllowed AllowMove(Position currentPosition, Position newPosition)
        {
            Position absoluteDifference = currentPosition.Difference(newPosition);

            if (absoluteDifference.X == absoluteDifference.Y)
            {
                return MoveAllowed.Yes;
            }

            return MoveAllowed.No;
        }

        public override Piece Clone()
        {
            return new Bishop(this.PieceColor, new Position(this.PiecePosition), this.CollisionBetween)
            {
                FirstMove = this.FirstMove,
                Defeated = this.Defeated                
            };
        }
    }
}
