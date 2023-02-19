using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using f = System.Windows.Forms;
using SaleTicket.Model;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace SaleTicket
{
    public partial class AdminWindow : Window
    {
        public List<User> users { get; set; }
        public List<Gender> genders { get; set; }
        public List<Role> Roles { get; set; }

        private User HandlerUser { get; set; }
        private Role HandlerRole { get; set; }
        private Gender HandlerGender { get; set; }

        int tick = 0, ID, Role;
        f.Timer timer = new f.Timer();
        Point now = new Point(0, 0);

        public AdminWindow(int ID_, int Role_)
        {
            InitializeComponent();

            Role = Role_;
            ID = ID_;

            HandlerUser = new User();
            HandlerRole = new Role();
            HandlerGender = new Gender();

            users = new List<User>();
            genders = new List<Gender>();
            Roles = new List<Role>();


            using (var context = new stariyEntities1())
            {
                context.Configuration.ProxyCreationEnabled = false;
                genders = context.Gender.ToList();
                Roles = context.Role.ToList();
            }

            LoadDataUser();

            RoleBoxUpdate.ItemsSource = Roles;
            GenderBoxUpdate.ItemsSource = genders;
            RoleBox.ItemsSource = Roles;
            GenderBox.ItemsSource = genders;

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
                AccountWindow accountWindow = new AccountWindow(ID, Role);
                accountWindow.Show();
                Close();
            }
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            AccountWindow accountWindow = new AccountWindow(ID, Role);
            accountWindow.Show();
            Close();
        }

        private void LoadDataUser()
        {
            using (var context = new stariyEntities1())
            {
                context.Configuration.ProxyCreationEnabled = false;
                users = context.User.ToList();
            }
            foreach (var User in users)
            {
                foreach (var gender in genders)
                    if (User.ID_Gender == gender.ID_Gender)
                        User.UserGender = gender.Gender1;
                foreach (var Role in Roles)
                    if (User.ID_Role == Role.ID_Role)
                        User.UserRole = Role.Role1;
            }
            myDataGrid.ItemsSource = users;
        }
        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            if (loginText.Text.Length > 0 && passwordText.Text.Length > 0 && nameText.Text.Length > 0 && GenderTextBox.Text.Length > 0 && RoleTextBox.Text.Length > 0)
            {
                try
                {
                    Role choiceRole = new Role();
                    if (HandlerRole.Role1 != null)
                        choiceRole.ID_Role = HandlerRole.ID_Role;

                    Gender choiceGender = new Gender();
                    if (HandlerGender.Gender1 != null)
                        choiceGender.ID_Gender = HandlerGender.ID_Gender;

                    using (var context = new stariyEntities1())
                    {
                        var us = new User();
                        us.Name = nameText.Text;
                        us.LastName = lastNameText.Text;
                        us.Login = loginText.Text;
                        us.Password = passwordText.Text;
                        us.ID_Role = choiceRole?.ID_Role;
                        us.ID_Gender = choiceGender?.ID_Gender;
                        if (getJPGFromImageControl(imageUser.Source as BitmapImage) != null)
                            us.Image = getJPGFromImageControl(imageUser.Source as BitmapImage);
                        context.User.Add(us);
                        context.SaveChanges();
                        MessageBox.Show("Пользователь зарегестрирован");
                        clear();
                    }
                }
                catch
                {
                    MessageBox.Show("Введите корректные данные");
                }
            }
        }


        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (loginText.Text.Length > 0 && passwordText.Text.Length > 0 && nameText.Text.Length > 0 && GenderTextBox.Text.Length > 0 && RoleTextBox.Text.Length > 0)
            {
                if (HandlerUser != null)
                {
                    using (var context = new stariyEntities1())
                    {
                        var us = context.User.SingleOrDefault(x => x.ID_User == HandlerUser.ID_User);
                        if (us != null)
                        {
                            us.Login = loginText.Text.Trim();
                            us.Password = passwordText.Text.Trim();
                            us.Name = nameText.Text.Trim();
                            us.LastName = lastNameText.Text.Trim();

                            if (HandlerRole.Role1 != null)
                                us.ID_Role = HandlerRole.ID_Role;
                            if (HandlerGender.Gender1 != null)
                                us.ID_Gender = HandlerGender.ID_Gender;
                            if (getJPGFromImageControl(imageUser.Source as BitmapImage) != null)
                                us.Image = getJPGFromImageControl(imageUser.Source as BitmapImage);
                        }
                        context.Entry(us).State = EntityState.Modified;
                        context.SaveChanges();
                        clear();
                    }

                    MessageBox.Show("Вы обновили пользователя!");
                    LoadDataUser();
                }
            }
            else
            {
                MessageBox.Show("Ввидите данные!");
            }
        }

        private void imageBtn_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            OpenFileDialog ofdPicture = new OpenFileDialog();
            ofdPicture.Filter =
                "Image files|*.bmp; *.jpg; *.JPG; *.gif; *.png; *.tif|All files|*.*";
            ofdPicture.FilterIndex = 1;
            timer.Start();
            if (ofdPicture.ShowDialog() == true)
            {
                myBitmapImage.UriSource = new Uri(ofdPicture.FileName);
                myBitmapImage.EndInit();
                imageUser.Source = myBitmapImage;
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


        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            clear();
        }

        void clear()
        {
            imageUser.Source = null;
            loginText.Text = "";
            passwordText.Text = "";
            nameText.Text = "";
            lastNameText.Text = "";
            EmailText.Text = "";
            GenderTextBox.Text = "";
            RoleTextBox.Text = "";

            GenderBox.Text = "";
            RoleBox.Text = "";
            TextSourse.Text = "";

            addBtn.IsEnabled = true;
            updateBtn.IsEnabled = false;

            LoadDataUser();
        }

        private void ComboGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = GenderBox.SelectedIndex;
            if (selectedIndex > -1)
            {
                Gender selectedItem = (Gender)GenderBox.SelectedItem;
                var filtered = users.Where(Users => Users.ID_Gender == selectedItem.ID_Gender);
                myDataGrid.ItemsSource = filtered;
            }
        }

        private void ComboRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = RoleBox.SelectedIndex;
            if (selectedIndex > -1)
            {
                Role selectedItem = (Role)RoleBox.SelectedItem;
                var filtered = users.Where(Users => Users.ID_Role == selectedItem.ID_Role);
                myDataGrid.ItemsSource = filtered;
            }
        }

        private void TextSourse_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filtered = users.Where(Users => Users.Login.StartsWith(TextSourse.Text) || Users.Password.StartsWith(TextSourse.Text) || Users.Name.StartsWith(TextSourse.Text) || Users.LastName.StartsWith(TextSourse.Text) || Users.Mail.StartsWith(TextSourse.Text));
            myDataGrid.ItemsSource = filtered;
        }

        private void myDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = myDataGrid.SelectedIndex;
            if (selectedIndex > -1 && selectedIndex < users.Count)
            {
                User selectedItem = (User)myDataGrid.SelectedItem;
                HandlerUser = selectedItem;
                RoleTextBox.Text = selectedItem.UserRole;
                GenderTextBox.Text = selectedItem.UserGender;
                loginText.Text = selectedItem.Login;
                passwordText.Text = selectedItem.Password;
                nameText.Text = selectedItem.Name;
                lastNameText.Text = selectedItem.LastName;
                EmailText.Text = selectedItem.Mail;

                addBtn.IsEnabled = false;
                updateBtn.IsEnabled = true;

                if (selectedItem.Image != null)
                    imageUser.Source = byteArrayToImage(selectedItem.Image).Source;
                else
                    imageUser.Source = null;
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

        private void RoleBoxUpdate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = RoleBoxUpdate.SelectedIndex;
            if (selectedIndex > -1)
            {
                HandlerRole = (Role)RoleBoxUpdate.SelectedItem;
                RoleTextBox.Text = HandlerRole.Role1;
            }
            RoleBoxUpdate.SelectedIndex = -1;
        }

        private void GenderBoxUpdate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = GenderBoxUpdate.SelectedIndex;
            if (selectedIndex > -1)
            {
                HandlerGender = (Gender)GenderBoxUpdate.SelectedItem;
                GenderTextBox.Text = HandlerGender.Gender1;
            }
            GenderBoxUpdate.SelectedIndex = -1;
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