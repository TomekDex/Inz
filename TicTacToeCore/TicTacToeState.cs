using GamesCore;
using System;

namespace TicTacToeCore
{
    public class TicTacToeState : IState<TicTacToeSummary>
    {
        public TicTacToePlayer[] Players { get; }
        public byte CurentPlayer { get; set; }
        public TicTacToePlayerType?[,] Board { get; } = new TicTacToePlayerType?[3, 3];

        private TicTacToeSummary summary;
        public TicTacToeSummary Summary
        {
            get
            {
                return summary ?? (summary = new TicTacToeSummary(this));
            }
        }

        public TicTacToeState(TicTacToeState state)
        {
            Players = state.Players;
            CurentPlayer = state.CurentPlayer;
            Board = (TicTacToePlayerType?[,])state.Board.Clone();
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

        public override string ToString()
        {
            string s = "Win: "+ Summary.Winner + "\r\n";
            for (int j = 0; j < 3; j++)
            {
                s += "\r\n";
                for (int i = 0; i < 3; i++)
                    s += Board[i, j]?.ToString() ?? " ";
            }
            return s;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var objState = (TicTacToeState)obj;

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (objState.Board[i, j] != Board[i, j])
                        return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = Board.Length;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    hash = unchecked(hash * 17 + Board[i, j].GetHashCode());
            return hash;
        }
    }

    public class TicTacToeSummary : ISummary
    {
        public TicTacToeSummary(TicTacToeState state)
        {
            for (int i = 0; i < 3; i++)
            {
                if (state.Board[i, 0] != default && state.Board[i, 0] == state.Board[i, 1] && state.Board[i, 0] == state.Board[i, 2])
                {
                    IsEnd = true;
                    Winner = state.Board[i, 0];
                    return;
                }
                if (state.Board[0, i] != default && state.Board[0, i] == state.Board[1, i] && state.Board[0, i] == state.Board[2, i])
                {
                    IsEnd = true;
                    Winner = state.Board[0, i];
                    return;
                }
            }
            if (state.Board[0, 0] != default && state.Board[0, 0] == state.Board[1, 1] && state.Board[0, 0] == state.Board[2, 2])
            {
                IsEnd = true;
                Winner = state.Board[0, 0];
                return;
            }
            if (state.Board[0, 2] != default && state.Board[0, 2] == state.Board[1, 1] && state.Board[0, 2] == state.Board[2, 0])
            {
                IsEnd = true;
                Winner = state.Board[0, 2];
                return;
            }
        }

        public bool IsEnd { get; set; }
        public TicTacToePlayerType? Winner { get; set; }
    }
}
