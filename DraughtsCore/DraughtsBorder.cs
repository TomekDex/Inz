using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DraughtsCore
{
    public class DraughtsBorder : ICloneable
    {
        private DraughtsPlayerType?[,] border = new DraughtsPlayerType?[5, 5];

        public Dictionary<Point, Vector[]> PlacesAndNeighbors { get; }

        public DraughtsBorder()
        {
            PlacesAndNeighbors = GetPlaces().ToDictionary(a=>a,GetNeighbors);
            foreach (var place in PlacesAndNeighbors.Keys)
                if (place.X != 2)
                    border[place.X, place.Y] = place.X < 2 ? DraughtsPlayerType.Black : DraughtsPlayerType.White;
        }

        private Vector[] GetNeighbors(Point point)
        {
            List<Vector> vectors = new List<Vector>();
            for (short i = -1; i < 2; i++)
                for (short j = -1; j < 2; j++)
                {
                    short newX = (short)(point.X + i);
                    short newY = (short)(point.Y + j);
                    if (newX < 0 || newX >= 5 || newY < 0 || newY >= 5)
                        continue;
                    if ((newX + newY) % 2 != 0)
                        continue;
                    vectors.Add(new Vector(i, j));
                }
            return vectors.ToArray();
        }

        private IEnumerable<Point> GetPlaces()
        {
            for (byte x = 0; x < 5; x++)
                for (byte y = 0; y < 5; y++)
                    if ((x + y) % 2 == 0)
                        yield return new Point(x, y);
        }

        private DraughtsBorder(DraughtsBorder draughtsBorder)
        {
            border = (DraughtsPlayerType?[,])draughtsBorder.border.Clone();
            PlacesAndNeighbors = draughtsBorder.PlacesAndNeighbors;
        }

        public DraughtsPlayerType? this[Point point]
        {
            get
            {
                return border[point.X, point.Y];
            }
            set
            {
                border[point.X, point.Y] = value;
            }
        }

        public object Clone()
        {
            return new DraughtsBorder(this);
        }
    }

    public struct Vector
    {
        public short Y { get; set; }
        public short X { get; set; }

        public Vector(short x, short y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector vector &&
                   Y == vector.Y &&
                   X == vector.X;
        }

        public override int GetHashCode()
        {
            var hashCode = 27121115;
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            return hashCode;
        }
    }
}
