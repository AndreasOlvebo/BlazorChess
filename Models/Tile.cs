using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;

namespace BlazorChess.Models
{
    public class Tile
    {
        public TileColor Color;
        public readonly Position TilePosition;
        public bool Selected { get; set; } = false;
        public bool Occupied { get { return ChessPiece is not null; } }
        private Piece _chessPiece;
        public Piece ChessPiece { 
            get { return _chessPiece; } 
            set {
                if (value is not null && !value.PiecePosition.Equals(TilePosition))
                {
                    value.PiecePosition = TilePosition;
                }
                _chessPiece = value;
            }
        }

        public Tile(Position tilePosition, TileColor color)
        {
            TilePosition = tilePosition;
            Color = color;
        }
    }
}
