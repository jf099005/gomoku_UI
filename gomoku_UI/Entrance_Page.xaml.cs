using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using gomoku;
using gomoku_UI.Models;
namespace gomoku_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Entrance : Page
    {
        public int Player_Stone_Color = 1;
        public int AI_Level = 0;

        public Entrance()
        {
            InitializeComponent();
        }

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            Information_Window start_Page = new Information_Window();
            start_Page.StartGame += get_information;
            start_Page.Show();

        }

        void get_information(object sender, EventArgs e)
        {
            Information_Window start_Page = (Information_Window)sender;
            Player_Stone_Color = start_Page.Player_Color.SelectedIndex==0?1:-1;
            AI_Level = start_Page.AI_Level.SelectedIndex;
            Game_Page game_Page = new Game_Page(Player_Stone_Color, AI_Level);
            this.NavigationService.Navigate(game_Page);
        }
    }
}