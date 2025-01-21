using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon.Primitives;

using System.Diagnostics;


using gomoku;
using gomoku_UI.Models;
namespace gomoku
{
    enum State
    {
        None = 0,
        enemy_two_step_draw = 35,
        self_two_step_draw = 36,
        enemy_one_step_to_win = 40,
        self_one_step_to_win = 41,
        enemy_one_step_draw = 45,
        self_one_step_draw = 46,
        enemy_win = 50,
        self_win = 51,
    }
    class Gomoku_MCTS:Gomoku_CPU_player
    {
        System.Random rand_step;
        string[] win_shape = { "OOOOO" };
        string[] one_step_draw_shape = { ".OOOO.", "OOO.O.OOO", "OO.OO.OO", "O.OOO.O" };
        string[] one_step_shape = { ".OOOO", "OOO.O.OOO", "OO.OO.OO", "O.OOO.O" };
        string[] two_step_draw_shape = { ".OOO.", ".OO.O.", ".O.O.O.O.", "OOO...OOO" };
        string[][] all_shapes;

        (State, State)[] all_states;

        Axis[] dir = {new Axis(1, 0), new Axis(-1, 0), new Axis(0, 1), new Axis(0, -1), new Axis(1, 1), new Axis(1, -1), new Axis(-1, 1), new Axis(-1, -1) };

        public Gomoku_MCTS() {
            rand_step = new System.Random();
            all_shapes = new string[][] { win_shape,   one_step_draw_shape, one_step_shape,        two_step_draw_shape };
            all_states = new (State, State)[]      { (State.self_win, State.enemy_win), 
                                                        (State.self_one_step_draw, State.enemy_one_step_draw),
                                                            (State.self_one_step_to_win, State.enemy_one_step_to_win),
                                                                (State.self_two_step_draw, State.enemy_two_step_draw) };
        }

        private bool type_cmp(Board board, int player, Axis pt, Axis diff, string shape)
        {
            for(int i = 0; i < shape.Length; i++)
            {
                Axis nx_pt = pt + i*diff;
                if (board.out_of_bound(nx_pt))
                    return false;
                if (shape[i] == 'O' && board[nx_pt] != player)
                    return false;
                if (shape[i] == '.' && board[nx_pt] != 0)
                    return false;
            }
            return true;
        }
        public State current_board_state(Board board, int player, Axis ref_pos)
        {   //if ref is assigned, then we only consider the positions which contain ref_pos
            //5: self win, 4:
           
            //record the maximal state
            State state = State.None;

            for (int state_idx = 0; state_idx < all_states.Length; state_idx++)
{
                foreach (string tp in all_shapes[state_idx])
                {
                    foreach (Axis d in dir)
                    {
                        for (int i = -tp.Length; i <= tp.Length; i++)
                        {
                            Axis corner_pt = ref_pos + i * d;
                            if (board.out_of_bound(corner_pt))
                                continue;
                            if (type_cmp(board, player, corner_pt, d, tp))
                            {
                                return all_states[state_idx].Item1;
                            }
                            else if (type_cmp(board, -player, corner_pt, d, tp))
                            {
                                state = all_states[state_idx].Item2;
                            }
                        }
                    }
                    if (state != 0)
                        return state;
                }
            }

            return 0;
        }

        public State global_board_state(Board board, int player)
        {   //if ref is assigned, then we only consider the positions which contain ref_pos
            //5: self win, 4:
            //if (ref_pos == null)
            //board.print_board();
            State mx_state = 0;
            for (int px = 0; px < board.board_size; px++)
            {
                for (int py = 0; py < board.board_size; py++)
                {
                    State cur_state = current_board_state(board, player, new Axis(px, py));
                    if (mx_state<cur_state){
                        mx_state = cur_state;
                    }
                    //Debug.Write(mx_state);
                    //Debug.Write(",");
                }
            }
            return mx_state;
        }

        public (int, int) get_critical_pt(Board board, int player)
        {   
            State mx_state = State.None;
            (int, int) critical_pt = (-1, -1);
            for (int px = 0; px < board.board_size; px++)
            {
                for (int py = 0; py < board.board_size; py++)
                {
                    if (board[px, py] != 0) 
                        continue;
                    board.add_stone(player, new Axis(px, py));
                    State cur_state = current_board_state(board, player, new Axis(px, py));
                    if (cur_state>mx_state){
                        mx_state = cur_state;
                        critical_pt = (px, py);
                    }
                    board.undo();
                }
            }
            //Debug.WriteLine(mx_state);
            return critical_pt;
        }

        private (int,int) get_rand_move(Board board, int player)
        {
            (int, int) move = get_critical_pt(board, player);
            if(move != (-1, -1))
            {
                return move;
            }
            int px = move.Item1, py=move.Item2;
            int cnt = 0;
            do
            {
                cnt++;
                py = rand_step.Next() % board.board_size;
                px = rand_step.Next() % board.board_size;
            }
            while (board[py,px] != 0);
            return (py,px);
        }

        public override (int,int) get_opt_move(Board board, int player)
        {
            Thread.Sleep(2000);
            return get_rand_move(board, player);
        }

    }

}
