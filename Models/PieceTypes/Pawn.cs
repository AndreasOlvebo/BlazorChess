using BlazorChess.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Models.PieceTypes
{
    public class Pawn : Piece
    {
        public Pawn(PieceColor pieceColor, Position piecePosition, Func<Position, Position, bool> collisionBetween) : base(pieceColor, piecePosition, collisionBetween)
        {
            MinRange = new Position(0, 1);
            MaxRange = new Position(1, 2);
        }
        public Pawn(Pawn pawn, Board board) : base(pawn, board)
        {
            
        }

        public override MoveAllowed AllowMove(Position currentPosition, Position newPosition)
        {
            Position difference = currentPosition.Difference(newPosition, Axis.X);
            if (difference.X == 0)
            {
                if (difference.Y == 2 && FirstMove)
                {
                    return MoveAllowed.IfEmpty;
                }
                if (difference.Y == 1)
                {
                    return MoveAllowed.IfEmpty;
                }
            }
            if (difference.X == 1 && difference.Y == 1)
            {
                return MoveAllowed.IfAttack;
            }
            return MoveAllowed.No;
        }

        public override Piece Clone(Board board)
        {
            return new Pawn(this, board);
            //return new Pawn(this.PieceColor, new Position(this.PiecePosition), this.CollisionBetween)
            //{
            //    FirstMove = this.FirstMove,
            //    Defeated = this.Defeated
            //};
        }

    }
}
