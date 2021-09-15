using BlazorChess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorChess.Extensions
{
    public static class PositionExtensions
    {
        public static bool Matches(this Position a, Position b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static Position Subtract(this Position position, Position positionToSubtractWith, Axis axis = Axis.All)
        {
            return axis switch
            {
                Axis.All => new Position(position.X - positionToSubtractWith.X, position.Y - positionToSubtractWith.Y),
                Axis.X => new Position(position.X - positionToSubtractWith.X, position.Y),
                Axis.Y => new Position(position.X, position.Y - positionToSubtractWith.Y),
                _ => throw new NotImplementedException(),
            };
        }

        public static Position Negate(this Position position)
        {
            return new Position(position.X * -1, position.Y * -1);
        }
        public static Position Absolute(this Position position, Axis axis = Axis.All)
        {
            int x = (axis == Axis.All || axis == Axis.X) ? position.X >= 0 ? position.X : position.X * -1 : position.X;
            int y = (axis == Axis.All || axis == Axis.Y) ? position.Y >= 0 ? position.Y : position.Y * -1 : position.Y;

            return new Position(x, y);
        }

        public static Position Invert(this Position position, Position bounds, Axis axis = Axis.All)
        {
            return axis switch
            {
                Axis.All => bounds.Subtract(position, axis),
                Axis.X => new Position(bounds.Subtract(position, axis).X, position.Y),
                Axis.Y => new Position(position.X, bounds.Subtract(position, axis).Y),
                _ => throw new NotImplementedException(),
            };
        }

        public static Position Difference(this Position a, Position b, Axis axis = Axis.All)
        {
            Position difference = b.Subtract(a);
            return difference.Absolute(axis);
        }

        public static void AddOrSubtractOne(this Position currentPosition, Position difference, Axis axis)
        {
            switch (axis)
            {
                case Axis.All:
                    currentPosition.X -= difference.X > 0 ? 1 : -1;
                    currentPosition.Y -= difference.Y > 0 ? 1 : -1;
                    break;
                case Axis.X:
                    currentPosition.X -= difference.X > 0 ? 1 : -1;
                    break;
                case Axis.Y:
                    currentPosition.Y -= difference.Y > 0 ? 1 : -1;
                    break;
                default:
                    throw new NotImplementedException();
            }

        }

        public static bool BothMatchesEither(this Position positionToEvaluate, Position positionToMatch)
        {
            if (positionToEvaluate.X == positionToMatch.X && positionToEvaluate.Y == positionToMatch.Y)
            {
                return true;
            }
            if (positionToEvaluate.Y == positionToMatch.X && positionToEvaluate.X == positionToMatch.Y)
            {
                return true;
            }
            return false;
        }

        public static Position[] PositionsWithinRange(this Position position, (Position boardOrigo, Position boardMax) boardSize, Position maximumRange, Position minimumRange)
        {
            List<Position> positionsToIterate = new();
            (int lowX, int highX) = GetLowAndHigh(position.X, boardSize.boardOrigo.X, boardSize.boardMax.X, minimumRange.X, maximumRange.X);
            (int lowY, int highY) = GetLowAndHigh(position.Y, boardSize.boardOrigo.Y, boardSize.boardMax.Y, minimumRange.Y, maximumRange.Y);

            for (int y = lowY; y <= highY; y++)
            {
                for (int x = lowX; x <= highX; x++)
                {
                    if (!position.Equals(new Position(x, y)))
                    {
                        Position positionToAdd = new Position(x, y);
                        positionsToIterate.Add(positionToAdd);
                    }
                }
            }

            return positionsToIterate.ToArray();
        }

        private static (int, int) GetLowAndHigh(int position, int lowEndOfAxis, int highEndOFAxis, int minimumRange, int maximumRange)
        {
            int lower = position - maximumRange;
            lower = maximumRange == 0 || lower < lowEndOfAxis ? lowEndOfAxis : lower;
            lower = position - lower < minimumRange ? position : lower;

            int higher = position + maximumRange;
            higher = maximumRange == 0 || higher > highEndOFAxis ? highEndOFAxis : higher;
            higher = higher - position < minimumRange ? position : higher;

            return (lower, higher);
        }
    }
}
