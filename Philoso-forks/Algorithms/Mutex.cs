using System;
using System.Collections.Generic;


namespace Philoso_forks.Algorithms
{
    class Mutex
    {
        List<Philos> philosis;
        public Mutex(ref List<Philos> philosis)
        {
            this.philosis = philosis; ;
            timer = new System.Windows.Threading.DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 1200), };
            timer.Tick += new EventHandler(Start);
        }
        System.Windows.Threading.DispatcherTimer timer;
        public void Start(object sender = null, EventArgs e = null)
        {
            if (!timer.IsEnabled) timer.Start();
            for (byte i = 0; i < 5; i++) philosis[i].eat();
        }
        public void Stop() { timer.Stop(); for (byte i = 0; i < 5; i++) philosis[i].Stop(); }
    }
}