using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Models
{
    public class Position : IEquatable<Position>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(Position position)
        {
            X = position.X;
            Y = position.Y;
        }
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }

        public override int GetHashCode()
        {
            return $"{X},{Y}".GetHashCode();
        }

        public override bool Equals(object other)
        {
            return Equals(other as Position);
        }

        public bool Equals(Position other)
        {
            return other is not null && other.X == this.X && other.Y == this.Y;
        }
    }
}
