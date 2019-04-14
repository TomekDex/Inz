using System;
using System.Collections.Generic;

namespace Games
{
    public abstract class State
    {
        public Summary Summary { get; set; }
    }
    public abstract class Game
    {
        public Judge Judge { get; set; }
        public Move[] Moves { get; set; }
        public State Start(Player[] players)
        {
            State state = Judge.SetStartState(players);
            while (Judge.IsNotEnd(state))
            {
                Player player = Judge.NextPlayer(state);
                List<Move> moveAllowed = Judge.GetMoveAllowed(state, Moves, player);
                Move nextMove = player.NextMove(state, moveAllowed);
                nextMove.Execute(state, player);
                Judge.UpdateSummary(state);
            }
            return state;
        }
    }

    public abstract class Summary
    {
    }

    public abstract class Move
    {
        public abstract bool IsAllowed(State state, Player player);
        public abstract void Execute(State state, Player player);
    }

    public abstract class Judge
    {
        public abstract State SetStartState(Player[] players);
        public abstract bool IsNotEnd(State game);
        public abstract Player NextPlayer(State state);
        public abstract void UpdateSummary(State state);

        internal List<Move> GetMoveAllowed(State state, Move[] moves, Player player)
        {
            List<Move> movesAllowed = new List<Move>();
            foreach (Move move in moves)
            {
                if (move.IsAllowed(state, player))
                    movesAllowed.Add(move);
            }
            return movesAllowed;
        }
    }

    public abstract class Player
    {
        public abstract Move NextMove(State state, List<Move> moveAllowed);
    }
}