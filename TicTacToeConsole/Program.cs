using System;
using TicTacToeCore;

namespace TicTacToeConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            TicTacToeGame game = new TicTacToeGame();
            game.UserInterface = new TicTacToeConsoleUserInterface();
            game.Start(new[] { new TicTacToePlayer(), new TicTacToePlayerAI() });
        }
    }
}
