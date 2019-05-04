using System.Windows;
using TicTacToeCore;

namespace TicTacToeWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            TicTacToe game = new TicTacToe();
            game.UserInterface = new TicTacToeUIWindow();
            await game.Start(new TicTacToeSettings(), new TicTacToePlayer(), new TicTacToePlayerAI());
        }
    }
}
