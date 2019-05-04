using GamesCore;
using System;
using System.Collections.Generic;

namespace DraughtsCore
{
    public class DraughtsPlayer : IPlayer<DraughtsState, DraughtsMove, DraughtsPlayer, DraughtsSummary, DraughtsAction>
    {
        public DraughtsPlayerType PlayerType { get; internal set; }
        public bool NextInThisMove { get; internal set; }

        public IUserInterface<DraughtsState, DraughtsMove, DraughtsPlayer, DraughtsSummary, DraughtsAction> UserInterface { get; set; }
        public bool NoMove { get; internal set; }

        public virtual DraughtsMove NextMove(DraughtsState state, List<DraughtsMove> allowedMoves)
        {
            return UserInterface.ShowSelectionMove(state, this, allowedMoves);
        }
    }

    public class DraughtsPlayerAI : DraughtsPlayer
    {
        readonly Random random = new Random();
        public override DraughtsMove NextMove(DraughtsState state, List<DraughtsMove> allowedMoves)
        {
            return allowedMoves[random.Next(allowedMoves.Count)];
        }
    }
}