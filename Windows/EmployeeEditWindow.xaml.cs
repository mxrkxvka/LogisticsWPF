using LogisticsWPF.Model;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace LogisticsWPF.Windows
{
    public partial class EmployeeEditWindow : Window
    {
        private int? staffId;

        public EmployeeEditWindow(int? staffId = null)
        {
            InitializeComponent();
            this.staffId = staffId;
            LoadCombos();

            if (staffId.HasValue)
            {
                LoadStaffData();
            }
        }

        private void LoadCombos()
        {
            using (var context = new SmartLogisticsEntities())
            {
                JobTitleComboBox.ItemsSource = context.JobTitles.ToList();
                DepartmentComboBox.ItemsSource = context.Departments.ToList();
                SkillLevelComboBox.ItemsSource = context.SkillLevels.ToList();
                PaymentTypeComboBox.ItemsSource = context.PaymentTypes.ToList();
                StatusComboBox.ItemsSource = context.StaffStatuses.ToList();
            }
        }

        private void LoadStaffData()
        {
            using (var context = new SmartLogisticsEntities())
            {
                var staff = context.Staff.Find(staffId.Value);
                if (staff == null)
                {
                    MessageBox.Show("Сотрудник не найден.");
                    this.Close();
                    return;
                }

                SurnameTextBox.Text = staff.Surname;
                NameTextBox.Text = staff.FirstName;
                PatronymicTextBox.Text = staff.Patronymic;
                PhoneTextBox.Text = staff.Phone;
                JobTitleComboBox.SelectedValue = staff.JobTitleID;
                DepartmentComboBox.SelectedValue = staff.DeptID;
                SkillLevelComboBox.SelectedValue = staff.LevelID;
                PaymentTypeComboBox.SelectedValue = staff.PaymentTypeID;
                StatusComboBox.SelectedValue = staff.StatusID;
            }
        }

        // Исправлены регулярные выражения
        private bool IsValidName(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && Regex.IsMatch(value, @"^[A-Za-zА-Яа-яЁё\-]+$");
        }

        private bool IsValidPhone(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && Regex.IsMatch(value, @"^[0-9]+$");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidName(SurnameTextBox.Text) || !IsValidName(NameTextBox.Text))
            {
                MessageBox.Show("Фамилия и имя обязательны и должны содержать только буквы.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrWhiteSpace(PatronymicTextBox.Text) && !IsValidName(PatronymicTextBox.Text))
            {
                MessageBox.Show("Отчество должно содержать только буквы.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsValidPhone(PhoneTextBox.Text))
            {
                MessageBox.Show("Телефон обязателен и должен содержать только цифры.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (JobTitleComboBox.SelectedValue == null || DepartmentComboBox.SelectedValue == null || SkillLevelComboBox.SelectedValue == null || PaymentTypeComboBox.SelectedValue == null || StatusComboBox.SelectedValue == null)
            {
                MessageBox.Show("Необходимо заполнить все выпадающие списки.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new SmartLogisticsEntities())
            {
                Staff staff;

                if (staffId.HasValue)
                {
                    staff = context.Staff.Find(staffId.Value);
                    if (staff == null) return;
                }
                else
                {
                    staff = new Staff();
                    context.Staff.Add(staff);
                }

                staff.Surname = SurnameTextBox.Text.Trim();
                staff.FirstName = NameTextBox.Text.Trim();
                staff.Patronymic = PatronymicTextBox.Text.Trim();
                staff.Phone = PhoneTextBox.Text.Trim();
                staff.JobTitleID = (int)JobTitleComboBox.SelectedValue;
                staff.DeptID = (int)DepartmentComboBox.SelectedValue;
                staff.LevelID = (int)SkillLevelComboBox.SelectedValue;
                staff.PaymentTypeID = (int)PaymentTypeComboBox.SelectedValue;
                staff.StatusID = (int)StatusComboBox.SelectedValue;

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
