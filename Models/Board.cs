using BlazorChess.Debugging;
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
        public Tile PreviousTile { get; set; } = null;
        private readonly PieceFactory PieceFactory;
        public List<Piece> DefeatedPieces;
        public readonly Options Options;
        public PieceColor CurrentPlayer { get; set; } = PieceColor.White;
        private TurnHistory TurnHistory { get; set; }
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
                Tiles[tile.TilePosition] = tile.Clone(this);
            }
            PreviousTile = Tiles[board.PreviousTile?.TilePosition];
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

        public (ClickResult, Turn) TileClicked(Tile clickedTile, TurnHistory turnHistory)
        {
            Board board = this.Clone();
            (ClickResult clickResult, Turn turn) = TileClickedClone(clickedTile, turnHistory);
            if (clickResult == ClickResult.Revoke)
            {
                turn.BoardState = board;
            }
            return (clickResult, turn);
        }

        //bool = turn passed. Turn = new Turn after processing clicked tile
        private (ClickResult, Turn) TileClickedClone(Tile clickedTile, TurnHistory turnHistory)
        {
            TurnHistory = turnHistory;
            ClickResult clickResult = ClickResult.Select;
            Turn newTurn = new Turn();
            Piece movedPiece = null;
            Piece defeatedPiece = null;
            PieceColor currentPlayer = CurrentPlayer;
            PieceColor opponent = currentPlayer.NextColor();
            SpecialMove specialMove = SpecialMove.No;
            if (PreviousTile is not null)
            {
                if (PreviousTile == clickedTile)
                {
                    DeselectTile(PreviousTile);
                }
                else if (PreviousTile.Occupied && IsPlayersTurn(PreviousTile.ChessPiece.PieceColor))
                {
                    bool processMove = false;
                    Piece potentiallyDefeatedPiece = clickedTile.ChessPiece;

                    if (IsSpecialMove(clickedTile, out specialMove))
                    {
                        processMove = true;
                    }
                    else if (clickedTile.Occupied && clickedTile.ChessPiece.PieceColor == CurrentPlayer)
                    {
                        processMove = false;
                    }
                    else
                    {
                        #region TaggedWriter
                        TaggedWriter.WriteLine($"About to check if move is allowed for {PreviousTile.ChessPiece.Name} at {PreviousTile.TilePosition} to {clickedTile.TilePosition}", PreviousTile.ChessPiece, clickedTile.TilePosition);
                        #endregion
                        MoveAllowed allowMove = PreviousTile.ChessPiece.IsMoveAllowed(BoardMax, PreviousTile.TilePosition, clickedTile.TilePosition);
                        switch (allowMove)
                        {
                            case MoveAllowed.No:
                                break;
                            case MoveAllowed.Yes:
                                if (!clickedTile.Occupied || PreviousTile.ChessPiece.PieceColor != clickedTile.ChessPiece.PieceColor)
                                {
                                    processMove = true;
                                }
                                break;
                            case MoveAllowed.IfEmpty:
                                if (!clickedTile.Occupied)
                                {
                                    processMove = true;
                                }
                                break;
                            case MoveAllowed.IfAttack:
                                if (clickedTile.Occupied && PreviousTile.ChessPiece.PieceColor != clickedTile.ChessPiece.PieceColor)
                                {
                                    processMove = true;
                                }
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    if (processMove)
                    {
                        if (specialMove != SpecialMove.No)
                        {
                            MakeSpecialMove(clickedTile, specialMove);
                        }
                        else
                        {
                            #region TaggedWriter
                            TaggedWriter.WriteLine($"MoveChessPiece", PreviousTile.ChessPiece, clickedTile.TilePosition);
                            #endregion
                            MoveChessPiece(clickedTile);
                        }

                        if (IsInCheck(currentPlayer) != Check.No)
                        {
                            #region write IN CHECK
                            Console.WriteLine("----------------------------------");
                            Console.WriteLine("----------------------------------");
                            Console.WriteLine("----------------------------------");
                            Console.WriteLine("-------------IN CHECK-------------");
                            Console.WriteLine("----------------------------------");
                            Console.WriteLine("----------------------------------");
                            Console.WriteLine("----------------------------------");
                            #endregion
                            clickResult = ClickResult.Revoke;
                            Console.WriteLine($"clickResult in Check: {clickResult}");
                        }
                        else
                        {
                            TaggedWriter.WriteLine($"board: {this}, clicked.chesspiece: {clickedTile.ChessPiece}, previoustile.pos: {PreviousTile.TilePosition}, clicked.pos: {clickedTile.TilePosition}, potential: {potentiallyDefeatedPiece}", PreviousTile.ChessPiece);
                            PawnToQueen(this);
                            NextPlayer();
                            DeselectTile(PreviousTile);
                            clickResult = ClickResult.Confirm;
                        }
                    }
                    else
                    {
                        DeselectTile(PreviousTile);
                        SelectTile(clickedTile);
                    }

                }
                else
                {
                    SelectTile(clickedTile);
                }
            }
            else
            {
                SelectTile(clickedTile);
            }
            if (clickResult == ClickResult.Confirm)
            {
                newTurn.BoardState = this;
                newTurn.MovedPiece = movedPiece;
                newTurn.DefeatedPiece = defeatedPiece;
            }
            return (clickResult, newTurn);
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

        public void GetBestMove(int iterations)
        {
            Board temporaryBoard = this.Clone();
            var moves = new List<Turn>();
            var turns = new Dictionary<Tile, Turn>();
            var occupiedTiles = temporaryBoard.Tiles.Where(tile => tile.Occupied && tile.ChessPiece.PieceColor == temporaryBoard.CurrentPlayer);
            foreach (var occupiedTile in occupiedTiles)
            {
                var tilesInRange = temporaryBoard.GetTilesAndMovesAllowedWithinRange(occupiedTile);
                foreach (var tileAndMoveAllowedInRange in tilesInRange)
                {
                    var tile = tileAndMoveAllowedInRange.Key;
                    var moveAllowed = tileAndMoveAllowedInRange.Value;
                    if (!CollisionBetween(occupiedTile.TilePosition, tile.TilePosition) && NotOccupiedByAlly(occupiedTile.ChessPiece, tile))
                    {
                        Console.WriteLine($"Move {occupiedTile.TilePosition} to Tile {tile.TilePosition} move allowed: {moveAllowed}");
                    }
                }
            }
        }

        private bool NotOccupiedByAlly(Piece piece, Tile tile)
        {
            return !tile.Occupied || (tile.Occupied && tile.ChessPiece.PieceColor != piece.PieceColor);
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

        private void SelectTile(Tile tile)
        {
            tile.Selected = true;
            DeselectTile(PreviousTile);
            PreviousTile = tile;
        }

        private void DeselectTile(Tile tile)
        {
            if (tile is not null)
            {
                tile.Selected = false;
                PreviousTile = null;
            }
        }
        private bool IsPlayersTurn(PieceColor player)
        {
            return player == CurrentPlayer || !Options.EnableTurns;
        }

        private void NextPlayer()
        {
            if (Options.EnableTurns)
            {
                CurrentPlayer = CurrentPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;
            }
        }

        private void MoveChessPiece(Tile clickedTile)
        {
            Piece defeatedPiece = null;
            if (clickedTile.Occupied)
            {
                defeatedPiece = clickedTile.ChessPiece;
                defeatedPiece.Defeated = true;
                DefeatedPieces.Add(defeatedPiece);
            }
            PreviousTile.ChessPiece.FirstMove = false;
            clickedTile.ChessPiece = PreviousTile.ChessPiece;

            PreviousTile.ChessPiece = null;

            //TurnHistory.AddTurn(Board, clickedTile.ChessPiece, PreviousTile.TilePosition, clickedTile.TilePosition, defeatedPiece);

            //DeselectTile(PreviousTile);
            //NextPlayer();
        }

        private void SetEligbleLocations(Piece piece)
        {
            //TODO: Do
        }

        private bool IsSpecialMove(Tile clickedTile, out SpecialMove specialMove)
        {
            //TODO: Any of tiles where king is moving (not only final position but the path there) cannot be in line of sight of any opposing piece.
            //Castling
            if (CastleAvailable(clickedTile))
            {
                specialMove = SpecialMove.Castle;
                return true;
            }

            else if (EnPassantAvailable(PreviousTile.ChessPiece, clickedTile))
            {

                specialMove = SpecialMove.EnPassant;
                return true;
            }

            specialMove = SpecialMove.No;
            return false;
        }

        private void MakeSpecialMove(Tile clickedTile, SpecialMove specialMove)
        {
            switch (specialMove)
            {
                case SpecialMove.Castle:
                    (King king, Rook rook) = GetKingAndRook(clickedTile.ChessPiece, PreviousTile.ChessPiece);
                    Castle((King)king, (Rook)rook);
                    break;
                case SpecialMove.EnPassant:
                    EnPassant(clickedTile);
                    break;
                default:
                    break;
            }
        }

        private bool EnPassantAvailable(Piece movingPiece, Tile clickedTile)
        {
            Piece lastMovedPiece = TurnHistory.LastTurn()?.MovedPiece;
            if (AnyNullObjects(movingPiece, lastMovedPiece))
            {
                return false;
            }

            if (IsPawn(lastMovedPiece) && IsPawn(movingPiece) && !AreAllies(lastMovedPiece, movingPiece))
            {
                if (movingPiece.PiecePosition.Y == lastMovedPiece.PiecePosition.Y
                && clickedTile.TilePosition.X == lastMovedPiece.PiecePosition.X
                && lastMovedPiece.PiecePosition.Difference(movingPiece.PiecePosition).Equals(new Position(1, 0)))
                {
                    return true;
                }
            }
            return false;
        }

        private Turn EnPassant(Tile clickedTile)
        {
            Tile enemyTile = Tiles[TurnHistory.LastTurn().NewPosition];
            enemyTile.ChessPiece.Defeated = true;
            Turn turn = new Turn()
            {
                BoardState = this,
                MovedPiece = PreviousTile.ChessPiece,
                OriginalPosition = PreviousTile.TilePosition,
                NewPosition = clickedTile.TilePosition,
                DefeatedPiece = enemyTile.ChessPiece
            };
            DefeatedPieces.Add(enemyTile.ChessPiece);
            clickedTile.ChessPiece = PreviousTile.ChessPiece;
            enemyTile.ChessPiece = null;
            PreviousTile.ChessPiece = null;
            return turn;
        }

        private bool CastleAvailable(Tile clickedTile)
        {
            if (AnyNullObjects(clickedTile.ChessPiece, PreviousTile.ChessPiece))
            {
                return false;
            }
            if (AreAllies(clickedTile.ChessPiece, PreviousTile.ChessPiece) && OneOfEachType(clickedTile.ChessPiece, PreviousTile.ChessPiece, typeof(King), typeof(Rook)))
            {
                (King king, Rook rook) = GetKingAndRook(clickedTile.ChessPiece, PreviousTile.ChessPiece);
                if (king.FirstMove && rook.FirstMove)
                {
                    return !CollisionBetween(king.PiecePosition, rook.PiecePosition);
                }
            }
            return false;
        }

        private (King, Rook) GetKingAndRook(Piece a, Piece b)
        {
            Piece king = a.GetType() == typeof(King) ? a : b;
            Piece rook = a.GetType() == typeof(Rook) ? a : b;
            return ((King)king, (Rook)rook);
        }
        private void Castle(King king, Rook rook)
        {
            int kingX = rook.PiecePosition.Subtract(king.PiecePosition).X > 0 ? 2 : -2;
            int rookX = kingX > 0 ? 1 : -1;
            Tiles[king.PiecePosition].ChessPiece = null;
            Tiles[rook.PiecePosition].ChessPiece = null;
            Tiles[king.PiecePosition.X + rookX, king.PiecePosition.Y].ChessPiece = rook;
            Tiles[king.PiecePosition.X + kingX, king.PiecePosition.Y].ChessPiece = king;
            (king.FirstMove, rook.FirstMove) = (false, false);
        }

        private bool PawnToQueen(Board board)
        {
            foreach (var tile in board.Tiles)
            {
                if (tile.Occupied && IsPawn(tile.ChessPiece) && LastRowForColor(tile.TilePosition, tile.ChessPiece.PieceColor))
                {
                    ReplacePieceOnTileWith(tile, PieceType.Queen);
                }
            }
            return false;
        }

        private bool AreAllies(Piece a, Piece b)
        {
            return a.PieceColor == b.PieceColor;
        }

        private bool OneOfEachType(Piece a, Piece b, Type x, Type y)
        {
            return (a.GetType() == x && b.GetType() == y) || (a.GetType() == y && b.GetType() == x);
        }

        private bool LastRowForColor(Position position, PieceColor pieceColor)
        {
            if (pieceColor == PieceColor.White && position.Y == BoardMax.Y)
            {
                return true;
            }
            else if (pieceColor == PieceColor.Black && position.Y == BoardOrigo.Y)
            {
                return true;
            }
            return false;
        }

        private bool IsPawn(Piece piece)
        {
            return piece.GetType() == typeof(Pawn);
        }

        private bool AnyNullObjects(params object[] objects)
        {
            return objects.Any(obj => obj is null);
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
            Console.WriteLine($"Cloning board {this.GetHashCode()}");
            Board newBoard =  new Board(this);
            return newBoard;
        }
    }
}
