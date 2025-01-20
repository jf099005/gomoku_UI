using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gomoku_UI.Models;
namespace gomoku
{
    class Board
    {
        public int board_size { get; }
        public int[,] board { get; }
        public int[,] history { get; }
        public int history_len;
        public Board(int n)
        {
            board_size = n;
            board = new int[n, n];
            history = new int[1000, 3];
        }

        public void print_board() {
            for (int i = 0; i < board_size; i++)
            {
                for (int j = 0; j < board_size; j++)
                {
                    Debug.Write(board[i, j]);
                }
                Debug.Write('\n');
            }
        }

        public void clear_board() {
            for (int i = 0; i < board_size; i++)
            {
                for (int j = 0; j < board_size; j++)
                {
                    board[i, j] = 0;
                }
            }
        }
        public bool out_of_bound(Axis pos)
        {
            return Math.Min(pos.x, pos.y) < 0 || Math.Max(pos.x, pos.y) >= board_size;
        }
        public int this[int x, int y]
        {
            get { return board[x, y]; }
        }

        public int this[Axis pos]
        {
            get {
                return board[pos.x, pos.y];
            }
        }

        public bool add_stone(int color, Axis pos)
        {
            return add_stone(color, new int[] { pos.x, pos.y });
        }
        public bool add_stone(int color, (int,int) pos)
        {
            return add_stone(color, new int[] { pos.Item1, pos.Item2 });
        }

        public bool add_stone(int color, int[] pos)
        {
            if (board[pos[0], pos[1]] != 0)
                return false;
            board[pos[0], pos[1]] = color;
            history[history_len, 0] = pos[0];
            history[history_len, 1] = pos[1];
            history[history_len, 2] = color;
            history_len++;
            return true;
        }
        public void undo()
        {
            history_len--;
            int py = history[history_len, 0];
            int px = history[history_len, 1];
            board[py, px] = 0;
        }

        public Axis latest_move { get {
                return new Axis(history[history_len - 1,0], history[history_len - 1,1]);
            }
        }

        public bool is_endgame()
        {
            for(int px = 0; px < board_size; px++)
            {
                for(int py = 0; py < board_size; py++)
                {
                    if (board[px, py] == 0)
                        continue;
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0)
                                continue;

                            if (px+ dx * 4 < 0 || px + dx * 4 >= board_size || py + dy * 4 < 0 || py + dy * 4 >= board_size)
                            {
                                continue;
                            }
                            bool flag = true;
                            for(int i = 0; i < 5; i++)
                            {
                                if (board[px + dx * i, py + dy * i] != board[px, py])
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
