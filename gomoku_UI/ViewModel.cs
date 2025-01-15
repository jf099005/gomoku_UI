using gomoku;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace gomoku_UI
{
    public class ViewModel : INotifyPropertyChanged
    {
        //public const int board_size = 15;
        //public Board board;
        public event PropertyChangedEventHandler PropertyChanged;
        public void HAHA_exe()
        {
            Debug.WriteLine("HAHA");
        }
        public ICommand HAHA
        {
            get
            {
                return new RelayCommand(HAHA_exe);
            }
        }
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
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
        public ViewModel()
        {
            _current_step = 123;
            //board = new Board(board_size);
        }

    }
}
