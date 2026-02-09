using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using LogisticsWPF.Model;
using LogisticsWPF.Windows;

namespace LogisticsWPF.Pages
{
    public partial class MaterialsPage : Page
    {
        public MaterialsPage()
        {
            InitializeComponent();
            Loaded += (s, e) => LoadMaterials();
        }

        private void LoadMaterials()
        {
            try
            {
                using (var context = new SmartLogisticsEntities())
                {
                    var materials = context.Components
                        .Include(m => m.ComponentTypes)
                        .Include(m => m.Measures)
                        .Select(m => new
                        {
                            m.ComponentID,
                            m.SerialCode,
                            m.Title,
                            ComponentTypeName = m.ComponentTypes != null ? m.ComponentTypes.TypeName : "—",
                            MeasureName = m.Measures != null ? m.Measures.Name : "—",
                            m.MinBalance,
                            m.Price
                        })
                        .ToList();

                    MaterialsDataGrid.ItemsSource = materials;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки склада: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int? GetSelectedMaterialId()
        {
            if (MaterialsDataGrid.SelectedItem == null) return null;

            dynamic item = MaterialsDataGrid.SelectedItem;
            return (int?)item.ComponentID;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new MaterialEditWindow();
            if (editWindow.ShowDialog() == true)
                LoadMaterials();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            int? id = GetSelectedMaterialId();
            if (id == null)
            {
                MessageBox.Show("Выберите позицию для изменения", "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var editWindow = new MaterialEditWindow(id);
            if (editWindow.ShowDialog() == true)
                LoadMaterials();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int? id = GetSelectedMaterialId();
            if (id == null) return;

            if (MessageBox.Show("Удалить выбранный компонент из справочника?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var context = new SmartLogisticsEntities())
                {
                    var material = context.Components.Find(id.Value);
                    if (material != null)
                    {
                        context.Components.Remove(material);
                        context.SaveChanges();
                        LoadMaterials();
                    }
                }
            }
        }
    }
}