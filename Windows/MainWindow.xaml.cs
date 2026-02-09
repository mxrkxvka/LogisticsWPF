using System.Windows;
using LogisticsWPF.Pages;
using LogisticsWPF.Model;
using System.Data.Entity;
using System.Linq;

namespace LogisticsWPF.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadUserData();

            MainFrame.Navigate(new SuppliersPage());
        }

        private void LoadUserData()
        {
            if (Application.Current.Properties["CurrentUser"] is AppUsers currentUser)
            {
                using (var context = new SmartLogisticsEntities())
                {
                    var user = context.AppUsers
                        .Include(u => u.Staff)
                        .Include(u => u.AppRoles)
                        .FirstOrDefault(u => u.UserID == currentUser.UserID);

                    if (user != null)
                    {
                        UserNameTextBlock.Text = (user.Staff != null)
                            ? $"{user.Staff.Surname} {user.Staff.FirstName}"
                            : user.LoginName;
                        UserRoleTextBlock.Text = user.AppRoles?.RoleName ?? "Роль не определена";
                    }
                    else
                    {
                        GoToLogin();
                    }
                }
            }
            else
            {
                GoToLogin();
            }
        }

        private void ProductsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductPage());
        }

        private void ComponentsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MaterialsPage()); 
        }

        private void SuppliersButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SuppliersPage());
        }

        private void EmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EmployeesPage()); 
        }

        private void EquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EquipmentPage());
        }

        private void SupplyOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PurchaseOrdersPage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            GoToLogin();
        }

        private void GoToLogin()
        {
            Application.Current.Properties["CurrentUser"] = null;
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}
