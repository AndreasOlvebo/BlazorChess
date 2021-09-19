using BlazorChess.Extensions;
using BlazorChess.Models.PieceTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Models
{
    public class PieceFactory
    {
        private int CurrentPieceId { get; set; } = 0;
        private readonly Func<Position, Position, bool> CollisionBetween;

        public PieceFactory(Func<Position, Position, bool> collisionBetween)
        {
            CollisionBetween = collisionBetween;
        }
        public Piece GeneratePiece(PieceColor pieceColor, PieceType type, Position piecePosition)
        {
            Piece piece = type switch
            {
                PieceType.Pawn => new Pawn(pieceColor, piecePosition, CollisionBetween),
                PieceType.Bishop => new Bishop(pieceColor, piecePosition, CollisionBetween),
                PieceType.Knight => new Knight(pieceColor, piecePosition, CollisionBetween),
                PieceType.Rook => new Rook(pieceColor, piecePosition, CollisionBetween),
                PieceType.Queen => new Queen(pieceColor, piecePosition, CollisionBetween),
                PieceType.King => new King(pieceColor, piecePosition, CollisionBetween),
                _ => throw new NotImplementedException(),
            };

            piece.Id = CurrentPieceId;
            CurrentPieceId += 1;

            return piece;
        }

        public Piece[] GeneratePieces(Position boardSize, PieceType[] pieceTypes)
        {
            List<Piece> pieces = new List<Piece>();
            Position position = new Position(0, 0);
            foreach (var pieceType in pieceTypes)
            {
                if (position.X > boardSize.X)
                {
                    position.X = 0;
                    position.Y += 1;
                }
                Position piecePosition = new Position(position.X, position.Y);
                pieces.Add(GeneratePiece(PieceColor.White, pieceType, piecePosition));
                pieces.Add(GeneratePiece(PieceColor.Black, pieceType, piecePosition.Invert(boardSize, Axis.Y)));
                position.X += 1;
            }

            return pieces.ToArray();
        }
    }
}
