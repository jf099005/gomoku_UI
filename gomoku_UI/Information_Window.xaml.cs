using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace gomoku_UI
{
    /// <summary>
    /// Interaction logic for Start_Menu.xaml
    /// </summary>
    public partial class Information_Window : Window
    {
        public event EventHandler StartGame;
        public Information_Window()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame?.Invoke(this, EventArgs.Empty);
            this.Close();
        }
    }
}
