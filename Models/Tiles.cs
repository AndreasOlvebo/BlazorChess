using BlazorChess.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Models
{
    public class Tiles : IEnumerable<Tile>
    {
        private Dictionary<Position, Tile> TileDictionary { get; set; }
        private readonly IOrderedEnumerable<Position> OrderedPositions;

        public Tiles(int rows, int columns, TileColor firstTileColor)
        {
            firstTileColor = firstTileColor.NextColor();
            TileDictionary = new Dictionary<Position, Tile>();
            for (int i = 0; i < rows; i++)
            {
                firstTileColor = firstTileColor.NextColor();
                TileColor tileColor = firstTileColor;
                for (int j = 0; j < columns; j++)
                {
                    Position position = new Position(j, i);
                    TileDictionary[position] = new Tile(position, tileColor);
                    tileColor = tileColor.NextColor();
                }
            }
            OrderedPositions = TileDictionary.Keys.OrderBy(pos => pos.Y).ThenBy(pos => pos.X);
        }
        public IEnumerator<Tile> GetEnumerator()
        {
            List<Tile> tiles = new List<Tile>();
            foreach (var pos in OrderedPositions)
            {
                tiles.Add(TileDictionary[pos]);
            }
            return tiles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public Tile this[Position position]
        {
            get 
            {
                if (position is null)
                {
                    return null;
                }
                return TileDictionary[position];
            }
            set
            {
                TileDictionary[position] = value;
            }
        }

        public Tile this[int x, int y]
        {
            get { return TileDictionary[new Position(x, y)]; }
            set
            {
                TileDictionary[new Position(x, y)] = value;
            }
        }

        public Tile Get(Position position)
        {
            return TileDictionary[position];
        }
    }
}
