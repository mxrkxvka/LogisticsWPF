using LogisticsWPF.Model;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LogisticsWPF.Windows;

namespace LogisticsWPF.Pages
{
    public partial class ProductPage : Page
    {
        public ProductPage()
        {
            InitializeComponent();
            Loaded += (s, e) => LoadProducts();
        }

        private void LoadProducts()
        {
            using (var context = new SmartLogisticsEntities())
            {
                var products = context.Items
                    .Include(i => i.Categories)
                    .Include(i => i.Measures)
                    .Include(i => i.ItemStatuses)
                    .Select(i => new
                    {
                        i.ItemID,
                        i.SKU,
                        i.FullTitle,
                        i.Details,
                        CategoryName = i.Categories.Title,
                        MeasureName = i.Measures.Name,
                        StatusName = i.ItemStatuses.StatusName
                    })
                    .ToList();

                ProductsDataGrid.ItemsSource = products;
            }
        }

        private int? GetSelectedItemId()
        {
            if (ProductsDataGrid.SelectedItem == null)
                return null;

            dynamic item = ProductsDataGrid.SelectedItem;
            return (int?)item.ItemID;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new ProductEditWindow();
            if (win.ShowDialog() == true)
                LoadProducts();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            int? id = GetSelectedItemId();
            if (id == null)
            {
                MessageBox.Show("Выберите продукцию для редактирования",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var win = new ProductEditWindow(id.Value);
            if (win.ShowDialog() == true)
                LoadProducts();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int? id = GetSelectedItemId();
            if (id == null) return;

            if (MessageBox.Show("Удалить выбранную продукцию?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                using (var context = new SmartLogisticsEntities())
                {
                    var item = context.Items.Find(id.Value);
                    if (item != null)
                    {
                        context.Items.Remove(item);
                        context.SaveChanges();
                    }
                }
                LoadProducts();
            }
        }
    }
}
