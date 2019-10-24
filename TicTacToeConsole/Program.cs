using System;
using TicTacToeCore;

namespace TicTacToeConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            TicTacToePlayer playerFirst = SetPlayer("First");
            TicTacToePlayer playerSecond = SetPlayer("Second");
            bool random = SetRandom();
            TicTacToe game = new TicTacToe();
            game.UserInterface = new TicTacToeConsoleUserInterface();
            game.Start(new TicTacToeSettings { RandomStart = random }, playerFirst, playerSecond);
            Console.ReadKey();
        }

        private static TicTacToePlayer SetPlayer(string vlaue)
        {
            Console.Clear();
            Console.WriteLine($"Do you want to control the {vlaue} player?");
            if (Console.ReadKey().Key == ConsoleKey.Y)
                return new TicTacToePlayerAI();
            else
                return new TicTacToePlayer();
        }

        private static bool SetRandom()
        {
            Console.Clear();
            Console.WriteLine("Random Start?");
            return Console.ReadKey().Key == ConsoleKey.Y;
        }
    }
}