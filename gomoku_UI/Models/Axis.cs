using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gomoku_UI.Models
{
    public struct Axis
    {
        public int x { get; set; }
        public int y { get; set; }
        public Axis(int X, int Y)
        {
            x = X;
            y = Y;
        }
        public static Axis operator +(Axis a, Axis b)
        {
            return new Axis(a.x + b.x, a.y + b.y);
        }
        public static Axis operator *(int c, Axis a)
        {
            return new Axis(c * a.x, c * a.y);
        }
        public static Axis operator *(Axis a, int c)
        {
            return new Axis(c * a.x, c * a.y);
        }
    };
    public enum Player
    {
        Black=1,
        White=-1,
        None=0
    }
    public enum Player_Type
    {
        CPU,
        Human
    }
}
