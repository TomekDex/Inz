using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public class Draughts : Game
    {
        public Draughts()
        {
            Judge = new DraughtsJudge();
            Moves = GetMoves();
        }

        private DraughtsMove[] GetMoves()
        {
            List<DraughtsMove> DraughtsMoves = new List<DraughtsMove>();
            for (byte x = 0; x < 5; x++)
                for (byte y = 0; y < 5; y++)
                    if ((x + y) % 2 == 0)
                    {
                        if (x != 0 && y != 0)
                            DraughtsMoves.Add(new DraughtsMove(x, y, -1, -1));
                        if (x != 0 && y != 4)
                            DraughtsMoves.Add(new DraughtsMove(x, y, -1, 1));
                        if (x != 4 && y != 0)
                            DraughtsMoves.Add(new DraughtsMove(x, y, 1, -1));
                        if (x != 4 && y != 4)
                            DraughtsMoves.Add(new DraughtsMove(x, y, 1, 1));
                    }
            return DraughtsMoves.ToArray();
        }
    }

    public class DraughtsMove : Move
    {
        public DraughtsMove()
        {

        }
        public DraughtsMove(byte fromX, byte fromY, short vectorX, short vectorY)
        {
            FromX = fromX;
            FromY = fromY;
            VectorX = vectorX;
            VectorY = vectorY;
        }

        public override string ToString()
        {
            return $"{FromX}.{FromY} --> {FromX + VectorX}.{FromY + VectorY}";
        }

        public byte FromX { get; }
        public byte FromY { get; }
        public short VectorX { get; }
        public short VectorY { get; }
        public bool Obligatory { get; set; }

        public override void Execute(State state, Player player)
        {
            DraughtsState stateD = state as DraughtsState;
            DraughtsPlayer playerD = player as DraughtsPlayer;
            if (stateD.Border[FromX + VectorX, FromY + VectorY] == default)
            {
                stateD.Border[FromX + VectorX, FromY + VectorY] = stateD.Border[FromX, FromY];
                stateD.Border[FromX, FromY] = default;
            }
            else
            {
                stateD.Border[FromX + VectorX * 2, FromY + VectorY * 2] = stateD.Border[FromX, FromY];
                stateD.Border[FromX, FromY] = default;
                stateD.Border[FromX + VectorX, FromY + VectorY] = default;
                stateD.CurentPlayer = (byte)((++stateD.CurentPlayer) % 2);
                playerD.NextInThisMove = true;
            }
        }

        public override bool IsAllowed(State state, Player player)
        {
            Obligatory = false;
            DraughtsState stateD = state as DraughtsState;
            DraughtsPlayer playerD = player as DraughtsPlayer;

            if (!playerD.NextInThisMove)
            {
                if (playerD.Char == 'B')
                {
                    if (VectorX == -1)
                        return false;
                }
                else if (VectorX == 1)
                    return false;
            }

            if (stateD.Border[FromX, FromY] != (player as DraughtsPlayer).Char)
                return false;
            if (stateD.Border[FromX + VectorX, FromY + VectorY] == default)
                return true;
            if (stateD.Border[FromX + VectorX, FromY + VectorY] == (player as DraughtsPlayer).Char)
                return false;

            int toX = FromX + VectorX * 2;
            int toY = FromY + VectorY * 2;
            if (toX > 0 && toX < 5 && toY > 0 && toY < 5 && stateD.Border[toX, toY] == default)
            {
                Obligatory = true;
                return true;
            }
            return false;
        }
    }

    public abstract class DraughtsPlayer : Player
    {
        public char Char { get; internal set; }
        public bool NextInThisMove { get; internal set; }
    }

    public class DraughtsPlayerAI : DraughtsPlayer
    {
        Random random = new Random();
        public override Move NextMove(State state, List<Move> moveAllowed)
        {
            return moveAllowed[random.Next(moveAllowed.Count)];
        }
    }

    public class DraughtsState : State
    {
        public DraughtsPlayer[] Players { get; }
        public byte CurentPlayer { get; set; }
        public char[,] Border { get; } = new char[5, 5];

        public DraughtsState(DraughtsPlayer[] players)
        {
            Random random = new Random();
            Summary = new DraughtsSummary();
            CurentPlayer = (byte)random.Next(2);
            players[CurentPlayer].Char = 'B';
            players[CurentPlayer == 0 ? 1 : 0].Char = 'W';
            Players = players;

            for (byte x = 0; x < 5; x++)
                for (byte y = 0; y < 5; y++)
                    if ((x + y) % 2 == 0 && x != 2)
                        Border[x, y] = x < 2 ? 'B' : 'W';
        }
    }

    public class DraughtsSummary : Summary
    {
        public char CharWinner { get { return CharLost == 'B' ? 'W' : 'B'; } }
        public char CharLost { get; set; }
    }

    internal class DraughtsJudge : Judge
    {
        public override List<Move> GetMoveAllowed(State state, Move[] moves, Player player)
        {
            List<Move> movesAllowed = base.GetMoveAllowed(state, moves, player);
            if (!movesAllowed.Any() || ((player as DraughtsPlayer).NextInThisMove && !movesAllowed.Any(a => (a as DraughtsMove).Obligatory)))
                return new List<Move> { new DraughtsNoMove() };

            if (movesAllowed.Any(a => (a as DraughtsMove).Obligatory))
                return movesAllowed.Where(a => (a as DraughtsMove).Obligatory).ToList();
            return movesAllowed;
        }

        public override bool IsNotEnd(State state)
        {
            return (state.Summary as DraughtsSummary).CharLost == default;
        }

        public override Player NextPlayer(State state)
        {
            DraughtsState DraughtsState = state as DraughtsState;
            DraughtsState.CurentPlayer = (byte)(DraughtsState.CurentPlayer == 0 ? 1 : 0);
            return DraughtsState.Players[DraughtsState.CurentPlayer];
        }

        public override State SetStartState(Player[] players)
        {
            if (players.Length != 2)
                throw new Exception("Bad number of players");

            return new DraughtsState(players as DraughtsPlayer[]);
        }

        public override void UpdateSummary(State state)
        {

        }
    }

    public class DraughtsNoMove : DraughtsMove
    {
        public override void Execute(State state, Player player)
        {
            DraughtsPlayer playerD = player as DraughtsPlayer;
            if (!playerD.NextInThisMove)
                (state.Summary as DraughtsSummary).CharLost = playerD.Char;
            else
                playerD.NextInThisMove = false;
        }

        public override bool IsAllowed(State state, Player player)
        {
            return true;
        }
    }
}
