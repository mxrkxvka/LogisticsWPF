using LogisticsWPF.Model;
using System.Linq;
using System.Windows;
using System.Data.Entity; // Убедитесь, что эта директива using добавлена

namespace LogisticsWPF.Windows
{
    public partial class SupplyOrderStatusWindow : Window
    {
        private int orderId;

        public SupplyOrderStatusWindow(int orderId)
        {
            InitializeComponent();
            this.orderId = orderId;
            LoadOrder();
        }

        private void LoadOrder()
        {
            using (var context = new SmartLogisticsEntities())
            {
                // Находим заказ и явно подгружаем связанный статус
                var order = context.SupplyOrders
                    .Include(o => o.SupplyStatuses) // Навигационное свойство к статусу
                    .FirstOrDefault(o => o.OrderID == orderId);

                if (order == null)
                {
                    MessageBox.Show("Заказ не найден.");
                    this.Close();
                    return;
                }

                // Отображаем имя текущего статуса
                CurrentStatusText.Text = order.SupplyStatuses?.StatusName ?? "Не определен";

                // Загружаем все возможные статусы в ComboBox
                StatusComboBox.ItemsSource = context.SupplyStatuses.ToList();
                // Устанавливаем текущий статус как выбранный
                StatusComboBox.SelectedValue = order.StatusID;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (StatusComboBox.SelectedValue == null)
            {
                MessageBox.Show("Пожалуйста, выберите новый статус.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int newStatusId = (int)StatusComboBox.SelectedValue;

            using (var context = new SmartLogisticsEntities())
            {
                var order = context.SupplyOrders.Find(orderId);
                if (order == null) return;

                if (order.StatusID == newStatusId)
                {
                    MessageBox.Show("Выбранный статус совпадает с текущим. Изменения не требуются.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Обновляем статус
                order.StatusID = newStatusId;
                context.SaveChanges();
            }

            DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
