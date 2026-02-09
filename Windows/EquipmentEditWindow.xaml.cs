using LogisticsWPF.Model;
using System;
using System.Linq;
using System.Windows;

namespace LogisticsWPF.Windows
{
    public partial class EquipmentEditWindow : Window
    {
        private int? equipmentId;

        public EquipmentEditWindow(int? equipmentId = null)
        {
            InitializeComponent();
            this.equipmentId = equipmentId;
            LoadCombos();

            if (equipmentId.HasValue)
            {
                LoadEquipment();
            }
        }

        private void LoadCombos()
        {
            using (var context = new SmartLogisticsEntities())
            {
                ModelComboBox.ItemsSource = context.EquipModels.ToList();
                TypeComboBox.ItemsSource = context.EquipTypes.ToList();
                StatusComboBox.ItemsSource = context.EquipStatuses.ToList();
            }
        }

        private void LoadEquipment()
        {
            using (var context = new SmartLogisticsEntities())
            {
                var eq = context.Equipment.Find(equipmentId.Value);
                if (eq == null)
                {
                    MessageBox.Show("Оборудование не найдено.");
                    this.Close();
                    return;
                }

                InventoryNumberTextBox.Text = eq.InventoryNumber;
                ModelComboBox.SelectedValue = eq.ModelID;
                TypeComboBox.SelectedValue = eq.TypeID;
                StatusComboBox.SelectedValue = eq.StatusID;
                CommissionDatePicker.SelectedDate = eq.CommissionDate;
                MaintenanceScheduleTextBox.Text = eq.MaintenanceSchedule;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InventoryNumberTextBox.Text))
            {
                MessageBox.Show("Инвентарный номер является обязательным полем.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (ModelComboBox.SelectedValue == null || TypeComboBox.SelectedValue == null || StatusComboBox.SelectedValue == null)
            {
                MessageBox.Show("Необходимо заполнить поля 'Модель', 'Тип' и 'Статус'.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!CommissionDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Укажите дату ввода в эксплуатацию.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new SmartLogisticsEntities())
            {
                string inventoryNumber = InventoryNumberTextBox.Text.Trim();

                // Проверка на уникальность инвентарного номера
                bool exists = context.Equipment.Any(eq =>
                    eq.InventoryNumber == inventoryNumber &&
                    (!equipmentId.HasValue || eq.EquipmentID != equipmentId.Value));

                if (exists)
                {
                    MessageBox.Show("Оборудование с таким инвентарным номером уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Equipment equipment;

                if (equipmentId.HasValue)
                {
                    equipment = context.Equipment.Find(equipmentId.Value);
                    if (equipment == null) return;
                }
                else
                {
                    equipment = new Equipment();
                    context.Equipment.Add(equipment);
                }

                equipment.InventoryNumber = inventoryNumber;
                equipment.ModelID = (int)ModelComboBox.SelectedValue;
                equipment.TypeID = (int)TypeComboBox.SelectedValue;
                equipment.StatusID = (int)StatusComboBox.SelectedValue;
                equipment.CommissionDate = CommissionDatePicker.SelectedDate.Value;
                equipment.MaintenanceSchedule = MaintenanceScheduleTextBox.Text.Trim();

                context.SaveChanges();
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
