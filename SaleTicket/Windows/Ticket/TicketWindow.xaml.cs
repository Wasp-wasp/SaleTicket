using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using f = System.Windows.Forms;
using System.Data.Entity;
using SaleTicket.Model;

namespace SaleTicket
{
    public partial class TicketWindow : Window
    {
        public List<Ticket> Tickets { get; set; }
        public List<Destination> Destinations { get; set; }
        public List<TypeOfTicket> TypeOfTickets { get; set; }
        public List<Transport> Transports { get; set; }

        private Ticket HandlerTicket { get; set; }
        private Destination HandlerPointOfDeparture { get; set; }
        private Destination HandlerPointDestination { get; set; }
        private TypeOfTicket HandlerTypeOfTicket { get; set; }
        private Transport HandlerTransport { get; set; }

        f.Timer timer = new f.Timer();
        Point now = new Point(0, 0);

        int id, role, tick = 0;

        public TicketWindow(int ID, int ROLE)
        {
            InitializeComponent();
            id = ID;
            role = ROLE;

            HandlerTicket = new Ticket();
            HandlerPointOfDeparture = new Destination();
            HandlerPointDestination = new Destination();
            HandlerTypeOfTicket = new TypeOfTicket();
            HandlerTransport = new Transport();

            Tickets = new List<Ticket>();
            Destinations = new List<Destination>();
            TypeOfTickets = new List<TypeOfTicket>();
            Transports = new List<Transport>();

            using (var context = new stariyEntities1())
            {
                context.Configuration.ProxyCreationEnabled = false;
                Destinations = context.Destination.ToList();
                TypeOfTickets = context.TypeOfTicket.ToList();
                Transports = context.Transport.ToList();
            }

            updateDataGrid();

            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();

            UpdateComboPointOfDeparture.ItemsSource = Destinations;
            ComboPointOfDeparture.ItemsSource = Destinations;
            UpdateComboPointDestination.ItemsSource = Destinations;
            ComboPointDestination.ItemsSource = Destinations;
            UpdateComboTransport.ItemsSource = Transports;
            UpdateComboTypeOfTicket.ItemsSource = TypeOfTickets;

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            tick++;
            if (tick > 20)
            {
                timer.Stop();
                AccountWindow accountWindow = new AccountWindow(id, role);
                accountWindow.Show();
                Close();
            }
        }

        private void myDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = myDataGrid.SelectedIndex;
            if (selectedIndex > -1 && selectedIndex < Tickets.Count)
            {
                Ticket selectedItem = (Ticket)myDataGrid.SelectedItem;
                HandlerTicket = selectedItem;
                UpdateTextPointOfDeparture.Text = selectedItem.TicketPointOfDeparture;
                UpdateTextPointDestination.Text = selectedItem.TicketPointDestination;
                UpdateTextTransport.Text = selectedItem.TicketTransport;
                UpdateTextTypeOfTicket.Text = selectedItem.TicketTypeOfTicket;
                UpdateDataTicket.Text = selectedItem.Data.Date.ToString();
                UpdateTextPrice.Text = selectedItem.Price.ToString();
                addBtn.IsEnabled = false;
                updateBtn.IsEnabled = true;
            }
        }

