using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace gomoku_UI
{
    public class player_info_ViewModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public player_info_ViewModel()
        {
            _timer.Interval = new TimeSpan(0,0,1);
        }

        DispatcherTimer _timer = new DispatcherTimer();
        int left_time;
        string topic= "player info";
        string left_time_str
        {
            get
            {
                return "left time: "+left_time.ToString();
            }
        }
        void on_enemy_add_stone()
        {
            left_time = 30;
            _timer.Start();
        }
        void on_self_add_stone()
        {
            //left_time = 30;
            _timer.Stop();
        }

    }
}
