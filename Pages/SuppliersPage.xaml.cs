using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LogisticsWPF.Model;
using LogisticsWPF.Windows;

namespace LogisticsWPF.Pages
{
    public partial class SuppliersPage : Page
    {
        public SuppliersPage()
        {
            InitializeComponent();
            Loaded += (s, e) => LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            using (var context = new SmartLogisticsEntities())
            {
                var suppliers = context.Vendors
                    .Select(v => new
                    {
                        v.VendorID,
                        v.TaxNumber,
                        v.CompanyTitle,
                        v.ContactEmail,
                        v.Rating,
                        v.PayConditions
                    })
                    .ToList();

                SuppliersDataGrid.ItemsSource = suppliers;
            }
        }

        private int? GetSelectedVendorId()
        {
            if (SuppliersDataGrid.SelectedItem == null)
                return null;

            dynamic item = SuppliersDataGrid.SelectedItem;
            return (int?)item.VendorID;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new SupplierEditWindow();
            if (win.ShowDialog() == true)
                LoadSuppliers();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            int? id = GetSelectedVendorId();
            if (id == null)
            {
                MessageBox.Show("Выберите поставщика для редактирования",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var win = new SupplierEditWindow(id.Value);
            if (win.ShowDialog() == true)
                LoadSuppliers();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int? id = GetSelectedVendorId();
            if (id == null) return;

            if (MessageBox.Show("Удалить выбранного поставщика?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                using (var context = new SmartLogisticsEntities())
                {
                    var vendor = context.Vendors.Find(id.Value);
                    if (vendor != null)
                    {
                        context.Vendors.Remove(vendor);
                        context.SaveChanges();
                    }
                }
                LoadSuppliers();
            }
        }
    }
}
