using Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToes
{
    class Program
    {
        static void Main(string[] args)
        {
            TicTacToe ticTacToe = new TicTacToe();
            TicTacToeState end = ticTacToe.Start(new TicTacToePlayer[] { new TicTacToePlayerAIConsol(DrowState), new TicTacToePlayerAIConsol(DrowState) }) as TicTacToeState;
            DrowState(end);
            Console.SetCursorPosition(0, 7);
            char winner = (end.Summary as TicTacToeSummary).CharWinner;
            if (winner == default)
                Console.WriteLine("Draw!!!!!");
            else
                Console.WriteLine($"{winner} Win !!!!!");
            Console.ReadKey();
        }

        public static void DrowState(TicTacToeState state)
        {
            Console.Clear();
            for (byte x = 0; x < 3; x++)
                for (byte y = 0; y < 3; y++)
                    if (state.Border[x, y] != default)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(state.Border[x, y]);
                    }
        }

        public static TicTacToeMove SelectMove(TicTacToeState sate, TicTacToeMove[] moves)
        {
            Console.SetCursorPosition(0, 7);
            Console.WriteLine($"Your turn {sate.Players[sate.CurentPlayer].Char}");
            do
            {
                string[] input = Console.ReadLine().Split(' ');
                if (input.Length > 1 && int.TryParse(input[0], out int x) && int.TryParse(input[1], out int y))
                {
                    TicTacToeMove move = moves.FirstOrDefault(m => m.X == x && m.Y == y);
                    if (move != null)
                        return move;
                }

            } while (true);
        }
    }

    internal class TicTacToePlayerAIConsol : TicTacToePlayerAI
    {
        private Action<TicTacToeState> drowState;

        public TicTacToePlayerAIConsol(Action<TicTacToeState> drowState)
        {
            this.drowState = drowState;
        }

        public override Move NextMove(State state, List<Move> moveAllowed)
        {
            drowState(state as TicTacToeState);
            return base.NextMove(state, moveAllowed);
        }
    }

    internal class TicTacToePlayerConsol : TicTacToePlayer
    {
        private Action<TicTacToeState> drowState;
        Func<TicTacToeState, TicTacToeMove[], TicTacToeMove> selectMove;

        public TicTacToePlayerConsol(Action<TicTacToeState> drowState, Func<TicTacToeState, TicTacToeMove[], TicTacToeMove> selectMove)
        {
            this.drowState = drowState;
            this.selectMove = selectMove;
        }

        public override Move NextMove(State state, List<Move> moveAllowed)
        {
            drowState(state as TicTacToeState);
            return selectMove(state as TicTacToeState, moveAllowed.Select(a => a as TicTacToeMove).ToArray());
        }
    }
}
