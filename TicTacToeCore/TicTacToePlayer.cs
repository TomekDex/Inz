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

        public virtual void AnalyzeRsult(TicTacToeState state)
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

        public override void AnalyzeRsult(TicTacToeState state)
        {
            if (!TicTacToeLearningAIPlayer.Tree.ContainsKey(state) && !aIPlayer.Root.Equals(state))
                aIPlayer.AddNode(aIPlayer.Root, state);
        }
    }

    public class TicTacToeLearningAIPlayer : AIPlayer<TicTacToeState, TicTacToePlayer>
    {
        public override Node<TicTacToeState, TicTacToePlayer> CreateNode(TicTacToeState state)
        {
            return new Node<TicTacToeState, TicTacToePlayer>
            {
                End = state.Summary.IsEnd,
                Winner = state.Summary.Winner != TicTacToePlayerType.No,
                Player = GetPlayer(state)
            };
        }

        private TicTacToePlayer GetPlayer(TicTacToeState state)
        {
            Dictionary<TicTacToePlayerType, byte> counts = new Dictionary<TicTacToePlayerType, byte> { { TicTacToePlayerType.X, 0 }, { TicTacToePlayerType.O, 0 } };
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    TicTacToePlayerType type = state.Board[i, j];
                    if (type != TicTacToePlayerType.No)
                        counts[type]++;
                }
            if (counts[TicTacToePlayerType.O] == counts[TicTacToePlayerType.X])
                return state.Players.Single(a => a.PlayerType == TicTacToePlayerType.X);
            else
                return state.Players.Single(a => a.PlayerType == TicTacToePlayerType.O);
        }
    }
}