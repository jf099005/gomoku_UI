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
namespace gomoku_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel _View;

        enum User_State
        {
            gaming,
            endgame,
            discuss,
        }

        User_State current_state = User_State.gaming;

        public int board_size = 10;
        public int board_grid_width = 40;
        private int current_player;
        private int[] players;//0: cpu, 1:human
        private Board board;
        private Gomoku_MCTS game;
        public MainWindow()
        {
            //_View = (ViewModel)base.DataContext;
            _View = new ViewModel();
            this.DataContext = _View;

            board = new Board((short)board_size);
            InitializeComponent();
            game = new Gomoku_MCTS();
            players = new int[]{1,0};
            current_player = 1;
        }

        private async void PutStone_event(object sender, MouseEventArgs e)
        {
            //if (players[current_player] != 1)
            //    return;
            if (current_state == User_State.endgame) 
            {
                Debug.WriteLine("game is ended");
                return;
            }
            if(current_state == User_State.gaming)
            {
                if(e.RightButton == MouseButtonState.Pressed)
                {
                    return;
                }
                if (current_player != 1)
                    return;
            }
            Point position = Mouse.GetPosition(GameBoard);
            int px = (int)(position.X / board_grid_width);
            int py = (int)(position.Y / board_grid_width);
            if (board[px, py] != 0)
                return;
            if (e.LeftButton == MouseButtonState.Pressed)
                current_player = 1;

            if (e.RightButton == MouseButtonState.Pressed)
                current_player = -1;

            add_stone(px, py, current_player);
            Debug.WriteLine("current_step:{0}", _View.current_step);
            if (board.end_game())
            {
                eng_game();
            }
            if (current_state == User_State.gaming){
                next_round();
                await cpu_move();
                next_round();
            }
            return;
        }

        private void eng_game() {
            current_state = User_State.endgame;
        }
        private void next_round()
        {
            current_player = -current_player;
        }
        private async void cpu_move(object sender, RoutedEventArgs e)
        {
            await cpu_move();
        }
        async private Task cpu_move()
        {
            //if (players[current_player] != 0)
            //    return;
            //current_player = -1;
            //current_player = -1;
            Debug.WriteLine("player:" + current_player);

            Task<(int, int)> get_move = Task.Run(()=>game.get_opt_move(board, current_player));
            (int, int) pt = await get_move;
            Debug.WriteLine(pt+",player:"+current_player);
            add_stone(pt, current_player);
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
            _View.current_step++;

            if (board.end_game())
            {

            }
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
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem data = Tab.SelectedItem as TabItem;

            if (data.Header == versusCPU.Header)
            {
                current_state = User_State.gaming;
            }
            else if(data.Header == test.Header)
            {
                current_state = User_State.discuss;
            }
        }

    }
}