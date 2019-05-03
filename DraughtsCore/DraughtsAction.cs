using GamesCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DraughtsCore
{

    public abstract class DraughtsAction : IAction
    {
        protected DraughtsAction(DraughtsPlayerType playerType, Point point, Vector vector)
        {
            PlayerType = playerType;
            Vector = vector;
            Point = point;
        }

        public DraughtsPlayerType PlayerType { get; internal set; }
        public Point Point { get; internal set; }
        public Vector Vector { get; internal set; }
        public bool IsAllowed { get; internal set; }
        public bool IsEnd { get; set; }

        public abstract void Execute(DraughtsState state);
    }

    public class DraughtsActionMove : DraughtsAction
    {
        public Point PointTarget { get; internal set; }
        public DraughtsActionMove(DraughtsPlayerType playerType, Point point, Vector vector, DraughtsState state) : base(playerType, point, vector)
        {
            IsEnd = true;
            PointTarget = new Point(point.X + vector.X, point.Y + vector.Y);
            Vector[] neighbors = state.Border.PlacesAndNeighbors[point];

            if (!neighbors.Contains(vector) ||
                playerType != state.Border[point] ||
                state.Border[PointTarget] != default ||
                PlayerType == DraughtsPlayerType.Black && vector.X == 1 ||
                PlayerType == DraughtsPlayerType.White && vector.X == -1)
                IsAllowed = false;
            else
                IsAllowed = true;
        }

        public override void Execute(DraughtsState state)
        {
            state.Border[Point] = default;
            state.Border[PointTarget] = PlayerType;
        }
    }

    public class DraughtsActionHit : DraughtsAction
    {
        public Point PointTarget { get; internal set; }
        public Point PointTargetToHit { get; internal set; }

        public DraughtsActionHit(DraughtsPlayerType playerType, Point point, Vector vector, DraughtsState state, bool isFirst) : base(playerType, point, vector)
        {
            Vector[] neighbors = state.Border.PlacesAndNeighbors[point];
            PointTargetToHit = new Point(point.X + vector.X, point.Y + vector.Y);
            PointTarget = new Point(point.X + vector.X * 2, point.Y + vector.Y * 2);
            DraughtsPlayerType enemy = PlayerType == DraughtsPlayerType.Black ? DraughtsPlayerType.White : DraughtsPlayerType.Black;

            if (!neighbors.Contains(vector) ||
                !state.Border.PlacesAndNeighbors[PointTargetToHit].Contains(vector) ||
                playerType != state.Border[point] ||
                state.Border[PointTargetToHit] != enemy ||
                state.Border[PointTarget] != default ||
                isFirst && (PlayerType == DraughtsPlayerType.Black && vector.X == 1 ||
                PlayerType == DraughtsPlayerType.White && vector.X == -1))
                IsAllowed = false;
            else
                IsAllowed = true;
        }

        public override void Execute(DraughtsState state)
        {
            state.Border[Point] = default;
            state.Border[PointTargetToHit] = default;
            state.Border[PointTarget] = PlayerType;
        }
    }
}
