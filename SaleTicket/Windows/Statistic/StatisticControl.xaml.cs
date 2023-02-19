using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using SaleTicket.Model;

namespace SaleTicket
{
    public partial class StatisticControl : UserControl
    {
        List<Order> Orders { get; set; }
        List<Basket> Baskets { get; set; }
        List<Ticket> Tickets { get; set; }
        List<TypeOfTicket> TypeOfTickets { get; set; }
        List<User> Users { get; set; }

        public StatisticControl()
        {
            InitializeComponent();

            Orders = new List<Order>();
            Baskets = new List<Basket>();
            Tickets = new List<Ticket>();
            TypeOfTickets = new List<TypeOfTicket>();
            Users = new List<User>();

            using (var context = new stariyEntities1())
            {
                context.Configuration.ProxyCreationEnabled = false;
                Orders = context.Order.ToList();
                Baskets = context.Basket.ToList();
                Tickets = context.Ticket.ToList();
                TypeOfTickets = context.TypeOfTicket.ToList();
                Users = context.User.ToList();
            }

            ChartPayments.ChartAreas.Add(new ChartArea("Main"));

            var currentSeries = new Series("Payments")
            {
                IsValueShownAsLabel = true
            };

            ChartPayments.Series.Add(currentSeries);

            ComboChart.ItemsSource = Enum.GetValues(typeof(SeriesChartType));

            foreach (var ticket in Tickets)
            {
                foreach (var typeOfTicket in TypeOfTickets)
                {
                    if (ticket.ID_TypeOfTicket == typeOfTicket.ID_TypeOfTicket)
                    {
                        ticket.TicketTypeOfTicket = typeOfTicket.Type;
                        break;
                    }
                }
            }

            DataGridUsers.ItemsSource = Users;
            ChoiceCar.ItemsSource = Tickets;
            StartDate.SelectedDate = new DateTime(2022, 04, 28);
            EndDate.SelectedDate = DateTime.Now;
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            DataGridUsers.SelectedIndex = -1;
            ChoiceCar.SelectedIndex = -1;
            StartDate.SelectedDate = new DateTime(2022, 04, 28);
            EndDate.SelectedDate = DateTime.Now;
            UpdateChart();
        }

        private void ChoiceCar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateChart();
        }

        private void ComboChart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateChart();
        }

        private void DataGridUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateChart();
        }

        private void StartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateChart();
        }

        private void EndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateChart();
        }

        private void UpdateChart()
        {
            Series currentSeries = ChartPayments.Series.FirstOrDefault();
            if (ComboChart.SelectedItem is SeriesChartType currentType)
            {
                currentSeries.ChartType = currentType;
                currentSeries.Points.Clear();
            }


            if (DataGridUsers.SelectedItem is User currentUsers && ChoiceCar.SelectedItem is Ticket currentCars)
            {
                var filterBasket = Baskets.Where(b => b.ID_Ticket == currentCars.ID_Ticket);

                foreach (var order in filterBasket.Where(b => b.Order.User.ID_User == currentUsers.ID_User && b.Order.data >= StartDate.SelectedDate.Value && b.Order.data <= EndDate.SelectedDate.Value))
                {
                    currentSeries.Points.AddXY($"{order.Order.data.Value.Day}.{order.Order.data.Value.Month}.{order.Order.data.Value.Year}\n{order.Order.User.LastName} {order.Order.User.Name}", order.Order.allPrice);
                }
            }
            else if (DataGridUsers.SelectedItem is User onlyUser)
            {
                var filterUsers = Orders.Where(o => o.User.ID_User == onlyUser.ID_User && o.data >= StartDate.SelectedDate.Value && o.data <= EndDate.SelectedDate.Value);

                foreach (var order in filterUsers)
                {
                    currentSeries.Points.AddXY($"{order.data.Value.Day}.{order.data.Value.Month}.{order.data.Value.Year}\n{order.User.LastName} {order.User.Name}", order.allPrice);
                }
            }
            else if (ChoiceCar.SelectedItem is Ticket onlyCar)
            {
                var filterBasket = Baskets.Where(b => b.ID_Ticket == onlyCar.ID_Ticket && b.Order.data >= StartDate.SelectedDate.Value && b.Order.data <= EndDate.SelectedDate.Value);
                foreach (var order in filterBasket)
                {
                    currentSeries.Points.AddXY($"{order.Order.data.Value.Day}.{order.Order.data.Value.Month}.{order.Order.data.Value.Year}\n{order.Order.User.LastName} {order.Order.User.Name}", order.Order.allPrice);
                }
            }
            else
            {
                if (StartDate.SelectedDate != null && EndDate.SelectedDate != null)
                {
                    var filterDate = Orders.Where(o => o.data >= StartDate.SelectedDate.Value && o.data <= EndDate.SelectedDate.Value);
                    foreach (var order in filterDate)
                    {
                        currentSeries.Points.AddXY($"{order.data.Value.Day}.{order.data.Value.Month}.{order.data.Value.Year}\n{order.User.LastName} {order.User.Name}", order.allPrice);
                    }
                }
                else
                {
                    foreach (var order in Orders)
                    {
                        currentSeries.Points.AddXY($"{order.data.Value.Day}.{order.data.Value.Month}.{order.data.Value.Year}\n{order.User.LastName} {order.User.Name}", order.allPrice);
                    }
                }
            }
        }
    }
}