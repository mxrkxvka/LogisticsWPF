using LogisticsWPF.Model;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LogisticsWPF.Windows;

namespace LogisticsWPF.Pages
{
    public partial class EmployeesPage : Page
    {
        public EmployeesPage()
        {
            InitializeComponent();
            Loaded += (_, __) => LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var context = new SmartLogisticsEntities())
                {
                    // Загружаем данные из таблицы Staff со всеми связями
                    EmployeesGrid.ItemsSource = context.Staff
                        .Include(s => s.JobTitles)
                        .Include(s => s.Departments)
                        .Include(s => s.SkillLevels)
                        .Include(s => s.PaymentTypes)
                        .Include(s => s.StaffStatuses)
                        .ToList();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var window = new EmployeeEditWindow();
            if (window.ShowDialog() == true)
                LoadData();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            // Используем класс Staff вместо Employees
            if (EmployeesGrid.SelectedItem is Staff selectedStaff)
            {
                var window = new EmployeeEditWindow(selectedStaff.StaffID);
                if (window.ShowDialog() == true)
                    LoadData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника из списка",
                                "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!(EmployeesGrid.SelectedItem is Staff selectedStaff))
                return;

            var result = MessageBox.Show($"Удалить сотрудника {selectedStaff.Surname}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = new SmartLogisticsEntities())
                {
                    var entity = context.Staff.Find(selectedStaff.StaffID);
                    if (entity != null)
                    {
                        context.Staff.Remove(entity);
                        context.SaveChanges();
                        LoadData();
                    }
                }
            }
        }
    }
}