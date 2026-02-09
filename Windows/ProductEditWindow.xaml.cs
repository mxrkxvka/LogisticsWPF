using System.Linq;
using System.Windows;
using LogisticsWPF.Model;

namespace LogisticsWPF.Windows
{
    public partial class ProductEditWindow : Window
    {
        // В коде это ID из таблицы Items
        private int? itemId;

        public ProductEditWindow(int? itemId = null)
        {
            InitializeComponent();
            this.itemId = itemId;
            LoadComboboxes();

            if (itemId.HasValue)
            {
                LoadItemData();
            }
        }

        private void LoadComboboxes()
        {
            using (var context = new SmartLogisticsEntities())
            {
                // Загружаем данные из правильных справочников
                CategoryComboBox.ItemsSource = context.Categories.ToList();
                MeasureComboBox.ItemsSource = context.Measures.ToList();
                StatusComboBox.ItemsSource = context.ItemStatuses.ToList();
            }
        }

        private void LoadItemData()
        {
            using (var context = new SmartLogisticsEntities())
            {
                var item = context.Items.Find(itemId.Value);
                if (item != null)
                {
                    SkuTextBox.Text = item.SKU;
                    FullTitleTextBox.Text = item.FullTitle;
                    DetailsTextBox.Text = item.Details;
                    CategoryComboBox.SelectedValue = item.CategoryID;
                    MeasureComboBox.SelectedValue = item.MeasureID;
                    StatusComboBox.SelectedValue = item.StatusID;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // --- Валидация ---
            string sku = SkuTextBox.Text.Trim();
            string fullTitle = FullTitleTextBox.Text.Trim();

            if (string.IsNullOrEmpty(sku) || string.IsNullOrEmpty(fullTitle))
            {
                MessageBox.Show("Артикул и наименование не могут быть пустыми.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (CategoryComboBox.SelectedValue == null || MeasureComboBox.SelectedValue == null || StatusComboBox.SelectedValue == null)
            {
                MessageBox.Show("Необходимо выбрать категорию, единицу измерения и статус.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new SmartLogisticsEntities())
            {
                // Проверка на уникальность SKU
                bool skuExists = context.Items
                    .Any(i => i.SKU == sku && (!itemId.HasValue || i.ItemID != itemId.Value));

                if (skuExists)
                {
                    MessageBox.Show("Продукция с таким артикулом (SKU) уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Items item;
                if (itemId.HasValue)
                {
                    item = context.Items.Find(itemId.Value);
                    if (item == null) return;
                }
                else
                {
                    item = new Items();
                    context.Items.Add(item);
                }

                // --- Сохранение данных ---
                item.SKU = sku;
                item.FullTitle = fullTitle;
                item.Details = DetailsTextBox.Text.Trim();
                item.CategoryID = (int)CategoryComboBox.SelectedValue;
                item.MeasureID = (int)MeasureComboBox.SelectedValue;
                item.StatusID = (int)StatusComboBox.SelectedValue;

                context.SaveChanges();
            }

            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
