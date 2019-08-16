using GamesCore;
using LearningAIPlayer;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToeCore
{
    public class TicTacToePlayer : IPlayer<TicTacToeState, TicTacToeMove, TicTacToePlayer, TicTacToeSummary, TicTacToeAction>
    {
        public TicTacToePlayerType PlayerType { get; set; }
        [JsonIgnore]
        public IUserInterface<TicTacToeState, TicTacToeMove, TicTacToePlayer, TicTacToeSummary, TicTacToeAction> UserInterface { get; set; }

        public virtual void AnalyzeResult(TicTacToeState state)
        {
        }

        public virtual Task<TicTacToeMove> NextMove(TicTacToeState state, List<TicTacToeMove> allowedMoves)
        {
            return UserInterface.ShowSelectionMove(state, this, allowedMoves);
        }

        public override bool Equals(object obj)
        {
            return obj is TicTacToePlayer player &&
                   PlayerType == player.PlayerType;
        }

        public override int GetHashCode()
        {
            return PlayerType.GetHashCode();
        }
    }

    public class TicTacToePlayerAI : TicTacToePlayer
    {
        readonly TicTacToeLearningAIPlayer aIPlayer;

        public TicTacToePlayerAI()
        {
            aIPlayer = new TicTacToeLearningAIPlayer();
        }

        public override async Task<TicTacToeMove> NextMove(TicTacToeState state, List<TicTacToeMove> allowedMoves)
        {
            TicTacToeState move = aIPlayer.GetNextMove(state, allowedMoves.Select(a => a.StateEnd).ToArray());
            return allowedMoves.First(a => a.StateEnd == move);
        }

        public override void AnalyzeResult(TicTacToeState state)
        {
            if (!TicTacToeLearningAIPlayer.Tree.ContainsKey(state) && !aIPlayer.Root.Equals(state))
                aIPlayer.AddNode(aIPlayer.Root, state);
        }
    }

    public class TicTacToeLearningAIPlayer : AIPlayer<TicTacToeState>
    {
        public override Node<TicTacToeState> CreateNode(TicTacToeState state)
        {
            return new Node<TicTacToeState>
            {
                End = state.Summary.IsEnd,
                Winner = state.Summary.Winner != TicTacToePlayerType.No
            };
        }
    }
}