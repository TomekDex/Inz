using GamesCore;
using LearningAIPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeCore
{
    public class TicTacToePlayer : IPlayer<TicTacToeState, TicTacToeMove, TicTacToePlayer, TicTacToeSummary, TicTacToeAction>
    {
        public TicTacToePlayerType PlayerType { get; internal set; }
        public IUserInterface<TicTacToeState, TicTacToeMove, TicTacToePlayer, TicTacToeSummary, TicTacToeAction> UserInterface { get; set; }

        public virtual void AnalyzeRsult(TicTacToeState state)
        {
        }

        public virtual Task<TicTacToeMove> NextMove(TicTacToeState state, List<TicTacToeMove> allowedMoves)
        {
            return UserInterface.ShowSelectionMove(state, this, allowedMoves);
        }
    }

    public class TicTacToePlayerAI : TicTacToePlayer
    {
        readonly Random random = new Random();
        readonly TicTacToeLearningAIPlayer aIPlayer;

        public TicTacToePlayerAI()
        {
            aIPlayer = new TicTacToeLearningAIPlayer(this);
        }

        public override async Task<TicTacToeMove> NextMove(TicTacToeState state, List<TicTacToeMove> allowedMoves)
        {
            TicTacToeState move = aIPlayer.GetNextMove(state, allowedMoves.Select(a => a.StateEnd).ToArray());
            return allowedMoves.First(a => a.StateEnd == move);
        }

        public override void AnalyzeRsult(TicTacToeState state)
        {
            if (!aIPlayer.Root.State.Equals(state))
            {
                aIPlayer.Root.Score = aIPlayer.GetScore(state);
            }
        }
    }

    public class TicTacToeLearningAIPlayer : AIPlayer<TicTacToeState>
    {
        private readonly TicTacToePlayerAI ticTacToePlayerAI;

        public TicTacToeLearningAIPlayer(TicTacToePlayerAI ticTacToePlayerAI)
        {
            this.ticTacToePlayerAI = ticTacToePlayerAI;
        }

        public override Node<TicTacToeState> CreateNode(TicTacToeState state)
        {
            return new Node<TicTacToeState>
            {
                State = state,
                Score = GetScore(state)
            };
        }

        public Score GetScore(TicTacToeState state)
        {
            if (state.Summary.IsEnd)
            {
                if (!state.Summary.Winner.HasValue)
                    return Score.Draw;
                return state.Summary.Winner == ticTacToePlayerAI.PlayerType ? Score.Win : Score.Defeat;
            }
            return Score.NotEnd;
        }
    }
}