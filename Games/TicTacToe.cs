using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public class TicTacToe : Game
    {
        public TicTacToe()
        {
            Judge = new TicTacToeJudge();
            Moves = GetMoves();
        }

        private TicTacToeMove[] GetMoves()
        {
            List<TicTacToeMove> ticTacToeMoves = new List<TicTacToeMove>();
            for (byte x = 0; x < 3; x++)
                for (byte y = 0; y < 3; y++)
                    ticTacToeMoves.Add(new TicTacToeMove(x, y));
            return ticTacToeMoves.ToArray();
        }
    }

    public class TicTacToeMove : Move
    {
        public TicTacToeMove(byte x, byte y)
        {
            X = x;
            Y = y;
        }

        public byte X { get; }
        public byte Y { get; }
        public override void Execute(State state, Player player)
        {
            (state as TicTacToeState).Border[X, Y] = (player as TicTacToePlayer).Char;
        }

        public override bool IsAllowed(State state, Player player)
        {
            return (state as TicTacToeState).Border[X, Y] == default;
        }
    }

    public abstract class TicTacToePlayer : Player
    {
        public char Char { get; internal set; }
    }

    public class TicTacToePlayerAI : TicTacToePlayer
    {
        Random random = new Random();
        public override Move NextMove(State state, List<Move> moveAllowed)
        {
            return moveAllowed[random.Next(moveAllowed.Count)];
        }
    }

    public class TicTacToeState : State
    {
        public TicTacToePlayer[] Players { get; }
        public byte CurentPlayer { get; set; }
        public char[,] Border { get; } = new char[3, 3];

        public TicTacToeState(TicTacToePlayer[] players)
        {
            Random random = new Random();
            Summary = new TicTacToeSummary();
            CurentPlayer = (byte)random.Next(2);
            players[CurentPlayer].Char = 'X';
            players[CurentPlayer == 0 ? 1 : 0].Char = 'O';
            Players = players;
        }
    }

    public class TicTacToeSummary : Summary
    {
        public char CharWinner { get; set; }
    }

    internal class TicTacToeJudge : Judge
    {
        bool isEnd;
        public override bool IsNotEnd(State state)
        {
            char[,] border = (state as TicTacToeState).Border;
            for (byte x = 0; x < 3; x++)
                for (byte y = 0; y < 3; y++)
                    if (border[x, y] == default)
                        return !isEnd;
            return false;
        }

        public override Player NextPlayer(State state)
        {
            TicTacToeState ticTacToeState = state as TicTacToeState;
            ticTacToeState.CurentPlayer = (byte)(ticTacToeState.CurentPlayer == 0 ? 1 : 0);
            return ticTacToeState.Players[ticTacToeState.CurentPlayer];
        }

        public override State SetStartState(Player[] players)
        {
            if (players.Length != 2)
                throw new Exception("Bad number of players");

            return new TicTacToeState(players as TicTacToePlayer[]);
        }

        public override void UpdateSummary(State state)
        {
            char[,] border = (state as TicTacToeState).Border;
            TicTacToeSummary ticTacToeSummary = state.Summary as TicTacToeSummary;
            for (int i = 0; i < 3; i++)
            {
                if (border[i, 0] != default && border[i, 0] == border[i, 1] && border[i, 0] == border[i, 2])
                {
                    isEnd = true;
                    ticTacToeSummary.CharWinner = border[i, 0];
                    return;
                }
                if (border[0, i] != default && border[0, i] == border[1, i] && border[0, i] == border[2, i])
                {
                    isEnd = true;
                    ticTacToeSummary.CharWinner = border[0, i];
                    return;
                }
            }
            if (border[0, 0] != default && border[0, 0] == border[1, 1] && border[0, 0] == border[2, 2])
            {
                isEnd = true;
                ticTacToeSummary.CharWinner = border[0, 0];
                return;
            }
            if (border[0, 2] != default && border[0, 2] == border[1, 1] && border[0, 2] == border[2, 0])
            {
                isEnd = true;
                ticTacToeSummary.CharWinner = border[0, 2];
                return;
            }
        }
    }
}
