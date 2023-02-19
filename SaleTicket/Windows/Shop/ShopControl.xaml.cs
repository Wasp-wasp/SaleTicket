using SaleTicket.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace SaleTicket
{
    public partial class ShopControl : UserControl
    {
        public List<Ticket> Tickets{ get; set; }
        public List<Destination> Destinations { get; set; }

        private List<Ticket> TicketsHandler { get; set; }
        private List<int> IdTicket { get; set; }
        private List<Basket> BasketTicket { get; set; }

        private int IndexItemCatalog = 0;
        private double additionsSum;

        private int IdUser = 0;
        private List<Ticket> HandlerBasketTicket { get; set; }

        public ShopControl(int user)
        {
            InitializeComponent();

            IdUser = user;
            Tickets = new List<Ticket>();
            TicketsHandler = new List<Ticket>();
            HandlerBasketTicket = new List<Ticket>();

            Destinations = new List<Destination>();
            IdTicket = new List<int>();

            using (var context = new stariyEntities1())
            {
                context.Configuration.ProxyCreationEnabled = false;
                Tickets = context.Ticket.ToList();
                Destinations = context.Destination.ToList();
            }

            PointOfDepartureComboBox.ItemsSource = Destinations;
            PointDestinationComboBox.ItemsSource = Destinations;

            foreach (var ticket in Tickets)
            {
                foreach (var destination in Destinations)
                {
                    if (ticket.ID_PointOfDeparture == destination.ID_Destination)
                        ticket.TicketPointOfDeparture = destination.TypeDestination;
                    
                    if(ticket.ID_PointDestination == destination.ID_Destination)
                        ticket.TicketPointDestination = destination.TypeDestination;
                }
            }
            ProductCatalog.ItemsSource = Tickets;
        }
        private void OneProductDisplay_Click(object sender, RoutedEventArgs e)
        {
            DataTemplate template = (DataTemplate)this.Resources["listTemplateBig"];
            ProductCatalog.ItemTemplate = template;

            ButtonHadlerOneProductDisplay.Visibility = Visibility.Visible;
            ButtonHadlerFourProductDisplay.Visibility = Visibility.Hidden;
            ClearSearchPanels();
        }

        private void FourProductDisplay_Click(object sender, RoutedEventArgs e)
        {
            DataTemplate template = (DataTemplate)this.Resources["listTemplateSmall"];
            ProductCatalog.ItemTemplate = template;

            ButtonHadlerOneProductDisplay.Visibility = Visibility.Hidden;
            ButtonHadlerFourProductDisplay.Visibility = Visibility.Visible;
            ClearSearchPanels();
        }

        private void ListProductDisplay_Click(object sender, RoutedEventArgs e)
        {
            DataTemplate template = (DataTemplate)this.Resources["listTemplateSmall"];
            ProductCatalog.ItemTemplate = template;

            ButtonHadlerOneProductDisplay.Visibility = Visibility.Hidden;
            ButtonHadlerFourProductDisplay.Visibility = Visibility.Hidden;
            ClearSearchPanels();
        }

        // Событие корзины
        private void ButtonChoice_Click(object sender, RoutedEventArgs e)
        {
            Ticket curItem = (Ticket)((ListBoxItem)ProductCatalog.ContainerFromElement((Button)sender)).Content;
            double temp = Convert.ToDouble(QuentytiProduct.Text);
            additionsSum = (double)(temp + curItem.Price);
            QuentytiProduct.Text = $"{additionsSum:0.00}";
            SumOrder.Text = $"{additionsSum:0.00}";

            HandlerBasketTicket.Add(curItem);
        }

        // Функция по выгрузке в лист айдишников машин 
        private void LoadIdFromIndex()
        {
            IdTicket.Clear();
            IndexItemCatalog = 0;
            foreach (var item in TicketsHandler)
            {
                IdTicket.Add(item.ID_Ticket);
            }
        }

        // Событие переключения отображения ОДНОГО товаров
        private void ButtonPreviousOneProduct_Click(object sender, RoutedEventArgs e)
        {
            if (IndexItemCatalog < 1)
                IndexItemCatalog = 0;
            else
                IndexItemCatalog--;

            //var carsItems = ProductCatalog.ItemsSource.Cast<Cars>();
            var filtered = TicketsHandler.Where(c => c.ID_Ticket == IdTicket[IndexItemCatalog]);
            ProductCatalog.ItemsSource = filtered;
        }

        private void ButtonNextOneProduct_Click(object sender, RoutedEventArgs e)
        {
            //var carsItems = ProductCatalog.ItemsSource.Cast<Cars>();

            IndexItemCatalog++;

            if (IndexItemCatalog < IdTicket.Count())
            {
                var filtered = TicketsHandler.Where(c => c.ID_Ticket == IdTicket[IndexItemCatalog]);
                ProductCatalog.ItemsSource = filtered;
            }
            else
                IndexItemCatalog--;
        }


        // Событие переключения отображения ЧЕТЫРЕХ товаров
        private void ButtonPreviousFourProduct_Click(object sender, RoutedEventArgs e)
        {
            int numberForCorrectDisplayDuringTransitions = 5;
            int numberToDisplayWhenEmptyClicks = 3;
            int numberForAccessingProductsByIndex = 3;

            if (IndexItemCatalog - numberForAccessingProductsByIndex >= 0)
            {
                IndexItemCatalog -= numberForAccessingProductsByIndex;
            }

            if (IndexItemCatalog - numberForAccessingProductsByIndex >= 0)
            {
                var filtered = TicketsHandler.Where(u => u.ID_Ticket > IdTicket[IndexItemCatalog] && u.ID_Ticket < IdTicket[IndexItemCatalog + numberForCorrectDisplayDuringTransitions]);
                ProductCatalog.ItemsSource = filtered;
            }
            else
            {
                if (IndexItemCatalog + numberToDisplayWhenEmptyClicks < IdTicket.Count())
                {
                    var filtered = TicketsHandler.Where(u => u.ID_Ticket >= IdTicket[IndexItemCatalog] && u.ID_Ticket <= IdTicket[IndexItemCatalog + numberToDisplayWhenEmptyClicks]);
                    ProductCatalog.ItemsSource = filtered;
                }
                else
                {
                    var filtered = TicketsHandler.Where(u => u.ID_Ticket >= IdTicket[IndexItemCatalog] && u.ID_Ticket <= IdTicket[IdTicket.Count() - 1]);
                    ProductCatalog.ItemsSource = filtered;
                }
            }

        }

        private void ButtonNextFourProduct_Click(object sender, RoutedEventArgs e)
        {
            int numberForCorrectDisplayDuringTransitions = 5;
            int numberToDisplayFinalProducts = 1;
            int numberForAccessingProductsByIndex = 3;

            if (IndexItemCatalog + numberForAccessingProductsByIndex < IdTicket.Count() - 1)
            {
                IndexItemCatalog += numberForAccessingProductsByIndex;
                if (IndexItemCatalog + numberForAccessingProductsByIndex > IdTicket.Count() - 1)
                {
                    IndexItemCatalog -= numberForAccessingProductsByIndex;
                }
            }

            if (IndexItemCatalog + numberForCorrectDisplayDuringTransitions < IdTicket.Count())
            {
                var filtered = TicketsHandler.Where(u => u.ID_Ticket > IdTicket[IndexItemCatalog] && u.ID_Ticket < IdTicket[IndexItemCatalog + numberForCorrectDisplayDuringTransitions]);
                ProductCatalog.ItemsSource = filtered;
            }
            else
            {
                if (IndexItemCatalog + numberToDisplayFinalProducts < IdTicket.Count() - 1)
                {
                    var filtered = TicketsHandler.Where(u => u.ID_Ticket > IdTicket[IndexItemCatalog + numberToDisplayFinalProducts] && u.ID_Ticket <= IdTicket[IdTicket.Count() - 1]);
                    ProductCatalog.ItemsSource = filtered;
                }
                else
                {
                    var filtered = TicketsHandler.Where(u => u.ID_Ticket >= IdTicket[IndexItemCatalog] && u.ID_Ticket <= IdTicket[IdTicket.Count() - 1]);
                    ProductCatalog.ItemsSource = filtered;
                }
            }
        }

        // Выбор бренда и открытие выбора модели машины
        private void PointOfDepartureComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = PointOfDepartureComboBox.SelectedIndex;
            if (selectedIndex > -1)
            {
                Destination selectedItem = (Destination)PointOfDepartureComboBox.SelectedItem;
                var filtered = TicketsHandler.Where(c => c.TicketPointOfDeparture.StartsWith(selectedItem.TypeDestination));
                ProductCatalog.ItemsSource = filtered;
                LoadIdFromIndex();
            }
        }

        private void PointDestinationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = PointDestinationComboBox.SelectedIndex;
            if (selectedIndex > -1)
            {
                Destination selectedItem = (Destination)PointDestinationComboBox.SelectedItem;
                var filtered = TicketsHandler.Where(c => c.TicketPointDestination.StartsWith(selectedItem.TypeDestination));
                ProductCatalog.ItemsSource = filtered;
                LoadIdFromIndex();
            }
        }

        // Переключение значений слайдеров цены и событие поиска
        private void SliderStart_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).SelectionEnd = e.NewValue;
            TextSliderStart.Text = $"{e.NewValue:0.00}";
        }

        private void SliderEnd_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).SelectionEnd = e.NewValue;
            TextSliderEnd.Text = $"{e.NewValue:0.00}";
        }

        private void SearchPrice_Click(object sender, RoutedEventArgs e)
        {
            var filtered = Tickets.Where(c => c.Price > Convert.ToDouble(TextSliderStart.Text) && c.Price < Convert.ToDouble(TextSliderEnd.Text));
            ProductCatalog.ItemsSource = filtered;
        }

        // Событие отчищениия поиска
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearSearchPanels();
        }

        // Функция отчистки поиска
        private void ClearSearchPanels()
        {
            PointOfDepartureComboBox.SelectedIndex = -1;
            PointDestinationComboBox.SelectedIndex = -1;

            ProductCatalog.ItemsSource = Tickets;

            TicketsHandler = (List<Ticket>)ProductCatalog.ItemsSource.Cast<Ticket>();


            if (OneProductDisplay.IsChecked == true)
            {
                var filtered = Tickets.Where(c => c.ID_Ticket == 1);
                ProductCatalog.ItemsSource = filtered;
            }
            else if (FourProductDisplay.IsChecked == true)
            {
                var filtered = Tickets.Where(u => u.ID_Ticket >= 1 && u.ID_Ticket < 5);
                ProductCatalog.ItemsSource = filtered;
            }

            LoadIdFromIndex();

            SliderStart.Value = 0;
            SliderEnd.Value = 10000;
            TextSliderStart.Text = "0,00";
            TextSliderEnd.Text = "10000,00";
        }

        // Переход к корзине
        private void MoveToBasket_Click(object sender, RoutedEventArgs e)
        {
            BasketGird.Visibility = Visibility.Visible;
            CatalogGrid.Visibility = Visibility.Hidden;
            BasketCatalog.ItemsSource = HandlerBasketTicket;
        }

        /*Функции и методы, которые относятся к корзине товаров*/

        private void BackToCatalog_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BasketGird.Visibility = Visibility.Hidden;
            CatalogGrid.Visibility = Visibility.Visible;
        }

        // Событие создания заказа
        private void MakeOrder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (HandlerBasketTicket.Count != 0)
            {
                try
                {
                    using (var context = new stariyEntities1())
                    {
                        var datetime = DateTime.Now;
                        var onlydate = datetime.Date;
                        Order newOrder = new Order()
                        {
                            ID_User = IdUser,
                            allPrice = additionsSum,
                            data = onlydate
                        };
                        context.Order.Add(newOrder);
                        context.SaveChanges();

                        var getOrder = context.Order.SingleOrDefault(o => o.ID_User == IdUser && o.allPrice == additionsSum && o.data == onlydate);

                        foreach (var item in HandlerBasketTicket)
                        {
                            Basket productToBasket = new Basket
                            {
                                ID_Ticket = item.ID_Ticket,
                                ID_Order = getOrder.ID_Order
                            };
                            context.Basket.Add(productToBasket);
                        }
                        context.SaveChanges();
                    }
                    MessageBox.Show("Заказ оформлен дон");

                    additionsSum = 0;
                    QuentytiProduct.Text = $"{additionsSum:0.00}";
                    SumOrder.Text = $"{additionsSum:0.00}";

                    HandlerBasketTicket.Clear();
                    BasketCatalog.ItemsSource = HandlerBasketTicket.ToList();
                }
                catch
                {
                    MessageBox.Show("Произшла непредвиденная ошибка, помолитесь");
                }

            }
            else
                MessageBox.Show("Товаров нет в корзине");
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            Ticket curItem = (Ticket)((ListBoxItem)BasketCatalog.ContainerFromElement((Button)sender)).Content;

            var newProducts = HandlerBasketTicket.Where(u => u.ID_Ticket != curItem.ID_Ticket);

            double temp = Convert.ToDouble(QuentytiProduct.Text);
            additionsSum = (double)(temp - curItem.Price);
            QuentytiProduct.Text = $"{additionsSum:0.00}";
            SumOrder.Text = $"{additionsSum:0.00}";

            HandlerBasketTicket.Remove(curItem);
            BasketCatalog.ItemsSource = newProducts;
        }
    }
}