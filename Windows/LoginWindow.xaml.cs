using System.Linq;
using System.Windows;
using LogisticsWPF.Model;
using System.Data.Entity; // <-- Добавьте это для использования .Include

namespace LogisticsWPF.Windows
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            LoginTextBox.Focus(); // Устанавливаем фокус на поле логина при запуске
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль.",
                                "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new SmartLogisticsEntities())
                {
                    // Явно подгружаем данные сотрудника, чтобы избежать ошибок
                    var user = context.AppUsers
                        .Include(u => u.Staff)
                        .FirstOrDefault(u => u.LoginName == login && u.PasswordKey == password);

                    if (user != null)
                    {
                        // Сохраняем пользователя для доступа в других частях приложения
                        Application.Current.Properties["CurrentUser"] = user;

                        string fullName = (user.Staff != null)
                            ? $"{user.Staff.Surname} {user.Staff.FirstName}"
                            : user.LoginName;

                        MessageBox.Show($"Добро пожаловать, {fullName}!",
                                        "Успешный вход", MessageBoxButton.OK, MessageBoxImage.Information);

                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль. Пожалуйста, попробуйте снова.",
                                        "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Произошла ошибка при подключении к базе данных: " + ex.Message,
                                "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
