using GamesCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TicTacToeCore;

namespace TicTacToeWindow
{
    /// <summary>
    /// Interaction logic for TicTacToeUIWindow.xaml
    /// </summary>
    public partial class TicTacToeUIWindow : Window, IUserInterface<TicTacToeState, TicTacToeMove, TicTacToePlayer, TicTacToeSummary, TicTacToeAction>
    {
        readonly ButtonAwait[,] board = new ButtonAwait[3, 3];
        public TicTacToeUIWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    board[i, j] = new ButtonAwait();
                    Grid.SetColumn(board[i, j], i);
                    Grid.SetRow(board[i, j], j);
                    MainGrid.Children.Add(board[i, j]);
                }
            Show();
        }

        public async Task ShowEndState(TicTacToeState state)
        {
            await ShowState(state);
            if (state.Summary.Winner == default)
                MessageBox.Show("Draw!!!!!");
            else
                MessageBox.Show($"{state.Summary.Winner} Win !!!!!");
        }

        public async Task<TicTacToeMove> ShowSelectionMove(TicTacToeState state, TicTacToePlayer player, List<TicTacToeMove> allowedMoves)
        {
            Task[] tasks = allowedMoves.Select(GetTask).ToArray();
            int index = await Task.Run(() => Task.WaitAny(tasks));
            return allowedMoves[index];
        }

        private async Task GetTask(TicTacToeMove move)
        {
            await board[move.Actions[0].X, move.Actions[0].Y];
        }

        public async Task ShowState(TicTacToeState state)
        {
            for (byte x = 0; x < 3; x++)
                for (byte y = 0; y < 3; y++)
                    if (state.Board[x, y] != default)
                        board[x, y].Content = state.Board[x, y];
        }
    }
}