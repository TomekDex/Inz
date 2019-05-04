using GamesCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TicTacToeCore
{
    public class TicTacToePlayer : IPlayer<TicTacToeState, TicTacToeMove, TicTacToePlayer, TicTacToeSummary, TicTacToeAction>
    {
        public TicTacToePlayerType PlayerType { get; internal set; }
        public IUserInterface<TicTacToeState, TicTacToeMove, TicTacToePlayer, TicTacToeSummary, TicTacToeAction> UserInterface { get; set; }

        public virtual Task<TicTacToeMove> NextMove(TicTacToeState state, List<TicTacToeMove> allowedMoves)
        {
            return UserInterface.ShowSelectionMove(state, this, allowedMoves);
        }
    }

    public class TicTacToePlayerAI : TicTacToePlayer
    {
        readonly Random random = new Random();

        public override async Task<TicTacToeMove> NextMove(TicTacToeState state, List<TicTacToeMove> allowedMoves)
        {
            return allowedMoves[random.Next(allowedMoves.Count)];
        }
    }
}