        private void UpdateComboPointOfDeparture_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = UpdateComboPointOfDeparture.SelectedIndex;
            if (selectedIndex > -1)
            {
                HandlerPointOfDeparture = (Destination)UpdateComboPointOfDeparture.SelectedItem;
                UpdateTextPointOfDeparture.Text = HandlerPointOfDeparture.TypeDestination;
            }
            UpdateComboPointOfDeparture.SelectedIndex = -1;
        }

        private void UpdateComboPointDestination_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = UpdateComboPointDestination.SelectedIndex;
            if (selectedIndex > -1)
            {
                HandlerPointDestination = (Destination)UpdateComboPointDestination.SelectedItem;
                UpdateTextPointDestination.Text = HandlerPointDestination.TypeDestination;
            }
            UpdateComboPointDestination.SelectedIndex = -1;
        }

        private void UpdateComboTransport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = UpdateComboTransport.SelectedIndex;
            if (selectedIndex > -1)
            {
                HandlerTransport = (Transport)UpdateComboTransport.SelectedItem;
                UpdateTextTransport.Text = HandlerTransport.TypeTransport;
            }
            UpdateComboTransport.SelectedIndex = -1;
        }

        private void UpdateComboTypeOfTicket_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = UpdateComboTypeOfTicket.SelectedIndex;
            if (selectedIndex > -1)
            {
                HandlerTypeOfTicket = (TypeOfTicket)UpdateComboTypeOfTicket.SelectedItem;
                UpdateTextTypeOfTicket.Text = HandlerTypeOfTicket.Type;
            }
            UpdateComboTypeOfTicket.SelectedIndex = -1;
        }

        private void ComboPointOfDeparture_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = ComboPointOfDeparture.SelectedIndex;
            if (selectedIndex > -1)
            {
                Destination selectedItem = (Destination)ComboPointOfDeparture.SelectedItem;
                var filtered = Tickets.Where(destination => destination.ID_PointOfDeparture == selectedItem.ID_Destination);
                myDataGrid.ItemsSource = filtered;
            }
        }

        private void ComboPointDestination_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = ComboPointDestination.SelectedIndex;
            if (selectedIndex > -1)
            {
                Destination selectedItem = (Destination)ComboPointDestination.SelectedItem;
                var filtered = Tickets.Where(destination => destination.ID_PointDestination == selectedItem.ID_Destination);
                myDataGrid.ItemsSource = filtered;
            }
        }

        private void TextSourse_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var filtered = Tickets.Where(Price => Convert.ToString(Price.Price).StartsWith(TextSourse.Text));
            myDataGrid.ItemsSource = filtered;
        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UpdateTextPointOfDeparture.Text.Length > 0 && UpdateTextPointDestination.Text.Length > 0 && UpdateTextTransport.Text.Length > 0 && UpdateTextTypeOfTicket.Text.Length > 0 && UpdateDataTicket.Text.Length > 0 && UpdateTextPrice.Text.Length > 0)
            {
                using (var context = new stariyEntities1())
                {
                    var ticket = context.Ticket.SingleOrDefault(x => x.ID_Ticket == HandlerTicket.ID_Ticket);
                    if (ticket != null)
                    {
                        if (HandlerPointOfDeparture.TypeDestination != null)
                            ticket.ID_PointOfDeparture = HandlerPointOfDeparture.ID_Destination;

                        if (HandlerPointDestination.TypeDestination != null)
                            ticket.ID_PointDestination = HandlerPointDestination.ID_Destination;

                        if (HandlerTypeOfTicket.Type != null)
                            ticket.ID_TypeOfTicket = HandlerTypeOfTicket.ID_TypeOfTicket;

                        if (HandlerTransport.TypeTransport != null)
                            ticket.ID_Transport = HandlerTransport.ID_transport;

                        ticket.Data = UpdateDataTicket.SelectedDate.Value;
                        ticket.Price = Convert.ToDouble(UpdateTextPrice.Text);
                    }
                    context.Entry(ticket).State = EntityState.Modified;
                    context.SaveChanges();
                    clear();
                }
                MessageBox.Show("Вы обнавили товар");
                updateDataGrid();
            }
            else
                MessageBox.Show("Ошибка");
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UpdateTextPointOfDeparture.Text.Length > 0 && UpdateTextPointDestination.Text.Length > 0 && UpdateTextTransport.Text.Length > 0 && UpdateTextTypeOfTicket.Text.Length > 0 && UpdateDataTicket.Text.Length > 0 && UpdateTextPrice.Text.Length > 0)
            {
                using (var context = new stariyEntities1())
                {
                    var tic = new Ticket()
                    {
                        ID_PointOfDeparture = HandlerPointOfDeparture.ID_Destination,
                        ID_PointDestination = HandlerPointDestination.ID_Destination,
                        ID_TypeOfTicket = HandlerTypeOfTicket.ID_TypeOfTicket,
                        ID_Transport = HandlerTransport.ID_transport,
                        Data = UpdateDataTicket.SelectedDate.Value,
                        Price = Convert.ToDouble(UpdateTextPrice.Text)
                    };

                    context.Ticket.Add(tic);
                    context.SaveChanges();
                    MessageBox.Show("Машина зарегестрирована");
                }
                clear();
                updateDataGrid();
            }
            else
                MessageBox.Show("123");
        }

        void clear()
        {
            UpdateTextPointOfDeparture.Text = "";
            UpdateTextPointDestination.Text = "";
            UpdateTextTransport.Text = "";
            UpdateTextTypeOfTicket.Text = "";

            UpdateDataTicket.Text = "";
            UpdateTextPrice.Text = "";

            ComboPointOfDeparture.Text = "";
            ComboPointDestination.Text = "";

            TextSourse.Text = "";

            addBtn.IsEnabled = true;
            updateBtn.IsEnabled = false;

            updateDataGrid();
        }

        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            clear();
        }

        void updateDataGrid()
        {
            using (var context = new stariyEntities1())
            {
                context.Configuration.ProxyCreationEnabled = false;
                Tickets = context.Ticket.ToList();
            }
            foreach (var ticket in Tickets)
            {
                foreach (var PointOfDeparture in Destinations)
                {
                    if (ticket.ID_PointOfDeparture == PointOfDeparture.ID_Destination)
                        ticket.TicketPointOfDeparture = PointOfDeparture.TypeDestination;
                    if(ticket.ID_PointDestination == PointOfDeparture.ID_Destination)
                        ticket.TicketPointDestination = PointOfDeparture.TypeDestination;
                }
                foreach (var transport in Transports)
                    if (ticket.ID_Transport == transport.ID_transport)
                        ticket.TicketTransport = transport.TypeTransport;
                foreach (var typeOfTicket in TypeOfTickets)
                    if (ticket.ID_TypeOfTicket == typeOfTicket.ID_TypeOfTicket)
                        ticket.TicketTypeOfTicket = typeOfTicket.Type;
            }
            myDataGrid.ItemsSource = Tickets;
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            AccountWindow accountWindow = new AccountWindow(id, role);
            accountWindow.Show();
            this.Close();
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
