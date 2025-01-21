using gomoku;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gomoku_UI.Models;
using System.Windows.Controls;
namespace gomoku_UI.Models
{

    internal class Game_Agent
    {
        private Dictionary<Player, Player_Type> player_type = new Dictionary<Player, Player_Type>();
        public readonly int board_size = 10;
        
        private Game_State current_state;
        public Game_State Current_State
        {
            get
            {
                return current_state;
            }
        }

        private Board board;
        private Gomoku_CPU_player cpu_player;
        private  Player current_player;
        public Player Current_Player
        {
            get {
                return current_player;
            }
        }
        public Game_Agent(int player_color= 1, int ai_level=0) {
            if(player_color!= 1 && player_color != -1)
            {
                //Debug.WriteLine("Error: invalid player color");
                throw new Exception("Error: invalid player color");
                return;
            }
            current_state = Game_State.gaming;
            current_player = Player.Black;
            board = new Board(board_size);
            player_type[(Player)player_color] = Player_Type.Human;
            player_type[(Player)(-player_color)] = Player_Type.CPU;
            cpu_player = new Gomoku_MCTS();

            Debug.WriteLine("start with player color:" + player_color);
        }


        public bool is_valid_move(int px, int py)
        {
            switch (current_state)
            {
                case Game_State.gaming:
                    return board[px, py] == 0;
                default:
                    return false;
            }
        }
        public bool is_valid_move(Player color, int px, int py)
        {
            if(color==Player.None)
                return is_valid_move(px, py);
            switch (current_state)
            {
                case Game_State.gaming:
                    return board[px, py] == 0 && current_player==color;
                default:
                    return false;
            }
        }
        public Player current_player_move(int px, int py)
        {
            if (board[px,py]==0 &&  current_state!= Game_State.endgame)
            {
                if(current_player == Player.None)
                {
                    Debug.WriteLine("Error: invalid player");
                    return Player.None;
                }
                board.add_stone((int)current_player, new Axis(px, py));
                if (current_state == Game_State.gaming)
                    check_endgame();
                Player add_stone_Player = current_player;
                gaming_next_round();
                return add_stone_Player;
            }
            Debug.WriteLine("Error: invalid move");
            return Player.None;
        }
    
        public bool add_stone(int color, Axis pt)
        {
            if (board[pt.x, pt.y] == 0 && current_state != Game_State.endgame)
            {
                board.add_stone(color, pt);
                return true;
            }
            return false;
        }

        bool check_endgame()
        {
            Debug.WriteLine("check_endgame");
            if (board.is_endgame())
            {
                Debug.WriteLine("endgame");
                current_state = Game_State.endgame;
                return true;
            }
            Debug.WriteLine("not endgame");
            board.print_board();

            return false;
        }

        private void change_color()
        {
            if (current_player == Player.Black)
                current_player = Player.White;
            else if (current_player == Player.White)
                current_player = Player.Black;
        }
        private void gaming_next_round()
        {
            change_color();
        }
        public bool is_cpu_turn()
        {
            return (player_type[current_player] == Player_Type.CPU);
        }
        public async Task<(int, int)> get_cpu_move(Player player)
        {
            Task<(int,int)> cpu_calculate = Task.Run( ()=>cpu_player.get_opt_move(board, (int)player) );
            (int,int) pt = await cpu_calculate;
            return pt;
        }
        
    }
}
