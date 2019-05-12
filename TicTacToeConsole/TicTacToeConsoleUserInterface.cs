using GamesCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToeCore;

namespace TicTacToeConsole
{
    internal class TicTacToeConsoleUserInterface : IUserInterface<TicTacToeState, TicTacToeMove, TicTacToePlayer, TicTacToeSummary, TicTacToeAction>
    {
        public async Task ShowEndState(TicTacToeState state)
        {
            ShowState(state);
            Console.SetCursorPosition(0, 7);
            TicTacToePlayerType winner = state.Summary.Winner;
            if (winner == TicTacToePlayerType.No)
                Console.WriteLine("Draw!!!!!");
            else
                Console.WriteLine($"{winner} Win !!!!!");
        }

        public async Task<TicTacToeMove> ShowSelectionMove(TicTacToeState state, TicTacToePlayer player, List<TicTacToeMove> allowedMoves)
        {
            Console.SetCursorPosition(0, 7);
            Console.WriteLine($"Your turn {player.PlayerType}");
            do
            {
                string[] input = Console.ReadLine().Split(' ');
                if (input.Length > 1 && int.TryParse(input[0], out int x) && int.TryParse(input[1], out int y))
                {
                    TicTacToeMove move = allowedMoves.FirstOrDefault(m => m.Actions[0].X == x && m.Actions[0].Y == y);
                    if (move != null)
                        return move;
                }

            } while (true);
        }

        public async Task ShowState(TicTacToeState state)
        {
            Console.Clear();
            for (byte x = 0; x < 3; x++)
                for (byte y = 0; y < 3; y++)
                    if (state.Board[x, y] != default)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(state.Board[x, y]);
                    }
        }
    }
}