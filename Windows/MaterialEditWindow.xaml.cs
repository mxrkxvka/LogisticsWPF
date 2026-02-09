using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using LogisticsWPF.Model;

namespace LogisticsWPF.Windows
{
    public partial class MaterialEditWindow : Window
    {
        // В коде это ID компонента
        private int? componentId;

        public MaterialEditWindow(int? componentId = null)
        {
            InitializeComponent();
            this.componentId = componentId;
            LoadComboboxes();

            if (componentId.HasValue)
            {
                LoadComponentData();
            }
        }

        private void LoadComboboxes()
        {
            using (var context = new SmartLogisticsEntities())
            {
                // Загружаем типы из ComponentTypes и единицы из Measures
                TypeComboBox.ItemsSource = context.ComponentTypes.ToList();
                MeasureComboBox.ItemsSource = context.Measures.ToList();
            }
        }

        private void LoadComponentData()
        {
            using (var context = new SmartLogisticsEntities())
            {
                var component = context.Components.Find(componentId.Value);
                if (component != null)
                {
                    SerialCodeTextBox.Text = component.SerialCode;
                    TitleTextBox.Text = component.Title;
                    TypeComboBox.SelectedValue = component.TypeID;
                    MeasureComboBox.SelectedValue = component.MeasureID;
                    // Используем CultureInfo.InvariantCulture для корректного преобразования decimal в строку
                    MinBalanceTextBox.Text = component.MinBalance?.ToString(CultureInfo.InvariantCulture);
                    PriceTextBox.Text = component.Price?.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // --- Валидация ---
            string serialCode = SerialCodeTextBox.Text.Trim();
            string title = TitleTextBox.Text.Trim();

            if (string.IsNullOrEmpty(serialCode) || string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Код и наименование не могут быть пустыми.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (TypeComboBox.SelectedValue == null || MeasureComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите тип и единицу измерения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Используем NumberStyles.Float и CultureInfo.InvariantCulture для парсинга
            if (!decimal.TryParse(MinBalanceTextBox.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal minBalance))
            {
                MessageBox.Show("Минимальный остаток должен быть числом. В качестве разделителя используйте точку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!decimal.TryParse(PriceTextBox.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal price))
            {
                MessageBox.Show("Цена должна быть числом. В качестве разделителя используйте точку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new SmartLogisticsEntities())
            {
                // Проверка на уникальность серийного кода
                bool codeExists = context.Components
                    .Any(c => c.SerialCode == serialCode && (!componentId.HasValue || c.ComponentID != componentId.Value));
                if (codeExists)
                {
                    MessageBox.Show("Компонент с таким серийным кодом уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Components component;
                if (componentId.HasValue)
                {
                    component = context.Components.Find(componentId.Value);
                    if (component == null) return;
                }
                else
                {
                    component = new Components();
                    context.Components.Add(component);
                }

                // --- Сохранение данных ---
                component.SerialCode = serialCode;
                component.Title = title;
                component.TypeID = (int)TypeComboBox.SelectedValue;
                component.MeasureID = (int)MeasureComboBox.SelectedValue;
                component.MinBalance = minBalance;
                component.Price = price;

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
