using GamesCore;
using System.Drawing;
using System.Linq;

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
            Vector[] neighbors = state.Board.PlacesAndNeighbors[point];

            if (!neighbors.Contains(vector) ||
                playerType != state.Board[point] ||
                state.Board[PointTarget] != default ||
                PlayerType == DraughtsPlayerType.Black && vector.X == -1 ||
                PlayerType == DraughtsPlayerType.White && vector.X == 1)
                IsAllowed = false;
            else
                IsAllowed = true;
        }

        public override void Execute(DraughtsState state)
        {
            state.Board[Point] = default;
            state.Board[PointTarget] = PlayerType;
        }
    }

    public class DraughtsActionHit : DraughtsAction
    {
        public Point PointTarget { get; internal set; }
        public Point PointTargetToHit { get; internal set; }

        public DraughtsActionHit(DraughtsPlayerType playerType, Point point, Vector vector, DraughtsState state) : base(playerType, point, vector)
        {
            Vector[] neighbors = state.Board.PlacesAndNeighbors[point];
            PointTargetToHit = new Point(point.X + vector.X, point.Y + vector.Y);
            PointTarget = new Point(point.X + vector.X * 2, point.Y + vector.Y * 2);
            DraughtsPlayerType enemy = PlayerType == DraughtsPlayerType.Black ? DraughtsPlayerType.White : DraughtsPlayerType.Black;

            if (!neighbors.Contains(vector) ||
                !state.Board.PlacesAndNeighbors[PointTargetToHit].Contains(vector) ||
                playerType != state.Board[point] ||
                state.Board[PointTargetToHit] != enemy ||
                state.Board[PointTarget] != default)
                IsAllowed = false;
            else
                IsAllowed = true;
        }

        public override void Execute(DraughtsState state)
        {
            state.Board[Point] = default;
            state.Board[PointTargetToHit] = default;
            state.Board[PointTarget] = PlayerType;
        }
    }
}
