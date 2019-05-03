using GamesCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DraughtsCore
{
    public enum DraughtsPlayerType
    {
        White,
        Black
    }

    public class Draughts : Game<DraughtsState, DraughtsMove, DraughtsPlayer, DraughtsSummary, DraughtsJudge, DraughtsAction>
    {
        public Draughts()
        {
        }
    }

    public class DraughtsMove : IMove<DraughtsState, DraughtsSummary, DraughtsAction>
    {
        public DraughtsMove(DraughtsState state, List<DraughtsAction> actions)
        {
            StateStart = state;
            Actions = actions;
            StateEnd = (DraughtsState)state.Clone();
            foreach (var action in actions)
                action.Execute(StateEnd);
        }

        public DraughtsState StateStart { get; set; }
        public DraughtsState StateEnd { get; set; }
        public List<DraughtsAction> Actions { get; set; }
    }

    public class DraughtsState : IState<DraughtsSummary>
    {
        public DraughtsPlayer[] Players { get; }
        public DraughtsPlayerType CurentPlayer { get; set; }
        public DraughtsBorder Border { get; } = new DraughtsBorder();
        public DraughtsSummary Summary { get { return new DraughtsSummary(this); } }

        public DraughtsState(DraughtsPlayer[] players)
        {
            Random random = new Random();
            CurentPlayer = DraughtsPlayerType.Black;
            players[random.Next(2)].PlayerType = DraughtsPlayerType.Black;
            Players = players;
        }

        private DraughtsState(DraughtsState state)
        {
            CurentPlayer = state.CurentPlayer;
            Players = state.Players;
            Border = (DraughtsBorder)state.Border.Clone();
        }

        public object Clone()
        {
            return new DraughtsState(this);
        }
    }

    public class DraughtsSummary : ISummary
    {
        public Dictionary<DraughtsPlayerType, byte> Results { get; set; } = new Dictionary<DraughtsPlayerType, byte> { { DraughtsPlayerType.Black, 0 }, { DraughtsPlayerType.White, 0 } };
        public DraughtsPlayerType? Winner { get; set; }
        public DraughtsSummary(DraughtsState draughtsState)
        {
            foreach (var place in draughtsState.Border.PlacesAndNeighbors.Keys)
                if (draughtsState.Border[place] != null)
                    Results[draughtsState.Border[place].Value]++;
            var max = Results.Values.Max();
            if (!Results.All(a => a.Value == max))
                Winner = Results.First(a => a.Value == max).Key;
        }
    }
}
