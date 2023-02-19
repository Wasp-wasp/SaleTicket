using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using f = System.Windows.Forms;
using SaleTicket.Model;
using System.IO;
using System.Data.Entity;

namespace SaleTicket
{
    public partial class AccountWindow : Window
    {
        int Role_ID, User_ID, tick = 0;
        Point now = new Point(0, 0);

        Image image = new Image();
        List<Gender> genders;
        f.Timer timer = new f.Timer();

        public AccountWindow(int id, int role)
        {
            InitializeComponent();
            if (role == 1)
                WindowUserAll.Visibility = Visibility.Visible;
            if (role == 2)
            {
                WindowStatistic.Visibility = Visibility.Visible;
                WindowCarAll.Visibility = Visibility.Visible;
            }

            User_ID = id;
            Role_ID = role;

            using (var context = new stariyEntities1())
            {
                genders = context.Gender.ToList();
                foreach (var i in genders)
                    GenderBox.Items.Add(i.Gender1);
            }

            userLoad();

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
                AuthorizationWindow authorization = new AuthorizationWindow();
                authorization.Show();
                Close();
            }
        }

        private void ShopBtn_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            ShopWindow shopWindow = new ShopWindow(User_ID, Role_ID);
            shopWindow.Show();
            this.Close();
        }

        private void userLoad()
        {
            using (var context = new stariyEntities1())
            {
                var user = context.User.SingleOrDefault(x => x.ID_User == User_ID);
                GenderBox.Text = user.Gender.Gender1;
                TextRole.Text = user.Role.Role1;
                TextLogin.Text = user.Login;
                TextPassword.Text = user.Password;
                TextName.Text = user.Name;
                TextLastName.Text = user.LastName;
                TextEmail.Text = user.Mail;
                byte[] img = user.Image;
                if (img != null)
                {
                    image = byteArrayToImage(user.Image);
                    image_main.Source = image.Source;
                    image.Source = image_main.Source;
                    Img.Source = image.Source;
                }
            }
        }

        public Image byteArrayToImage(byte[] byteArray)
        {
            Image image = new Image();
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                image.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
            return image;
        }

        private void UpdateAll_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new stariyEntities1())
            {
                var userAgain = context.User.SingleOrDefault(i => i.ID_User == User_ID);
                if (TextName.Text.Length > 0 && GenderBox.Text.Length > 0 && TextPassword.Text.Length > 0)
                {
                    userAgain.Name = TextName.Text.Trim();
                    userAgain.LastName = TextLastName.Text.Trim();
                    var GenderBoxItem = context.Gender.SingleOrDefault(a => a.Gender1 == (string)GenderBox.SelectedItem);
                    userAgain.ID_Gender = GenderBoxItem.ID_Gender;
                    userAgain.Mail = TextEmail.Text.Trim();
                    userAgain.Password = TextPassword.Text.Trim();
                    if (getJPGFromImageControl(image.Source as BitmapImage) != null)
                        userAgain.Image = getJPGFromImageControl(image.Source as BitmapImage);
                    context.Entry(userAgain).State = EntityState.Modified;
                    context.SaveChanges();
                    MessageBox.Show("Вы обнавили!");
                }
            }
        }

        public byte[] getJPGFromImageControl(BitmapImage imageC)
        {
            if (imageC != null)
            {
                MemoryStream memStream = new MemoryStream();
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(imageC));
                encoder.Save(memStream);
                return memStream.ToArray();
            }
            else
                return null;
        }

        private void ImageUpdate_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            Microsoft.Win32.OpenFileDialog ofdPicture = new Microsoft.Win32.OpenFileDialog();
            ofdPicture.Filter =
                "Image files|*.bmp;*.jpg;*.JPG;*.gif;*.png;*.tif|All files|*.*";
            ofdPicture.FilterIndex = 1;
            timer.Start();
            if (ofdPicture.ShowDialog() == true)
            {
                myBitmapImage.UriSource = new Uri(ofdPicture.FileName);
                myBitmapImage.EndInit();
                image.Source = myBitmapImage;
                image_main.Source = image.Source;
                Img.Source = image.Source;
            }
        }

        private void BackBtn(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            AuthorizationWindow authorization = new AuthorizationWindow();
            Close();
            authorization.Show();
        }

        private void WindowUserAll_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            AdminWindow adminWindow = new AdminWindow(User_ID, Role_ID);
            adminWindow.Show();
            this.Close();
        }

        private void WindowTicketAll_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            TicketWindow ticketWindow = new TicketWindow(User_ID, Role_ID);
            ticketWindow.Show();
            this.Close();
        }

        private void WindowStatistic_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            StatisticWindow statisticWindow = new StatisticWindow(User_ID, Role_ID);
            statisticWindow.Show();
            this.Close();
        }

        private void Img_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Img.Visibility = Visibility.Visible;
            Main.Visibility = Visibility.Hidden;
        }

        private void Img_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Img.Visibility = Visibility.Hidden;
            Main.Visibility = Visibility.Visible;
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