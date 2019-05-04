using DraughtsCore;

namespace DraughtsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Draughts game = new Draughts { UserInterface = new DraughtsUI() };
            game.Start(new DraughtsSettings(), new DraughtsPlayerAI(), new DraughtsPlayer());
        }
    }
}