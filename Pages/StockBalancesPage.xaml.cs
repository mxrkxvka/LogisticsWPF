using LogisticsWPF.Model;
using System.Data.Entity;
using System.Linq;
using System.Windows.Controls;

namespace LogisticsWPF.Pages
{
    public partial class StockBalancesPage : Page
    {
        public StockBalancesPage()
        {
            InitializeComponent();
            Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            using (var context = new SmartLogisticsEntities())
            {
                var stock = context.CurrentInventory
                    .Include(ci => ci.Storages)
                    .Include(ci => ci.Components.Measures)
                    .Select(ci => new
                    {
                        ci.InventoryID,
                        StorageName = ci.Storages.Title,
                        ComponentName = ci.Components.Title,
                        ci.Quantity,
                        MeasureName = ci.Components.Measures.Name
                    })
                    .ToList();

                StockGrid.ItemsSource = stock;
            }
        }
    }
}
