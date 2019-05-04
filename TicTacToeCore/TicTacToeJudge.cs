using GamesCore;
using System;
using System.Collections.Generic;

namespace TicTacToeCore
{
    public class TicTacToeJudge : IJudge<TicTacToeState, TicTacToeMove, TicTacToePlayer, TicTacToeSummary, TicTacToeAction, TicTacToeSettings>
    {
        public List<TicTacToeMove> GetAllowedMoves(TicTacToeState state, TicTacToePlayer player)
        {
            List<TicTacToeMove> moves = new List<TicTacToeMove>();
            for (byte x = 0; x < 3; x++)
                for (byte y = 0; y < 3; y++)
                {
                    TicTacToeAction action = new TicTacToeAction(x, y, player.PlayerType, state);
                    if (action.IsAllowed)
                        moves.Add(new TicTacToeMove(action, state));
                }
            return moves;
        }

        public bool IsNotEnd(TicTacToeState state)
        {
            for (byte x = 0; x < 3; x++)
                for (byte y = 0; y < 3; y++)
                    if (state.Board[x, y] == default)
                        return !state.Summary.IsEnd;
            return false;
        }

        public TicTacToePlayer NextPlayer(TicTacToeState state)
        {
            state.CurentPlayer = (byte)(state.CurentPlayer == 0 ? 1 : 0);
            return state.Players[state.CurentPlayer];
        }

        public TicTacToeState SetStartState(TicTacToeSettings settings, TicTacToePlayer[] players)
        {
            if (players.Length != 2)
                throw new Exception("Bad number of players");

            return new TicTacToeState(settings, players);
        }
    }
}