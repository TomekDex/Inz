using TicTacToeCore;

namespace TicTacToeConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            TicTacToe game = new TicTacToe();
            game.UserInterface = new TicTacToeConsoleUserInterface();
            while (true)
            {
                game.Start(new TicTacToeSettings(), new TicTacToePlayerAI(), new TicTacToePlayerAI());
            }

        }
    }
}