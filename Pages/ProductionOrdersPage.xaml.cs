using LogisticsWPF.Model;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LogisticsWPF.Pages
{
    public partial class ProductionOrdersPage : Page
    {
        public ProductionOrdersPage()
        {
            InitializeComponent();
            Loaded += (s, e) => LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                using (var context = new SmartLogisticsEntities())
                {
                    var orders = context.Items
                        .Include(i => i.Categories)
                        .Include(i => i.ItemStatuses)
                        .Select(i => new
                        {
                            i.ItemID,
                            ProductName = i.FullTitle,
                            Category = i.Categories.Title,
                            Status = i.ItemStatuses.StatusName,
                            SKU = i.SKU
                        })
                        .ToList();

                    OrdersGrid.ItemsSource = orders;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int? GetSelectedItemId()
        {
            if (OrdersGrid.SelectedItem == null)
                return null;

            dynamic item = OrdersGrid.SelectedItem;
            return (int?)item.ItemID;
        }

        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Добавление производственного заказа пока не реализовано",
                "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            int? id = GetSelectedItemId();
            if (id == null)
            {
                MessageBox.Show("Выберите изделие", "Инфо",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBox.Show($"Редактирование изделия ID = {id}",
                "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateStatus_Click(object sender, RoutedEventArgs e)
        {
            int? id = GetSelectedItemId();
            if (id == null)
            {
                MessageBox.Show("Выберите изделие для изменения статуса",
                    "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBox.Show($"Изменение статуса изделия ID = {id}",
                "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
