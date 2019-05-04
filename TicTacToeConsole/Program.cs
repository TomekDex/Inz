using System;
using TicTacToeCore;

namespace TicTacToeConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            TicTacToe game = new TicTacToe();
            game.UserInterface = new TicTacToeConsoleUserInterface();
            game.Start(new TicTacToeSettings(), new TicTacToePlayer(), new TicTacToePlayerAI());
        }
    }
}
