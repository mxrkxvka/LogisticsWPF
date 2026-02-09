using LogisticsWPF.Model;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LogisticsWPF.Windows;

namespace LogisticsWPF.Pages
{
    public partial class PurchaseOrdersPage : Page
    {
        public PurchaseOrdersPage()
        {
            InitializeComponent();
            Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            using (var context = new SmartLogisticsEntities())
            {
                var orders = context.SupplyOrders
                    .Include(o => o.Vendors)
                    .Include(o => o.Storages)
                    .Include(o => o.SupplyStatuses)
                    .Select(o => new
                    {
                        o.OrderID,
                        VendorName = o.Vendors.CompanyTitle,
                        StorageName = o.Storages.Title,
                        o.OrderDate,
                        o.DeliveryDate,
                        StatusName = o.SupplyStatuses.StatusName
                    })
                    .ToList();

                OrdersGrid.ItemsSource = orders;
            }
        }

        private int? GetSelectedOrderId()
        {
            if (OrdersGrid.SelectedItem == null)
                return null;

            dynamic item = OrdersGrid.SelectedItem;
            return (int?)item.OrderID;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var win = new PurchaseOrderEditWindow();
            if (win.ShowDialog() == true)
                LoadData();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            int? id = GetSelectedOrderId();
            if (id == null)
            {
                MessageBox.Show("Выберите закупку для редактирования",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var win = new PurchaseOrderEditWindow(id.Value);
            if (win.ShowDialog() == true)
                LoadData();
        }

        private void Receipt_Click(object sender, RoutedEventArgs e)
        {
            var receiptWindow = new InventoryReceiptWindow();

            receiptWindow.ShowDialog();
        }



        private void StockBalances_Click(object sender, RoutedEventArgs e)
        {
            if (Parent is Frame frame)
            {
                frame.Navigate(new StockBalancesPage());
            }
            else
            {
                new Window
                {
                    Title = "Остатки на складе",
                    Content = new StockBalancesPage(),
                    Width = 900,
                    Height = 600,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                }.Show();
            }
        }
    }
}
