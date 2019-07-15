using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Philoso_forks
{
    class Fork
    {
        #region Semaphore

        byte semaphore_N = 0;
        const byte semaphore_N_max = 1;

        public void init() { /*semaphore_N_max = 1;*/ }
        public bool enter()
        {
            if (semaphore_N < semaphore_N_max) { semaphore_N++; return true; }
            return false;
        }
        public void leave() { if (semaphore_N > 0) semaphore_N--; }
        #endregion


        #region Mutex

        byte mute = 0;
        byte mutex_owner = 33;
        public void changeOwner(byte owner)
        {
            mutex_owner = owner;
        }
        public bool mutex()
        {
            if (mute == 0) { mute++; return true; }
            return false;
        }
        public void clear_mutex() { mute = 0; }
        #endregion


        #region Futex

        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 1000), };//По истечению этого времени вилка сообщает, что она свободна
        public void initTimer()
        {
            if (!timer.IsEnabled)
            {
                timer.Tick += new EventHandler(invokeMethod);
                timer.Start();
            }
        }
        public void StopTimer() { if (!timer.IsEnabled) timer.Stop(); }
        List<byte> whoCallMe = new List<byte>();
        public static bool[] freeForks = { true, true, true, true, true };
        async void invokeMethod(object sender, EventArgs e)
        {
            if (freeForks[myIndex] && whoCallMe.Count > 0)
            {
                philosis[whoCallMe[0]].forkFree(myIndex);
                whoCallMe.RemoveAt(0);
                await Task.Delay(2000);
            }
        }
        byte myIndex;
        static List<Philos> philosis;
        public void youFree() { Fork.freeForks[myIndex] = true; }
        public void showForkAllPhilosis(ref List<Philos> philosis) { Fork.philosis = philosis; }
        public void areYouFree(byte index, byte myIndex)
        {
            this.myIndex = myIndex;
            if (freeForks[myIndex]) { Fork.freeForks[myIndex] = false; philosis[index].forkFree(myIndex); }
            else if (!whoCallMe.Contains(index)) whoCallMe.Add(index);

        }
        #endregion
    }
}
