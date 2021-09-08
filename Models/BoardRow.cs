using BlazorChess.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Models
{
    public class BoardRow
    {
        public readonly int RowNumber;
        public readonly Tile[] Tiles;
        public BoardRow(int rowNumber, int numberOfColumns)
        {
            RowNumber = rowNumber;
            Tiles = new Tile[numberOfColumns];
        }
        public BoardRow(int rowNumber, int columns, TileColor firstTileColor)
        {
            RowNumber = rowNumber;
            Tiles = new Tile[columns];
            TileColor tileColor = firstTileColor;
            for (int i = 0; i < columns; i++)
            {
                Tiles[i] = new Tile(
                        new Position(i, rowNumber),
                        tileColor
                    );
                tileColor = tileColor.NextColor();
            }
        }
    }
}
