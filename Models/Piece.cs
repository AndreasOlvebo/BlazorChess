using BlazorChess.Debugging;
using BlazorChess.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Models
{
    public abstract class Piece
    {
        public PieceColor PieceColor;
        public string Name { get { return this.GetType().Name; } }
        public string ImagePath { get { return $@"/images/{PieceColor}-{Name}.png".ToLower(); } }
        public bool FirstMove { get; set; } = true;
        public bool IgnoreCollision { get; set; } = false;
        public Position PiecePosition { get; set; }
        public Position MinRange { get; set; } = new Position(0, 0);
        public Position MaxRange { get; set; } = new Position(0, 0);
        public bool Defeated { get; set; } = false;
        public bool Tagged { get; set; } = false;

        public Func<Position, Position, bool> CollisionBetween;

        public Piece(PieceColor pieceColor, Position piecePosition, Func<Position, Position, bool> collisionBetween)
        {
            PieceColor = pieceColor;
            PiecePosition = piecePosition;
            CollisionBetween = collisionBetween;
        }

        public Piece(Piece piece, Board board)
        {
            MinRange = piece.MinRange;
            MaxRange = piece.MaxRange;
            PiecePosition = piece.PiecePosition;
            PieceColor = piece.PieceColor;
            Defeated = piece.Defeated;
            CollisionBetween = board.CollisionBetween;
            FirstMove = piece.FirstMove;
            IgnoreCollision = piece.IgnoreCollision;
            Tagged = piece.Tagged;
            piece.Tagged = false;
        }

        public abstract MoveAllowed AllowMove(Position currentPosition, Position newPosition);
        public void MakeMove(Position confirmedPosition)
        {
            if (FirstMove)
            {
                FirstMove = false;
            }
            //PiecePosition = confirmedPosition;
        }
        
        public virtual MoveAllowed IsMoveAllowed(Position boardSize, Position currentPosition, Position newPosition)
        {
            TaggedWriter.WriteLine($"difference curr - new pos: {currentPosition.Difference(newPosition)}", this, newPosition);
            TaggedWriter.WriteLine($"WithinRange: {WithinRange(currentPosition.Difference(newPosition))}", this, newPosition);
            TaggedWriter.WriteLine($"CollisionBetween(currentPosition, newPosition): {CollisionBetween(currentPosition, newPosition)}", this, newPosition);
            if (!WithinRange(currentPosition.Difference(newPosition)))
            {
                return MoveAllowed.No;
            }
            if (!IgnoreCollision && CollisionBetween(currentPosition, newPosition))
            {
                return MoveAllowed.No;
            }
            if (PieceColor == PieceColor.Black)
            {
                currentPosition = currentPosition.Invert(boardSize, Axis.Y);
                newPosition = newPosition.Invert(boardSize, Axis.Y);
            }
            return AllowMove(currentPosition, newPosition);
        }

        public abstract Piece Clone(Board board);

        protected virtual bool WithinRange(Position difference)
        {
            Position absoluteDifference = difference.Absolute();
            bool xWithinRange = absoluteDifference.X >= MinRange.X && (MaxRange.X == 0 || absoluteDifference.X <= MaxRange.X);
            bool yWithinRange = absoluteDifference.Y >= MinRange.Y && (MaxRange.Y == 0 || absoluteDifference.Y <= MaxRange.Y);
            return xWithinRange && yWithinRange;
        }
    }
}
