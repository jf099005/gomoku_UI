using gomoku;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gomoku_UI.Models
{
    internal abstract class Gomoku_CPU_player
    {
        public Gomoku_CPU_player() { }
        public abstract (int, int) get_opt_move(Board board, int player);
    }
}
