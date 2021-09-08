using BlazorChess.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Models
{
    public abstract class Piece
    {
        public readonly PieceColor PieceColor;
        public string Name { get { return this.GetType().Name; } }
        public string ImagePath { get { return $@"/images/{PieceColor}-{Name}.png".ToLower(); } }
        public bool FirstMove { get; set; } = true;
        public bool IgnoreCollision { get; set; } = false;
        public Position PiecePosition { get; set; }
        protected Position MinRange { get; set; } = new Position(0, 0);
        protected Position MaxRange { get; set; } = new Position(0, 0);
        public bool Defeated { get; set; } = false;

        protected Func<Position, Position, bool> CollisionBetween;

        public Piece(PieceColor pieceColor, Position piecePosition, Func<Position, Position, bool> collisionBetween)
        {
            PieceColor = pieceColor;
            PiecePosition = piecePosition;
            CollisionBetween = collisionBetween;
        }

        public abstract MoveAllowed AllowMove(Position currentPosition, Position newPosition);
        private void MakeMove(Position confirmedPosition)
        {
            if (FirstMove)
            {
                FirstMove = false;
            }
            PiecePosition = confirmedPosition;
        }
        
        public virtual (MoveAllowed, Action<Position>) IsMoveAllowed(Position boardSize, Position currentPosition, Position newPosition)
        {
            if (!WithinRange(currentPosition.Difference(newPosition)))
            {
                return (MoveAllowed.No, null);
            }
            if (!IgnoreCollision && CollisionBetween(currentPosition, newPosition))
            {
                return (MoveAllowed.No, null);
            }
            if (PieceColor == PieceColor.Black)
            {
                currentPosition = currentPosition.Invert(boardSize, Axis.Y);
                newPosition = newPosition.Invert(boardSize, Axis.Y);
            }
            return (AllowMove(currentPosition, newPosition), MakeMove);
        }

        public abstract Piece Clone();

        protected virtual bool WithinRange(Position difference)
        {
            Position absoluteDifference = difference.Absolute();
            bool xWithinRange = absoluteDifference.X >= MinRange.X && (MaxRange.X == 0 || absoluteDifference.X <= MaxRange.X);
            bool yWithinRange = absoluteDifference.Y >= MinRange.Y && (MaxRange.Y == 0 || absoluteDifference.Y <= MaxRange.Y);
            return xWithinRange && yWithinRange;
        }
    }
}
