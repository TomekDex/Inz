using GamesCore;
using System;
using System.Collections.Generic;

namespace TicTacToeCore
{
    public enum TicTacToePlayerType
    {
        X,
        O
    }

    public class TicTacToe : Game<TicTacToeState, TicTacToeMove, TicTacToePlayer, TicTacToeSummary, TicTacToeJudge, TicTacToeAction>
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

    public class TicTacToeState : IState<TicTacToeSummary>
    {
        public TicTacToePlayer[] Players { get; }
        public byte CurentPlayer { get; set; }
        public TicTacToePlayerType?[,] Border { get; } = new TicTacToePlayerType?[3, 3];

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
            Border = (TicTacToePlayerType?[,])state.Border.Clone();
        }

        public TicTacToeState(TicTacToePlayer[] players)
        {
            Random random = new Random();
            CurentPlayer = (byte)random.Next(2);
            players[CurentPlayer].PlayerType = TicTacToePlayerType.X;
            players[CurentPlayer == 0 ? 1 : 0].PlayerType = TicTacToePlayerType.O;
            Players = players;
        }

        public object Clone()
        {
            return new TicTacToeState(this);
        }
    }

    public class TicTacToeSummary : ISummary
    {
        public TicTacToeSummary(TicTacToeState state)
        {
            for (int i = 0; i < 3; i++)
            {
                if (state.Border[i, 0] != default && state.Border[i, 0] == state.Border[i, 1] && state.Border[i, 0] == state.Border[i, 2])
                {
                    IsEnd = true;
                    Winner = state.Border[i, 0];
                    return;
                }
                if (state.Border[0, i] != default && state.Border[0, i] == state.Border[1, i] && state.Border[0, i] == state.Border[2, i])
                {
                    IsEnd = true;
                    Winner = state.Border[0, i];
                    return;
                }
            }
            if (state.Border[0, 0] != default && state.Border[0, 0] == state.Border[1, 1] && state.Border[0, 0] == state.Border[2, 2])
            {
                IsEnd = true;
                Winner = state.Border[0, 0];
                return;
            }
            if (state.Border[0, 2] != default && state.Border[0, 2] == state.Border[1, 1] && state.Border[0, 2] == state.Border[2, 0])
            {
                IsEnd = true;
                Winner = state.Border[0, 2];
                return;
            }
        }

        public bool IsEnd { get; set; }
        public TicTacToePlayerType? Winner { get; set; }
    }
}
