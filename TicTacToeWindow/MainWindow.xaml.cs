using System.Windows;
using System.Windows.Controls;
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
            await game.Start(new TicTacToeSettings() { RandomStart = (RandomStart.IsChecked ?? false)}, GetPlayer(PlayerFirst.SelectedItem), GetPlayer(PlayerSecond.SelectedItem));
        }

        private TicTacToePlayer GetPlayer(object value)
        {
            if (((ComboBoxItem)value).Content.ToString() == "AI")
                return new TicTacToePlayerAI();
            else
                return new TicTacToePlayer();
        }
    }
}
