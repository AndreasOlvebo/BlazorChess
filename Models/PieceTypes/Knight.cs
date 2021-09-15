using BlazorChess.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Models.PieceTypes
{
    public class Knight : Piece
    {
        public Knight(PieceColor pieceColor, Position piecePosition, Func<Position, Position, bool> collisionBetween) : base(pieceColor, piecePosition, collisionBetween)
        {
            IgnoreCollision = true;
        }

        public Knight(Knight knight, Board board) : base(knight, board)
        {

        }
        public override MoveAllowed AllowMove(Position currentPosition, Position newPosition)
        {
            Position absoluteDifference = currentPosition.Difference(newPosition);
            Position movementPattern = new Position(1, 2);
            if (absoluteDifference.BothMatchesEither(movementPattern))
            {
                return MoveAllowed.Yes;
            }
            return MoveAllowed.No;
        }

        public override Piece Clone(Board board)
        {
            return new Knight(this, board);
        }
    }
}
