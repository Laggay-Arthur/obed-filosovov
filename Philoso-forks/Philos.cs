using System;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.Generic;


namespace Philoso_forks
{
    class Philos
    {
        byte myIndex;
        public byte myTask;
        public bool isStoped;
        Label myState;
        List<Fork> forks;
        const int EAT_TIME = 1000;// Время за которое философ съедает еду
        System.Windows.Threading.DispatcherTimer timer;
        public Philos(ref List<Fork> forks, byte index, Label myState)
        {// Конструктор и начальная инициализация полей
            this.forks = forks;
            myIndex = index;
            this.myState = myState;
            timer = new System.Windows.Threading.DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 1500), };//По истечению этого времени философ считается голодным
            timer.Tick += new EventHandler(hungry);
        }
        byte leftFork, rightFork;
        public async void eat()
        {// Проверка доступности вилок и измнениение состояния на 0(ест)
            if (!isStoped && GetState != 1 && GetState != 0)
            {
                leftFork = get_left_index_of_fork(myIndex);
                rightFork = get_right_index_of_fork(myIndex);
                if (myTask == 0)
                {// Семафоры
                    if (forks[leftFork].enter() && forks[rightFork].enter())
                    {
                        timer.Stop();
                        ChangeState = 0;
                        myState.Content = "Ест"; myState.Foreground = Brushes.Blue;
                        await Task.Delay(EAT_TIME);
                        forks[leftFork].leave();
                        forks[rightFork].leave();
                        think();
                    }
                    else if (GetState != 2) think();

                    forks[leftFork].leave();
                    forks[rightFork].leave();
                }
                else if (myTask == 1)
                {// Mutex
                    if (forks[leftFork].mutex() && forks[rightFork].mutex())
                    {
                        timer.Stop();
                        forks[leftFork].changeOwner(myIndex);
                        forks[rightFork].changeOwner(myIndex);
                        ChangeState = 0;
                        myState.Content = "Ест"; myState.Foreground = Brushes.Blue;
                        await Task.Delay(EAT_TIME);
                        forks[leftFork].changeOwner(33); forks[rightFork].changeOwner(33);
                        forks[leftFork].clear_mutex(); forks[rightFork].clear_mutex();
                        think();
                    }
                    else if (GetState != 2) think();

                    forks[leftFork].changeOwner(33); forks[rightFork].changeOwner(33);
                    forks[leftFork].clear_mutex(); forks[rightFork].clear_mutex();
                }
                else if (myTask == 2)
                {// Futex
                    if (Fork.freeForks[leftFork] && Fork.freeForks[rightFork])
                    {
                        timer.Stop();
                        Fork.freeForks[leftFork] = false;
                        Fork.freeForks[rightFork] = false;
                        ChangeState = 0;
                        myState.Content = "Ест"; myState.Foreground = Brushes.Blue;
                        await Task.Delay(EAT_TIME);
                        forks[leftFork].youFree(); forks[rightFork].youFree();
                    }
                    else if (Fork.freeForks[leftFork] && !Fork.freeForks[rightFork])
                    {
                        forks[rightFork].areYouFree(myIndex, rightFork);
                    }

                    else if (!Fork.freeForks[leftFork] && Fork.freeForks[rightFork])
                    {
                        forks[leftFork].areYouFree(myIndex, leftFork);
                    }
                    else
                    {
                        forks[leftFork].areYouFree(myIndex, leftFork);
                        forks[rightFork].areYouFree(myIndex, rightFork);
                    }

                    if (GetState != 2) think();
                }
            }
        }
        async public void forkFree(byte who)
        {// Вилка освободилась

            //string s = "";//Дебаг доступности вилок
            //for (int i = 0; i < 5; i++) s += Fork.freeForks[i] ? "1" : "0";
            //System.Windows.MessageBox.Show(s);

            if (Fork.freeForks[leftFork] && Fork.freeForks[rightFork])
            {
                timer.Stop();
                Fork.freeForks[leftFork] = false;
                Fork.freeForks[rightFork] = false;
                ChangeState = 0;
                myState.Content = "Ест"; myState.Foreground = Brushes.Blue;
                await Task.Delay(EAT_TIME);
                forks[leftFork].youFree(); forks[rightFork].youFree();
            }
            if (GetState != 2) think();
            else eat();
        }
        public async void think()
        {// Начинаем думать
            if (!isStoped && GetState != 1)
            {
                ChangeState = 1;
                myState.Content = "Думает"; myState.Foreground = Brushes.YellowGreen;
                await Task.Delay(EAT_TIME / 2);
                timer.Start();
                eat();
            }
        }
        void hungry(object sender, EventArgs e)
        {// Вызываем голод
            if (!isStoped && GetState != 2)
            {
                ChangeState = 2;
                timer.Stop();
                myState.Content = "Голодает"; myState.Foreground = Brushes.Red;
                if (myTask == 2)
                {
                    forkFree(0);
                }
            }
        }
        byte get_left_index_of_fork(byte b)
        {// Получаем левый индекс вилки
            if (b == 0) return 1;
            if (b == 4) return 0;
            return ++b;
        }
        byte get_right_index_of_fork(byte b) => b;//Получаем правый индекс вилка
        public void Stop() { timer.Stop(); }//Останавливаем таймер
        public byte GetState { get; private set; } = 3;//0 - eat, 1 - think, 2 - hungry
        public byte ChangeState { set => GetState = value; }//Изменить состояние философа
    }
}