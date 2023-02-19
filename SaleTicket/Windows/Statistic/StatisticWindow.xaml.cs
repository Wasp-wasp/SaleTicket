using System;
using System.Windows;
using f = System.Windows.Forms;

namespace SaleTicket
{
    public partial class StatisticWindow : Window
    {
        f.Timer timer = new f.Timer();
        Point now = new Point(0, 0);
        int id, ROLE, tick = 0;

        public StatisticWindow(int ID, int role)
        {
            id = ID;
            ROLE = role;
            InitializeComponent();

            PageChange.Content = new StatisticControl();

            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            tick++;
            if (tick > 20)
            {
                timer.Stop();
                AccountWindow accountWindow = new AccountWindow(id, ROLE);
                accountWindow.Show();
                Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            AccountWindow accountWindow = new AccountWindow(id, ROLE);
            this.Close();
            accountWindow.Show();
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point pp = e.GetPosition(this);
            if (pp != now)
            {
                tick = 0;
            }
            now = pp;
        }

        private void Window_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            tick = 0;
        }

        private void Window_KeyDownAndUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            tick = 0;
        }

        private void Window_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            tick = 0;
        }
    }
}
