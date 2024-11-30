using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using hotel.config;

namespace hotel
{
    public partial class CreateEmployee : Page
    {
        public CreateEmployee()
        {
            InitializeComponent();
        }

        // Lưu nhân viên vào cơ sở dữ liệu
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string password = txtPassword.Password.Trim();
            string address = txtAddress.Text.Trim();
            string position = (cmbPosition.SelectedItem as ComboBoxItem)?.Content.ToString();
            decimal salary;

            // Kiểm tra các trường dữ liệu
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(position) ||
                !decimal.TryParse(txtSalary.Text, out salary))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Kết nối cơ sở dữ liệu và thực hiện lưu thông tin
                string query = "INSERT INTO Employees (FullName, Email, Phone, Password, Address, Position, Salary) " +
                               "VALUES (@FullName, @Email, @Phone, @Password, @Address, @Position, @Salary)";

                using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@Position", position);
                    cmd.Parameters.AddWithValue("@Salary", salary);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Thêm nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi xảy ra khi lưu nhân viên.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    NavigationService?.Navigate(new ManageEmployee());

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Xóa form
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        // Hàm xóa thông tin trong form
        private void ClearForm()
        {
            txtFullName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtPassword.Clear();
            txtAddress.Clear();
            cmbPosition.SelectedIndex = -1;
            txtSalary.Clear();
        }

       
    }
}
