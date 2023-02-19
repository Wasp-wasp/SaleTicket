using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SaleTicket.Model;
using System.Collections.Generic;
using System.Linq;
using f = System.Windows.Forms;
using Microsoft.Win32;

namespace SaleTicket
{
    public partial class RegistrationWindow : Window
    {
        int tick = 0;
        
        Point now = new Point(0, 0);

        f.Timer timer = new f.Timer();
        Image image = new Image();

        List<Gender> gender { get; set; }
        List<Role> roles { get; set; }

        public RegistrationWindow()
        {
            InitializeComponent();

            roles = new List<Role>();
            gender = new List<Gender>();

            comboBoxGet();

            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            tick++;
            if (tick == 20)
            {
                timer.Stop();
                AuthorizationWindow main = new AuthorizationWindow();
                Close();
                main.Show();
            }
        }

        private void comboBoxGet()
        {
            using (var context = new stariyEntities1())
            {
                roles = context.Role.Where(i => i.ID_Role != 1 && i.ID_Role != 2 && i.ID_Role != 4).ToList();
                foreach (var i in roles)
                    RoleBox.Items.Add(i.Role1);
                gender = context.Gender.ToList();
                foreach (var i in gender)
                    GenderBox.Items.Add(i.Gender1);
            }
        }

        private void RegistratoinBtn(object sender, RoutedEventArgs e)
        {
            TextName.ToolTip = null;
            TextName.Background = Brushes.Transparent;
            TextLastName.ToolTip = null;
            TextLastName.Background = Brushes.Transparent;
            TextLogin.ToolTip = null;
            TextLogin.Background = Brushes.Transparent;
            PasswordBox.ToolTip = null;
            PasswordBox.Background = Brushes.Transparent;
            PasswordBox2.ToolTip = null;
            PasswordBox2.Background = Brushes.Transparent;

            if (TextName.Text.Length < 1)
            {
                TextName.ToolTip = "Слишком короткое имя";
                TextName.Background = Brushes.IndianRed;
            }
            else if (TextLastName.Text.Length < 5)
            {
                TextLastName.ToolTip = "Слишком короткая фамилия";
                TextLastName.Background = Brushes.IndianRed;
            }
            else if (GenderBox.Text.Length < 1)
            {
                GenderBox.ToolTip = "Вы не выбрали пол";
                GenderBox.Background = Brushes.IndianRed;
            }
            else if (TextLogin.Text.Length < 3)
            {
                TextLogin.ToolTip = "Слишком маленький логин";
                TextLogin.Background = Brushes.IndianRed;
            }
            else if (PasswordBox.Password.Length < 3)
            {
                PasswordBox.ToolTip = "Слишком маленький пароль";
                PasswordBox.Background = Brushes.IndianRed;
            }
            else if (PasswordBox.Password != PasswordBox2.Password)
            {
                PasswordBox2.ToolTip = "Не верно введён пароль";
                PasswordBox2.Background = Brushes.IndianRed;
            }
            else if (RoleBox.Text.Length < 1)
            {
                RoleBox.ToolTip = "Вы не выбрали роль.";
                RoleBox.Background = Brushes.IndianRed;
            }
            else
            {
                TextName.ToolTip = null;
                TextName.Background = Brushes.Transparent;
                TextLastName.ToolTip = null;
                TextLastName.Background = Brushes.Transparent;
                TextLogin.ToolTip = null;
                TextLogin.Background = Brushes.Transparent;
                PasswordBox.ToolTip = null;
                PasswordBox.Background = Brushes.Transparent;
                PasswordBox2.ToolTip = null;
                PasswordBox2.Background = Brushes.Transparent;
                try
                {
                    using (var context = new stariyEntities1())
                    {
                        var request = context.User.SingleOrDefault(o => o.Login == TextLogin.Text);
                        if (request == null)
                        {
                            var roleBox = context.Role.SingleOrDefault(i => i.Role1 == (string)RoleBox.SelectedItem);
                            var genderBox = context.Gender.SingleOrDefault(a => a.Gender1 == (string)GenderBox.SelectedItem);
                            var user = new User();
                            user.ID_Role = roleBox.ID_Role;
                            user.ID_Gender = genderBox.ID_Gender;
                            user.Login = TextLogin.Text.Trim();
                            user.Password = PasswordBox.Password.Trim();
                            user.Name = TextName.Text.Trim();
                            user.LastName = TextLastName.Text.Trim();
                            user.Mail = TextMail.Text.Trim();
                            if (image.Source != null)
                                user.Image = getJPGFromImageControl(image.Source as BitmapImage);
                            context.User.Add(user);
                            context.SaveChanges();

                            MessageBox.Show("Вы успешно зарегистрированы!");

                            timer.Stop();
                            AuthorizationWindow authorization = new AuthorizationWindow();
                            authorization.Show();
                            Close();
                        }
                        else
                            MessageBox.Show("Есть такой пользователь!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка, подключение к базы. \n\n\n\nОшибка:\n" + ex.ToString());
                }
            }
        }

        public byte[] getJPGFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();
        }

        private void ImageBtn(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            OpenFileDialog ofdPicture = new OpenFileDialog();
            ofdPicture.Filter =
                "Image files|*.bmp;*.jpg;*.JPG;*.gif;*.png;*.tif|All files|*.*";
            ofdPicture.FilterIndex = 1;
            timer.Start();
            if (ofdPicture.ShowDialog() == true)
            {
                myBitmapImage.UriSource = new Uri(ofdPicture.FileName);
                myBitmapImage.EndInit();
                image.Source = myBitmapImage;
            }
        }

        private void AuthBtn(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            AuthorizationWindow window = new AuthorizationWindow();
            window.Show();
            Close();
        }

        private void Button_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextPass.Text = PasswordBox.Password.Trim();
            TextPass2.Text = PasswordBox2.Password.Trim();
            TextPass.Visibility = Visibility.Visible;
            TextPass2.Visibility = Visibility.Visible;
            PasswordBox.Visibility = Visibility.Hidden;
            PasswordBox2.Visibility = Visibility.Hidden;
        }

        private void Button_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextPass.Visibility = Visibility.Hidden;
            TextPass2.Visibility = Visibility.Hidden;
            PasswordBox.Visibility = Visibility.Visible;
            PasswordBox2.Visibility = Visibility.Visible;
        }

        private void Window_KeyDownAndUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            tick = 0;
        }

        private void Window_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            tick = 0;
        }

        private void Window_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            tick = 0;
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

        private void GenderBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}