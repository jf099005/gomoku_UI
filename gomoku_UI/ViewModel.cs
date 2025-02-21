using gomoku;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using gomoku_UI.Models;
using System.Windows.Media;
using System.Windows;
using System.Runtime.InteropServices.Marshalling;
using System.Windows.Controls.Primitives;

namespace gomoku_UI
{
    public struct UI_stone
    {
        public int x { get; set; }
        public int y { get; set; }
        public Brush color { get; set; }
        public int diameter { get; set; }
        public UI_stone(int x, int y, int board_grid_width, int diameter, Player color, bool is_mouse_position = false)
        {
            this.x = x * board_grid_width - diameter / 2 + board_grid_width / 2;
            this.y = y * board_grid_width- diameter/2 + board_grid_width / 2;
            this.diameter = diameter;
            if (is_mouse_position && !(color == Player.None))
            {
                throw new Exception("ERROR: non-player move cannot be set into mouse position");
            }
            if (color == Player.Black)
                this.color = Brushes.Black;
            else if(color==Player.White)
                this.color = Brushes.White;
            else if(color == Player.None)
            {
                if (is_mouse_position)
                {
                    this.color = Brushes.DarkRed;
                }
                else
                {
                    throw new Exception("ERROR: invalid stone type:" + color.ToString());
                }
            }
        }
    }

    enum UI_state
    {
        ready,
        data_processing,
    }


    public class ViewModel : INotifyPropertyChanged
    {
        public int board_size = 15;
        public int board_grid_width = 53;
        public int stone_diameter = 35;
        int mouse_mark_diameter = 10;
        //private Board board;
        private Game_Agent agent;

        

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<UI_stone> _UI_board = new ObservableCollection<UI_stone>();

        public ObservableCollection<UI_stone> UI_board
        {
            get
            {
                return _UI_board;
            }
            set
            {
                _UI_board = value;
                OnPropertyChanged(nameof(UI_board));
            }
        }

        private Player player_color;
        public player_info_ViewModel PlayerInfo;


        //methods ---------------------------------------
        public ViewModel(int _player_color=1, int _ai_level=0)
        {
            agent = new Game_Agent(board_size, _player_color, _ai_level);
            player_color= (Player)_player_color;
            _current_step = 123;
            PlayerInfo = new player_info_ViewModel();
            if (player_color == Player.White)
                VM_cpu_move();
        }


        private void VM_add_stone(int px, int py, Player color)
        {
            if (!agent.is_valid_move(color, px, py))
            {
                throw new Exception("VM: invalid move");
                //return;
            }
            if (color == Player.None || color == agent.Current_Player)
            {
                color = agent.current_player_move(px, py);
            }
            else
            {
                agent.add_stone((int)color, new Axis(px, py));
            }
            UI_board.Add(new UI_stone(px, py, board_grid_width, stone_diameter, color));

            if(agent.Current_State == Game_State.endgame)
            {
                Debug.WriteLine("endgame, receive winner:{0}", agent.winner);
                _show_result_window( agent.winner );
            }
        }

        private void VM_add_mouse_position(int px, int py)
        {
            if (agent.out_of_bound(px,py))
            {
                throw new Exception("VM: invalid move");
            }
            for(int i = 0; i < UI_board.Count; i++)
            {
                if (UI_board[i].diameter == mouse_mark_diameter)
                {
                    UI_board.Remove(UI_board.Where(i=>i.diameter == mouse_mark_diameter).Single());
                    break;
                }
            }
            if (agent.out_of_bound(px, py))
            {
                return;
            }

            UI_board.Add(new UI_stone(px, py, board_grid_width, mouse_mark_diameter, Player.None, true));
        }
        private async Task VM_cpu_move()
        {
            (int px, int py) = await agent.get_cpu_move(agent.Current_Player);
            if(!agent.is_valid_move(agent.Current_Player, px, py))
            {
                throw new Exception("ERROR: cpu move at invalid position:" + px.ToString() + "," + py.ToString());
            }
            VM_add_stone(px, py, agent.Current_Player);
        }

        void board_mousemove(object obj)
        {
            if(obj is MouseEventArgs e)
            {

                //Debug.WriteLine("mouse moved");

                var canvas = e.Source as System.Windows.IInputElement;
                if (canvas is not null)
                {
                    double mouse_x = e.GetPosition(canvas).X;
                    double mouse_y = e.GetPosition(canvas).Y;
                    //Debug.WriteLine("position: ("+mouse_x.ToString()+","+mouse_y.ToString()+")");
                    int px = (int)(mouse_x / board_grid_width);
                    int py = (int)(mouse_y / board_grid_width);
                    if (!agent.out_of_bound(px, py))
                    {
                        VM_add_mouse_position(px, py);
                    }
                }

            }
            else
            {
                throw new Exception("ERROR occur on mouse-move");
            }
        }
        public ICommand Board_MouseMove
        {
            get
            {
                return new RelayCommand(e=>board_mousemove(e));
            }
        }

        void board_mousedown(object obj)
        {
            if (obj is MouseEventArgs e)
            {
                var canvas = e.Source as System.Windows.IInputElement;
                if (canvas is not null)
                {
                    double mouse_x = e.GetPosition(canvas).X;
                    double mouse_y = e.GetPosition(canvas).Y;
                    int px = (int)(mouse_x / board_grid_width);
                    int py = (int)(mouse_y / board_grid_width);
                    if (agent.is_valid_move(player_color, px, py))
                    {
                        VM_add_stone(px, py, agent.Current_Player);
                    }
                    else
                    {
                        Debug.WriteLine("invalid position:(" + px.ToString() + "," + py.ToString());
                        return;
                    }

                    if (agent.is_cpu_turn() && agent.Current_State != Game_State.endgame)
                    {
                        VM_cpu_move();
                    }
                }
            }
            else
            {
                throw new Exception("Error: obj is not MouseEventArgs");
            }
        }
        public ICommand Board_MouseDown
        {
            get
            {
                return new RelayCommand(e => board_mousedown(e));
            }
        }


        public int _current_step;
        public int current_step
        {
            get
            {
                return _current_step;
            }
            set
            {
                OnPropertyChanged("current_step");
                _current_step = value;
            }
        }

        public void _show_result_window(object e)
        {
            if (e is int winner)
            {
                Debug.WriteLine("show_result_window");
                Debug.WriteLine("winner:{0}", winner);
                Window result_window = new Game_Result_Window(winner);
                result_window.Show();
            }
        }

        public ICommand Show_Result
        {
            get
            {
                return new RelayCommand(e => _show_result_window(e));
            }
        }
    }
}
