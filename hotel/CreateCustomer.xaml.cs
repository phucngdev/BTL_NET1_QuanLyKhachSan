using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using hotel.config;

namespace hotel
{
    public partial class CreateCustomer : Page
    {
        public CreateCustomer()
        {
            InitializeComponent();
        }

        private void SaveCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();
            string dateOfBirthText = txtDateOfBirth.Text.Trim();
            string cccd = txtCCCD.Text.Trim();

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone) ||
                string.IsNullOrEmpty(address) || string.IsNullOrEmpty(dateOfBirthText) || string.IsNullOrEmpty(cccd))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DateTime.TryParse(dateOfBirthText, out DateTime dateOfBirth))
            {
                MessageBox.Show("Ngày sinh không hợp lệ. Vui lòng nhập theo định dạng ngày/tháng/năm.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO Customers (FullName, Email, Phone, Address, DateOfBirth, CCCD) 
                        VALUES (@FullName, @Email, @Phone, @Address, @DateOfBirth, @CCCD)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FullName", fullName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                        cmd.Parameters.AddWithValue("@CCCD", cccd);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Khách hàng đã được thêm mới thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearFields();
                NavigationService.Navigate(new ListCustomer());

            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearFields()
        {
            txtFullName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtDateOfBirth.Text = string.Empty;
            txtCCCD.Text = string.Empty;
        }

        private void ClearCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }
    }
}
