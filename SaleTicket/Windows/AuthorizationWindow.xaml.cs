using System;
using System.IO;
using System.Linq;
using SaleTicket.Model;
using System.Windows;
using System.Windows.Media.Imaging;
using f = System.Windows.Forms;

namespace SaleTicket
{
    public partial class AuthorizationWindow : Window
    {
        int tick = 0, number = 0;
        string _captchaCode;

        Point now = new Point(0, 0);
        
        f.Timer timer = new f.Timer();

        public AuthorizationWindow()
        {
            InitializeComponent();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            tick++;
            if (tick == 20)
            {
                Environment.Exit(0);
            }
        }

        private void ButtonClickEnter(object sender, RoutedEventArgs e)
        {
            using (var context = new stariyEntities1())
            {
                var user = context.User.SingleOrDefault(x => x.Login == TextBoxLogin.Text.Trim() && x.Password == TextBoxPassword.Password.Trim());
                if (user != null)
                {
                    timer.Stop();
                    AccountWindow account = new AccountWindow(user.ID_User, Convert.ToInt32(user.ID_Role));
                    account.Show();
                    this.Close();
                }
                else
                {
                    number++;
                    if (number == 3)
                    {
                        FullIcon.Visibility = Visibility.Hidden;
                        ImageEye.Visibility = Visibility.Hidden;
                        captch.Visibility = Visibility.Visible;
                        LoadContentCapha();
                        number = 0;
                    }
                    MessageBox.Show("Не правельно введён логин или пароль");
                }
            }
        }

        private void ButtonClickRegistration(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
            this.Close();
        }

        private void CapchaLoad(object sender, RoutedEventArgs e)
        {
            if (cnText.Text.ToUpper() == _captchaCode)
            {
                FullIcon.Visibility = Visibility.Visible;
                captch.Visibility = Visibility.Hidden;
            }
            else
            {
                MessageBox.Show("Не правельно ввидена Captcha!");
                LoadContentCapha();
            }
        }

        public void LoadContentCapha()
        {
            int width = 100;
            int height = 36;

            var captchaCode = Captcha.GenerateCaptchaCode();
            _captchaCode = captchaCode;
            var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);

            Stream s = new MemoryStream(result.CaptchaByteData);

            luc.Source = BitmapFrame.Create(s,
                BitmapCreateOptions.None,
                BitmapCacheOption.OnLoad);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LoadContentCapha();
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Windows.Point pp = e.GetPosition(this);
            if (pp != now)
            {
                tick = 0;
            }
            now = pp;
        }

        private void ImageEye_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextPass.Text = TextBoxPassword.Password.Trim();
            TextPass.Visibility = Visibility.Visible;
            TextBoxPassword.Visibility = Visibility.Hidden;
        }

        private void ImageEye_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextPass.Visibility = Visibility.Hidden;
            TextBoxPassword.Visibility = Visibility.Visible;
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