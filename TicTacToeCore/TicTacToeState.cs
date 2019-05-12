using GamesCore;
using Newtonsoft.Json;
using System;

namespace TicTacToeCore
{
    public class TicTacToeState : IState<TicTacToeSummary>
    {
        [JsonIgnore]
        public TicTacToePlayer[] Players { get; }
        [JsonIgnore]
        public byte CurentPlayer { get; set; }
        public TicTacToeBoard Board { get; set; } = new TicTacToeBoard();

        private TicTacToeSummary summary;
        [JsonIgnore]
        public TicTacToeSummary Summary
        {
            get
            {
                return summary ?? (summary = new TicTacToeSummary(this));
            }
        }

        public TicTacToeState()
        {

        }

        public TicTacToeState(TicTacToeState state)
        {
            Players = state.Players;
            CurentPlayer = state.CurentPlayer;
            Board = new TicTacToeBoard { Board = state.Board.Board };
        }

        public TicTacToeState(TicTacToeSettings settings, TicTacToePlayer[] players)
        {
            Random random = new Random();
            CurentPlayer = settings.RandomStart ? (byte)random.Next(2) : (byte)1;
            players[CurentPlayer].PlayerType = TicTacToePlayerType.X;
            players[CurentPlayer == 0 ? 1 : 0].PlayerType = TicTacToePlayerType.O;
            Players = players;
        }

        public object Clone()
        {
            return new TicTacToeState(this);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return ((TicTacToeState)obj).Board.Board == Board.Board;
        }

        public override int GetHashCode()
        {
            return Board.Board;
        }
    }

    public class TicTacToeSummary : ISummary
    {
        public TicTacToeSummary(TicTacToeState state)
        {
            TicTacToePlayerType? score = state.Board.GetScore();
            if (score != null)
            {
                IsEnd = true;
                Winner = score.Value;
            }
        }

        public bool IsEnd { get; set; }
        public TicTacToePlayerType Winner { get; set; } = TicTacToePlayerType.No;
    }
}