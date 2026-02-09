using System;
using System.Linq;
using System.Windows;
using LogisticsWPF.Model;

namespace LogisticsWPF.Windows
{
    public partial class SupplierEditWindow : Window
    {
        // ID из таблицы Vendors
        private int? vendorId;

        public SupplierEditWindow(int? vendorId = null)
        {
            InitializeComponent();
            this.vendorId = vendorId;

            if (vendorId.HasValue)
                LoadVendorData();
        }

        private void LoadVendorData()
        {
            using (var context = new SmartLogisticsEntities())
            {
                var vendor = context.Vendors.Find(vendorId.Value);
                if (vendor != null)
                {
                    TaxNumberTextBox.Text = vendor.TaxNumber;
                    CompanyTitleTextBox.Text = vendor.CompanyTitle;
                    EmailTextBox.Text = vendor.ContactEmail;
                    RatingTextBox.Text = vendor.Rating?.ToString();
                    PayConditionsTextBox.Text = vendor.PayConditions;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // --- Валидация ---
            string taxNumber = TaxNumberTextBox.Text.Trim();
            string companyTitle = CompanyTitleTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string payConditions = PayConditionsTextBox.Text.Trim();
            string ratingText = RatingTextBox.Text.Trim();

            if (string.IsNullOrEmpty(taxNumber) || string.IsNullOrEmpty(companyTitle))
            {
                MessageBox.Show("ИНН и Наименование компании являются обязательными полями.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!taxNumber.All(char.IsDigit))
            {
                MessageBox.Show("ИНН должен содержать только цифры.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrEmpty(email) && !email.Contains("@"))
            {
                MessageBox.Show("Введен некорректный Email.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int? rating = null;
            if (!string.IsNullOrEmpty(ratingText))
            {
                if (!int.TryParse(ratingText, out int r) || r < 0 || r > 5)
                {
                    MessageBox.Show("Рейтинг должен быть целым числом от 0 до 5.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                rating = r;
            }

            using (var context = new SmartLogisticsEntities())
            {
                // Проверка на уникальность ИНН
                bool taxNumberExists = context.Vendors.Any(v =>
                    v.TaxNumber == taxNumber && (!vendorId.HasValue || v.VendorID != vendorId.Value));

                if (taxNumberExists)
                {
                    MessageBox.Show("Поставщик с таким ИНН уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Vendors vendor;
                if (vendorId.HasValue)
                {
                    vendor = context.Vendors.Find(vendorId.Value);
                    if (vendor == null) return;
                }
                else
                {
                    vendor = new Vendors();
                    context.Vendors.Add(vendor);
                }

                // --- Сохранение данных ---
                vendor.TaxNumber = taxNumber;
                vendor.CompanyTitle = companyTitle;
                vendor.ContactEmail = email;
                vendor.Rating = rating;
                vendor.PayConditions = payConditions;

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
