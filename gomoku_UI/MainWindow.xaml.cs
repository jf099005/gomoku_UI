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
using gomoku_UI.Models.Axis;
namespace test2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int board_size = 6;
        public int board_grid_width = 65;
        private int current_player;
        private int current_player_type;
        private int[] players;//0: cpu, 1:human
        private Board board;
        private Gomoku_MCTS game;
        public MainWindow()
        {
            board = new Board((short)board_size);
            InitializeComponent();
            game = new Gomoku_MCTS();
            players = new int[]{1,0};
            current_player = 0;
        }
        private void PutStone_event(object sender, MouseEventArgs e)
        {
            //if (players[current_player] != 1)
            //    return;

            Point position = Mouse.GetPosition(GameBoard);
            int px = (int)(position.X / board_grid_width);
            int py = (int)(position.Y / board_grid_width);

            if (e.LeftButton == MouseButtonState.Pressed)
                current_player = 1;

            if (e.RightButton == MouseButtonState.Pressed)
                current_player = -1;

            add_stone(px, py, current_player);

            next_round();
            //if (players[current_player] == 0)
                //cpu_move();
            return;
        }

        private void next_round()
        {
            current_player = 1 - current_player;
        }
        private void cpu_move(object sender, RoutedEventArgs e)
        {
            //if (players[current_player] != 0)
            //    return;
            current_player = -1;
            (int,int) pt = game.get_opt_move(board, current_player);

            Debug.WriteLine(pt);

            add_stone(pt, current_player);
            board.add_stone(current_player, pt);
            next_round();
        }
        private void add_stone((int,int) pt, int player)
        {
            add_stone(pt.Item1, pt.Item2, player);
        }
        private void add_stone(int px, int py, int player)//color 0: white, 1:black
        {
            board.add_stone(player, [px,py]);
            var stone_color = player==1?Brushes.Black:Brushes.White;
            Ellipse stone = new Ellipse
            {
                Width = board_grid_width * 0.8,
                Height = board_grid_width * 0.8,
                Fill = stone_color
            };
            Debug.Write("add stone\n");

            Canvas.SetTop(stone, py* board_grid_width);
            Canvas.SetLeft(stone, px* board_grid_width);
            GameBoard.Children.Add(stone);
        }

        private void Restart(object sender, RoutedEventArgs e)
        {
            Debug.Write("restart\n");
            GameBoard.Children.Clear();
            board.clear_board();
        }

        private void calculate_state(object sender, RoutedEventArgs e)
        {
            Axis prv = board.latest_move;
            Debug.WriteLine("latest:({0},{1})", prv.x, prv.y);
            //int state = game.global_board_state(board, 1);
            Debug.WriteLine("current state is");
            Debug.WriteLine(game.global_board_state(board, 1));
            Debug.WriteLine(game.current_board_state(board, 1, prv));
            Debug.WriteLine("------------------------");
            Debug.WriteLine(game.global_board_state(board, -1));
            Debug.WriteLine(game.current_board_state(board, -1, prv));

            //Debug.WriteLine();
        }

        private void print_board(object sender, RoutedEventArgs e)
        {
            board.print_board();
        }
    }
}