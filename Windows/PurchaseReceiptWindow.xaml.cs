using LogisticsWPF.Model;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace LogisticsWPF.Windows
{
    public partial class InventoryReceiptWindow : Window
    {
        public InventoryReceiptWindow()
        {
            InitializeComponent();
            LoadCombos();
        }

        private void LoadCombos()
        {
            using (var context = new SmartLogisticsEntities())
            {
                ComponentComboBox.ItemsSource = context.Components.ToList();
                StorageComboBox.ItemsSource = context.Storages.ToList();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // --- Валидация ---
            if (ComponentComboBox.SelectedValue == null || StorageComboBox.SelectedValue == null)
            {
                MessageBox.Show("Пожалуйста, выберите компонент и склад.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(QuantityTextBox.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal quantityToAdd))
            {
                MessageBox.Show("Количество должно быть числом. В качестве разделителя используйте точку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (quantityToAdd <= 0)
            {
                MessageBox.Show("Количество для добавления должно быть больше нуля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int selectedComponentId = (int)ComponentComboBox.SelectedValue;
            int selectedStorageId = (int)StorageComboBox.SelectedValue;

            using (var context = new SmartLogisticsEntities())
            {
                // Ищем существующую запись об остатках для данного компонента на данном складе
                var inventoryItem = context.CurrentInventory.FirstOrDefault(inv =>
                    inv.ComponentID == selectedComponentId &&
                    inv.StorageID == selectedStorageId);

                if (inventoryItem != null)
                {
                    // Если запись найдена, просто увеличиваем количество
                    inventoryItem.Quantity += quantityToAdd;
                }
                else
                {
                    // Если такой записи нет, создаем новую
                    inventoryItem = new CurrentInventory
                    {
                        ComponentID = selectedComponentId,
                        StorageID = selectedStorageId,
                        Quantity = quantityToAdd
                    };
                    context.CurrentInventory.Add(inventoryItem);
                }

                context.SaveChanges();
                MessageBox.Show($"Товар '{((Components)ComponentComboBox.SelectedItem).Title}' успешно оприходован на склад.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
