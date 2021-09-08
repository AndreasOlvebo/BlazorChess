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
    }
}
