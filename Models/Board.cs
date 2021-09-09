using BlazorChess.Extensions;
using BlazorChess.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Models
{
    public class Board
    {
        public int NumberOfRows;
        public int NumberOfColumns { get; init; }
        public Tiles Tiles { get; set; }
        //public BoardRow[] Rows;
        private readonly PieceFactory PieceFactory;
        public List<Piece> DefeatedPieces;
        public readonly Options Options;

        public Board(int rows, int columns)
        {
            NumberOfRows = rows;
            NumberOfColumns = columns;
            DefeatedPieces = new List<Piece>();
            PieceFactory = new PieceFactory(CollisionBetween);
            TileColor startingTileColor = TileColor.Black;
            Options = new Options();
            Tiles = new Tiles(rows, columns, startingTileColor);
            
            PopulateBoard();
        }

        public Board(Board board)
        {
            NumberOfRows = board.NumberOfRows;
            NumberOfColumns = board.NumberOfColumns;

            Tiles = new Tiles(NumberOfRows, NumberOfColumns, board.Tiles[0, 0].Color);
            foreach (var tile in board.Tiles)
            {
                Tiles[tile.TilePosition].ChessPiece = tile.ChessPiece?.Clone(); 
            }
            
            DefeatedPieces = new List<Piece>();
            
            foreach (var defeatedPiece in board.DefeatedPieces)
            {
                DefeatedPieces.Add(defeatedPiece.Clone());
            }
        }

        public Tile GetTile(Position position)
        {
            return Tiles[position];
        }

        public Piece GetPiece(Position position)
        {
            return Tiles[position].ChessPiece;
        }

        public Check IsCheck(PieceColor playerColor)
        {
            throw new NotImplementedException();
        }

        //Unused --evaluate if should be removed or kept for later.
        public bool WithinBounds(Position newPosition)
        {
            return WithinRange(newPosition.X, 0, NumberOfRows - 1) && WithinRange(newPosition.Y, 0, NumberOfColumns - 1);
        }
        private static bool WithinRange(int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        private static bool MovesAlong(Position difference, Axis axis)
        {
            Position absolutePosition = new Position(difference.X, difference.Y).Absolute();
            return axis switch
            {
                Axis.All => absolutePosition.X > 1 && absolutePosition.Y > 1,
                Axis.X => absolutePosition.Y == 0 && absolutePosition.X > 0,
                Axis.Y => absolutePosition.X == 0 && absolutePosition.Y > 0,
                _ => throw new NotImplementedException(),
            };
        }

        public bool CollisionBetween(Position a, Position b)
        {
            int safetyCounter = 0;
            Position positionToEvaluate = new Position(a.X, a.Y);
            while (!positionToEvaluate.Matches(b) && safetyCounter < 10)
            {
                Position difference = positionToEvaluate.Subtract(b);

                if (difference.Absolute().X == 1 || difference.Absolute().Y == 1)
                {
                    return false;
                }
                else if (MovesAlong(difference, Axis.Y))
                {
                    positionToEvaluate.AddOrSubtractOne(difference, Axis.Y);
                }
                else if (MovesAlong(difference, Axis.X))
                {
                    positionToEvaluate.AddOrSubtractOne(difference, Axis.X);
                }
                else
                {
                    positionToEvaluate.AddOrSubtractOne(difference, Axis.All);
                }

                if (GetTile(positionToEvaluate).Occupied)
                {
                    return true;
                }
                else
                {
                    safetyCounter++;
                }
            }
            if (safetyCounter >= 10)
            {
                throw new Exception($"SafetyCounter reached {safetyCounter}");
            }
            return false;
        }

        private void PopulateBoard()
        {
            Position boardSize = new Position(NumberOfColumns - 1, NumberOfRows - 1);
            PieceType[] pieceTypes = new[]
            {
                PieceType.Rook,
                PieceType.Knight,
                PieceType.Bishop,
                PieceType.Queen,
                PieceType.King,
                PieceType.Bishop,
                PieceType.Knight,
                PieceType.Rook,
                PieceType.Pawn,
                PieceType.Pawn,
                PieceType.Pawn,
                PieceType.Pawn,
                PieceType.Pawn,
                PieceType.Pawn,
                PieceType.Pawn,
                PieceType.Pawn,
            };
            Piece[] pieces = PieceFactory.GeneratePieces(boardSize, pieceTypes);

            foreach (var piece in pieces)
            {
                GetTile(piece.PiecePosition).ChessPiece = piece;
            }
        }

        public Board Clone()
        {
            return new Board(this);
        }
    }
}
