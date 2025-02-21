using gomoku;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace gomoku_UI.Models
{
    class Negamax: Gomoku_CPU_player
    {
        const string dll_path = @"C:\Users\user\Downloads\2025winter\vs\alpha_beta_Gomoku\x64\Release\alpha_beta_Gomoku.dll";
        [DllImport(dll_path, EntryPoint = "get_negamax_agent")]
        private static extern IntPtr get_negamax_agent(int board_size);

        [DllImport(dll_path, EntryPoint = "add_agent_stone")]
        private static extern void add_agent_stone(IntPtr agent, int color, int y, int x);

        [DllImport(dll_path, EntryPoint = "find_opt_move_with_alpha_beta")]
        private static extern int find_opt_move_with_alpha_beta(IntPtr agent, int color, ref int rec_y, ref int rec_x, int time_limit);
        
        [DllImport(dll_path, EntryPoint = "print_board")]
        private static extern void print_board(IntPtr agent);

        IntPtr agent;
        public Negamax()
        {
            agent = get_negamax_agent(15);
        }
        
        public Negamax(int board_size)
        {
            agent = get_negamax_agent(board_size);
        }

        public override (int, int) get_opt_move(Board board, int player)
        {
            for(int x = 1; x <= board.board_size; x++)
            {
                for(int y = 1; y <= board.board_size; y++)
                {
                    add_agent_stone(agent, board[x-1, y-1], y, x);
                }
            }
            int px = -1, py = -1;
            int depth = find_opt_move_with_alpha_beta(agent, player, ref py, ref px, 1);
            Debug.WriteLine("generate move of depth");
            Debug.WriteLine(depth);
            if (px <= 0 || px > board.board_size || py <= 0 || py > board.board_size)
                throw new Exception("Error: invalid cpu move [ ("+px.ToString()+","+py.ToString()+") ]");
            return (px-1, py-1);
        }
    }
}
