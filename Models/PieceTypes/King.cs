using BlazorChess.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Models.PieceTypes
{
    public class King : Piece
    {
        public King(PieceColor pieceColor, Position piecePosition, Func<Position, Position, bool> collisionBetween) : base(pieceColor, piecePosition, collisionBetween)
        {
            MaxRange = new Position(1, 1);
        }

        public King(King king, Board board) : base(king, board)
        {
            
        }

        public override MoveAllowed AllowMove(Position currentPosition, Position newPosition)
        {
            Position absoluteDifference = currentPosition.Difference(newPosition);
            if (absoluteDifference.X == 1 || absoluteDifference.Y == 1)
            {
                return MoveAllowed.Yes;
            }

            return MoveAllowed.No;
        }

        public override Piece Clone(Board board)
        {
            return new King(this, board);
            //return new King(this.PieceColor, new Position(this.PiecePosition), this.CollisionBetween)
            //{
            //    FirstMove = this.FirstMove,
            //    Defeated = this.Defeated
            //};
        }
    }
}
