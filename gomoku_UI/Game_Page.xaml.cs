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
    /// Interaction logic for Game_Page.xaml
    /// </summary>
    public partial class Game_Page : Page
    {
        public Game_Page()
        {
            InitializeComponent();
        }
        public Game_Page(int player_color, int AI_level)
        {
            InitializeComponent();
            this.DataContext = new ViewModel(player_color, AI_level);
        }
    }
}
