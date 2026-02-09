using System;
using System.Linq;
using System.Windows;
using LogisticsWPF.Model;

namespace LogisticsWPF.Windows
{
    public partial class SupplyOrderEditWindow : Window
    {
        private int? orderId;

        public SupplyOrderEditWindow(int? orderId = null)
        {
            InitializeComponent();
            this.orderId = orderId;
            LoadCombos();

            if (orderId.HasValue)
            {
                LoadOrderData();
            }
            else
            {
                // Устанавливаем дату по умолчанию для нового заказа
                OrderDatePicker.SelectedDate = DateTime.Now;
            }
        }

        private void LoadCombos()
        {
            using (var context = new SmartLogisticsEntities())
            {
                VendorComboBox.ItemsSource = context.Vendors.ToList();
                StorageComboBox.ItemsSource = context.Storages.ToList();
                StatusComboBox.ItemsSource = context.SupplyStatuses.ToList();

                // Загружаем сотрудников, создавая новое свойство FullName для отображения
                StaffComboBox.ItemsSource = context.Staff
                    .Select(s => new {
                        s.StaffID,
                        FullName = s.Surname + " " + s.FirstName
                    })
                    .ToList();
            }
        }

        private void LoadOrderData()
        {
            using (var context = new SmartLogisticsEntities())
            {
                var order = context.SupplyOrders.Find(orderId.Value);
                if (order == null)
                {
                    MessageBox.Show("Заказ не найден.");
                    this.Close();
                    return;
                }

                VendorComboBox.SelectedValue = order.VendorID;
                StorageComboBox.SelectedValue = order.StorageID;
                StaffComboBox.SelectedValue = order.StaffID;
                StatusComboBox.SelectedValue = order.StatusID;
                OrderDatePicker.SelectedDate = order.OrderDate;
                DeliveryDatePicker.SelectedDate = order.DeliveryDate;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // --- Валидация ---
            if (VendorComboBox.SelectedValue == null || StorageComboBox.SelectedValue == null ||
                StaffComboBox.SelectedValue == null || StatusComboBox.SelectedValue == null)
            {
                MessageBox.Show("Необходимо заполнить все выпадающие списки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!OrderDatePicker.SelectedDate.HasValue || !DeliveryDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Необходимо указать дату заказа и дату доставки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (DeliveryDatePicker.SelectedDate.Value < OrderDatePicker.SelectedDate.Value)
            {
                MessageBox.Show("Дата доставки не может быть раньше даты заказа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new SmartLogisticsEntities())
            {
                SupplyOrders order;

                if (orderId.HasValue)
                {
                    order = context.SupplyOrders.Find(orderId.Value);
                    if (order == null) return;
                }
                else
                {
                    order = new SupplyOrders();
                    context.SupplyOrders.Add(order);
                }

                // --- Сохранение данных ---
                order.VendorID = (int)VendorComboBox.SelectedValue;
                order.StorageID = (int)StorageComboBox.SelectedValue;
                order.StaffID = (int)StaffComboBox.SelectedValue;
                order.StatusID = (int)StatusComboBox.SelectedValue;
                order.OrderDate = OrderDatePicker.SelectedDate.Value;
                order.DeliveryDate = DeliveryDatePicker.SelectedDate.Value;

                context.SaveChanges();
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
