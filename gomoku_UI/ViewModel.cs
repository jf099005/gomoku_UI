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

namespace gomoku_UI
{
    public struct UI_stone
    {
        public int x { get; set; }
        public int y { get; set; }
        public Brush color { get; set; }
        public UI_stone(int x, int y, int board_grid_width)
        {
            this.x = x*board_grid_width;
            this.y = y*board_grid_width;
            color = Brushes.Black;
        }
        public UI_stone(int x, int y, int board_grid_width, Player color)
        {
            this.x = x * board_grid_width;
            this.y = y * board_grid_width;
            if (color == Player.Black)
                this.color = Brushes.Black;
            else
                this.color = Brushes.White;
        }

    }

    enum UI_state
    {
        ready,
        data_processing,

    }
    public class ViewModel : INotifyPropertyChanged
    {
        public int board_size = 10;
        public int board_grid_width = 40;
        public double stone_diameter = 20;
        private UI_state io_state;

        //private Board board;
        private Game_Agent agent;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
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

        //methods ---------------------------------------
        public ViewModel()
        {
            agent = new Game_Agent();
            _current_step = 123;
            io_state = UI_state.ready;
        }

        private void VM_add_stone(int px, int py, Player color = Player.None)
        {
            if (!agent.is_valid_move(color, px,py))
            {
                return;
            }
            if (color == Player.None || color==agent.Current_Player)
            {
                color = agent.current_player_move(px, py);
            }
            else
            {
                agent.add_stone((int)color, new Axis(px, py));
            }
            UI_board.Add(new UI_stone(px,py,board_grid_width,color));
        }
        private async Task VM_cpu_move()
        {
            (int px,int py) = await agent.get_cpu_move( agent.Current_Player );
            VM_add_stone(px, py);
        }

        void board_mousedown(object obj)
        {
            if (obj is MouseEventArgs e)
            {
                var canvas = e.Source as System.Windows.IInputElement;
                if(canvas is not null)
                {
                    double mouse_x = e.GetPosition(canvas).X;
                    double mouse_y = e.GetPosition(canvas).Y;
                    int px = (int)(mouse_x / board_grid_width);
                    int py = (int)(mouse_y / board_grid_width);
                    if (agent.is_valid_move(px, py))
                    {
                        VM_add_stone(px, py);
                    }
                    else
                    {
                        return;
                    }

                    if (agent.is_cpu_turn())
                    {
                        VM_cpu_move();
                    }
                }
            }
            else
            {
                Debug.WriteLine("obj is not MouseEventArgs");
            }
        }
        public ICommand Board_MouseDown
        {
            get
            {
                return new RelayCommand(e=>board_mousedown(e) );
            }
        }


        public int _current_step;
        public int current_step
        {
            get
            {
                return  _current_step;
            }
            set
            {
                RaisePropertyChanged("current_step");
                _current_step = value;
            }
        }

    }
}
