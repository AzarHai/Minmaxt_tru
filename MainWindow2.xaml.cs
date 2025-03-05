using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Minmaxt_tru
{
    public partial class MainWindow2 : Window
    {
        public MainWindow2()
        {
            InitializeComponent();
        }

        private void OpenMainWindow_Click(object sender, RoutedEventArgs e)
        {
            MinimaxApp.MainWindow1 mainWindow = new MinimaxApp.MainWindow1();
            mainWindow.Show();
            this.Close(); // Закрываем текущее окно
        }

        private void OpenMinimaxAlphaBeta_Click(object sender, RoutedEventArgs e)
        {
            MinimaxAlphaBeta.MainWindow alphaBetaWindow = new MinimaxAlphaBeta.MainWindow();
            alphaBetaWindow.Show();
            this.Close();
        }
    }
}
