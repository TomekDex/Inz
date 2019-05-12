using GamesCore;
using System.Collections.Generic;

namespace TicTacToeCore
{
    public enum TicTacToePlayerType
    {
        No = 0,
        X = 2,
        O = 3
    }

    public class TicTacToe : Game<TicTacToeState, TicTacToeMove, TicTacToePlayer, TicTacToeSummary, TicTacToeJudge, TicTacToeAction, TicTacToeSettings>
    {
    }

    public class TicTacToeMove : IMove<TicTacToeState, TicTacToeSummary, TicTacToeAction>
    {
        public TicTacToeMove(TicTacToeAction action, TicTacToeState state)
        {
            Actions = new List<TicTacToeAction> { action };
            StateStart = state;
            StateEnd = (TicTacToeState)state.Clone();
            action.Execute(StateEnd);
        }

        public TicTacToeState StateStart { get; set; }
        public TicTacToeState StateEnd { get; set; }
        public List<TicTacToeAction> Actions { get; set; }
    }
}
