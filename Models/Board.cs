using BlazorChess.Extensions;
using BlazorChess.Models.PieceTypes;
using BlazorChess.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Models
{
    public class Board
    {
        public int BoardId { get; set; }
        public int NumberOfRows;
        public int NumberOfColumns;
        public readonly Position BoardOrigo;
        public readonly Position BoardMax;
        public Tiles Tiles { get; set; }
        //public BoardRow[] Rows;
        private readonly PieceFactory PieceFactory;
        public List<Piece> DefeatedPieces;
        public readonly Options Options;
        public PieceColor CurrentPlayer { get; set; } = PieceColor.White;
        private static object _sync = new();

        public Board(int rows, int columns)
        {
            BoardId = 1;
            NumberOfRows = rows;
            NumberOfColumns = columns;
            BoardOrigo = new Position(0, 0);
            BoardMax = new Position(columns - 1, rows - 1);
            DefeatedPieces = new List<Piece>();
            PieceFactory = new PieceFactory(CollisionBetween);
            TileColor startingTileColor = TileColor.Black;
            Options = new Options();
            Tiles = new Tiles(rows, columns, startingTileColor);
            
            PopulateBoard();
        }

        public Board(Board board)
        {
            BoardId = board.BoardId + 1;
            NumberOfRows = board.NumberOfRows;
            NumberOfColumns = board.NumberOfColumns;
            BoardOrigo = new Position(board.BoardOrigo);
            BoardMax = new Position(board.BoardMax);
            PieceFactory = new PieceFactory(CollisionBetween);
            Tiles = new Tiles(NumberOfRows, NumberOfColumns, board.Tiles[0, 0].Color);
            foreach (var tile in board.Tiles)
            {
                Tiles[tile.TilePosition].ChessPiece = tile.ChessPiece?.Clone(this); 
            }
            DefeatedPieces = new List<Piece>();
            
            foreach (var defeatedPiece in board.DefeatedPieces)
            {
                DefeatedPieces.Add(defeatedPiece);
            }
            Options = board.Options.Clone();
            CurrentPlayer = board.CurrentPlayer;
            
        }

        public Tile GetTile(Position position)
        {
            return Tiles[position];
        }

        public Piece GetPiece(Position position)
        {
            return Tiles[position].ChessPiece;
        }

        public Check IsInCheck(PieceColor playerColor)
        {
            //TODO: add IsInCheckBy som en lista kanske?
            Check isCheck = Check.No;
            var occupiedTiles = Tiles.Where(tile => tile.Occupied && tile.ChessPiece.PieceColor == playerColor.NextColor());

            foreach (var occupiedTile in occupiedTiles)
            {
                var tilesInRange = GetTilesAndMovesAllowedWithinRange(occupiedTile);

                foreach (var tileAndMoveAllowedInRange in tilesInRange)
                {
                    var tile = tileAndMoveAllowedInRange.Key;
                    var moveAllowed = tileAndMoveAllowedInRange.Value;
                    if (IsKingAndMoveAllowed(tile, moveAllowed, playerColor))
                    {
                        Console.WriteLine($"-----------------IS KING AND MOVE ALLOWED!-----------------------");
                        Console.WriteLine($"{occupiedTile.ChessPiece.PieceColor} {occupiedTile.ChessPiece.Name} at { occupiedTile.TilePosition} can attack {playerColor} {tile.ChessPiece.Name} at {tile.TilePosition}");
                        isCheck = Check.Yes;
                        break;
                    }
                }
                if (isCheck == Check.Yes)
                {
                    break;
                }
            }
            if (isCheck == Check.Yes)
            {
                isCheck = IsCheckMate(isCheck, playerColor);
            }
            return isCheck;
        }

        private static bool IsKingAndMoveAllowed(Tile tile, MoveAllowed moveAllowed, PieceColor kingColor)
        {
            return tile.ChessPiece is not null && tile.ChessPiece.GetType() == typeof(King) && tile.ChessPiece.PieceColor == kingColor && (moveAllowed == MoveAllowed.Yes || moveAllowed == MoveAllowed.IfAttack);
        }

        public Check IsCheckMate(Check check, PieceColor playerColor)
        {
            //TODO: check if checkmate
            return check;
        }

        private Dictionary<Tile, MoveAllowed> GetTilesAndMovesAllowedWithinRange(Tile tile)
        {
            Dictionary<Tile, MoveAllowed> tilesWhereMoveAllowed = new();
            Piece piece = tile.ChessPiece;
      
            //TODO: Gör detta parallelt
            Position[] possiblePositions = piece.PiecePosition.PositionsWithinRange((BoardOrigo, BoardMax), piece.MaxRange, piece.MinRange);

            foreach (var possiblePosition in possiblePositions)
            {
                if (!WithinBoundsOf(possiblePosition, BoardOrigo, BoardMax))
                {
                    continue;
                }
                MoveAllowed moveAllowed = piece.IsMoveAllowed(BoardMax, piece.PiecePosition, possiblePosition);
                if (moveAllowed != MoveAllowed.No)
                {
                    lock (_sync)
                    {
                        tilesWhereMoveAllowed.Add(Tiles[possiblePosition], moveAllowed); 
                    }
                }
            }

            return tilesWhereMoveAllowed;
        }

        public void ReplacePieceOnTileWith(Tile tile, PieceType pieceType)
        {
            tile.ChessPiece = PieceFactory.GeneratePiece(tile.ChessPiece.PieceColor, pieceType, tile.TilePosition);
        }

        public bool WithinBoundsOf(Position position, Position minRange, Position maxRange)
        {
            return WithinRange(position.X, minRange.X, maxRange.X) && WithinRange(position.Y, minRange.Y, maxRange.Y);
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
            Board newBoard =  new Board(this);
            //Console.WriteLine($"Cloning board {this.GetHashCode()} to {newBoard.GetHashCode()}");
            return newBoard;
        }
    }
}
