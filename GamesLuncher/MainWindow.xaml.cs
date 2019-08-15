using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace GamesLuncher
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

        private void TicTacToeConsole_Click(object sender, RoutedEventArgs e)
        {
            StartGame("TicTacToeConsole");
        }

        private void TicTacToeWindow_Click(object sender, RoutedEventArgs e)
        {
            StartGame("TicTacToeWindow");
        }

        private void DraughtsConsole_Click(object sender, RoutedEventArgs e)
        {
            StartGame("DraughtsConsole");
        }

        private void StartGame(string v)
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{v}.exe")))
                Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{v}.exe"));
            else if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{v}.dll")))
                Process.Start("dotnet.exe", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{v}.dll"));
        }
    }
}
