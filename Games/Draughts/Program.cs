using Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Draughtss
{
    class Program
    {
        static void Main(string[] args)
        {
            Draughts Draughts = new Draughts();
            DraughtsState end = Draughts.Start(new DraughtsPlayer[] { new DraughtsPlayerAIConsol(DrowState), new DraughtsPlayerAIConsol(DrowState) }) as DraughtsState;
            DrowState(end);
            Console.SetCursorPosition(0, 7);
            char winner = (end.Summary as DraughtsSummary).CharWinner;
            if (winner == default)
                Console.WriteLine("Draw!!!!!");
            else
                Console.WriteLine($"{winner} Win !!!!!");
            Console.ReadKey();
        }

        public static void DrowState(DraughtsState state)
        {
            Console.Clear();
            for (byte x = 0; x < 5; x++)
                for (byte y = 0; y < 5; y++)
                    if (state.Border[x, y] != default)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(state.Border[x, y]);
                    }
        }

        public static DraughtsMove SelectMove(DraughtsState sate, DraughtsMove[] moves)
        {
            if (moves.First() is DraughtsNoMove)
                return moves.First();
            Console.SetCursorPosition(0, 7);
            Console.WriteLine($"Your turn {sate.Players[sate.CurentPlayer].Char}");
            for (int i = 0; i < moves.Length; i++)
                Console.WriteLine($"{i} {moves[i].FromX}.{moves[i].FromY} --> {moves[i].FromX + moves[i].VectorX}.{moves[i].FromY + moves[i].VectorY}");
            Console.WriteLine("SelectMove");
            do
            {
                if (int.TryParse(Console.ReadLine(), out int y))
                {
                    DraughtsMove move = moves[y];
                    if (move != null)
                        return move;
                }

            } while (true);
        }
    }

    internal class DraughtsPlayerAIConsol : DraughtsPlayerAI
    {
        private Action<DraughtsState> drowState;

        public DraughtsPlayerAIConsol(Action<DraughtsState> drowState)
        {
            this.drowState = drowState;
        }

        public override Move NextMove(State state, List<Move> moveAllowed)
        {
            Thread.Sleep(1000);
            drowState(state as DraughtsState);
            return base.NextMove(state, moveAllowed);
        }
    }

    internal class DraughtsPlayerConsol : DraughtsPlayer
    {
        private Action<DraughtsState> drowState;
        Func<DraughtsState, DraughtsMove[], DraughtsMove> selectMove;

        public DraughtsPlayerConsol(Action<DraughtsState> drowState, Func<DraughtsState, DraughtsMove[], DraughtsMove> selectMove)
        {
            this.drowState = drowState;
            this.selectMove = selectMove;
        }

        public override Move NextMove(State state, List<Move> moveAllowed)
        {
            drowState(state as DraughtsState);
            return selectMove(state as DraughtsState, moveAllowed.Select(a => a as DraughtsMove).ToArray());
        }
    }
}
