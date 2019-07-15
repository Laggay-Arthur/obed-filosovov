using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;


namespace Philoso_forks
{
    public partial class MainWindow : Window
    {
        bool isInit = false;
        static List<Fork> forks;
        static List<Philos> philosis = new List<Philos>();
        dynamic selectedAlgorithm;
        public MainWindow()
        {
            InitializeComponent();
            forks = new List<Fork>();
            for (byte i = 0; i < 5; i++)
            { forks.Add(new Fork()); }
            List<Label> lbs = new List<Label>(fil_grid.Children.OfType<Label>());
            for (byte i = 0; i < lbs.Count; i++)
                if (lbs[i].Name == "fil" + philosis.Count + "_state") philosis.Add(new Philos(ref forks, (byte)philosis.Count, lbs[i]));

            selectedAlgorithm = new Algorithms.Semaphore(ref philosis);
            isInit = true;
        }
        void Rb_Checked(object sender, RoutedEventArgs e)
        {// Изменения метода решения
            if (isInit)
            {
                string rb_name = ((RadioButton)sender).Name;

                selectedAlgorithm.Stop();

                for (byte i = 0; i < 5; i++)
                {
                    philosis[i].isStoped = true;
                    forks[i].leave();
                    forks[i].clear_mutex();
                    forks[i].changeOwner(33);
                    forks[i].StopTimer();
                }//Очищаем вилки
                if (rb_name == "rb1")// Семофоры
                {
                    selectedAlgorithm = new Algorithms.Semaphore(ref philosis);
                    changeSelectedTask(0);
                }
                else if (rb_name == "rb2")// Mutex
                {
                    selectedAlgorithm = new Algorithms.Mutex(ref philosis);
                    changeSelectedTask(1);
                }
                else if (rb_name == "rb3")// Futex
                {
                    selectedAlgorithm = new Algorithms.Futex(ref philosis);
                    changeSelectedTask(2);
                }
            }
        }
        void changeSelectedTask(byte on)
        {// Изменение задания на другое и остановка работающих таймеров
            for (byte i = 0; i < 5; i++)
            {
                if (on == 2)
                {
                    forks[i].initTimer();
                    forks[i].showForkAllPhilosis(ref philosis);
                }
                philosis[i].myTask = on; philosis[i].isStoped = false;
            }
        }
        void Clear_Click(object sender, RoutedEventArgs e)
        {// Очистка состояние
            byte index = 0;
            foreach (Label lb in fil_grid.Children.OfType<Label>())
                if (lb.Name == "fil" + index + "_state")
                {
                    index++;
                    lb.Content = "Состояние";
                    lb.Foreground = System.Windows.Media.Brushes.Black;
                }
        }
        void Feed_Click(object sender, RoutedEventArgs e) { selectedAlgorithm.Start(); }// Запускаем выбранное решение
        void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();
    }
}