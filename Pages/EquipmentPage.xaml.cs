using LogisticsWPF.Model;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LogisticsWPF.Windows;

namespace LogisticsWPF.Pages
{
    public partial class EquipmentPage : Page
    {
        public EquipmentPage()
        {
            InitializeComponent();
            Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var context = new SmartLogisticsEntities())
                {
                    EquipmentGrid.ItemsSource = context.Equipment
                        .Include(e => e.EquipModels)
                        .Include(e => e.EquipTypes)
                        .Include(e => e.EquipStatuses)
                        .ToList();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var win = new EquipmentEditWindow();
            if (win.ShowDialog() == true)
                LoadData();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentGrid.SelectedItem is Equipment selectedEq)
            {
                var win = new EquipmentEditWindow(selectedEq.EquipmentID);
                if (win.ShowDialog() == true)
                    LoadData();
            }
            else
            {
                MessageBox.Show("Выберите объект из списка для внесения изменений.",
                                "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!(EquipmentGrid.SelectedItem is Equipment selectedEq)) return;

            var result = MessageBox.Show($"Вы уверены, что хотите списать оборудование {selectedEq.InventoryNumber}?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new SmartLogisticsEntities())
                    {
                        var entity = context.Equipment.Find(selectedEq.EquipmentID);
                        if (entity != null)
                        {
                            context.Equipment.Remove(entity);
                            context.SaveChanges();
                            LoadData();
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Невозможно удалить объект, так как он используется в логах или записях производства.");
                }
            }
        }
    }
}