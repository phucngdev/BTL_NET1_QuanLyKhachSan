using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using hotel.config;

namespace hotel
{
    public partial class EditEmployee : Page
    {
        private int employeeId; // ID nhân viên cần chỉnh sửa

        public EditEmployee(int id)
        {
            InitializeComponent();
            employeeId = id; // lấy id nhân viên 
            LoadEmployeeDetails(); // tải thông tin nhân viên
        }

        private void LoadEmployeeDetails()
        {
            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                // mở kết nối
                connection.Open();
                // câu truy vấn
                string query = "SELECT FullName, Email, Phone, Address, Position, Salary, Password FROM Employees WHERE EmployeeID = @EmployeeID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // thêm tham số cho query
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);
                    // thực hiện truy vấn
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // đọc truy vấn
                        {
                            // hiển thị ra giao diện
                            txtFullName.Text = reader.GetString(0);
                            txtEmail.Text = reader.GetString(1);
                            txtPhone.Text = reader.GetString(2);
                            txtAddress.Text = reader.GetString(3);
                            cmbPosition.Text = reader.GetString(4);
                            txtSalary.Text = reader.GetDecimal(5).ToString("F2");
                            txtPassword.Password = reader.GetString(6); 
                        }
                        else
                        {
                            // nếu không có dữ liệu thì hiển thị
                            MessageBox.Show("Không tìm thấy thông tin nhân viên.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        // hàm lưu thông tin thay đổi
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open(); // mở kết nối
                // câu truy vấn
                string query = "UPDATE Employees SET FullName = @FullName, Email = @Email, Phone = @Phone, Address = @Address, Position = @Position, Salary = @Salary, Password = @Password WHERE EmployeeID = @EmployeeID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // thêm tham số
                    command.Parameters.AddWithValue("@FullName", txtFullName.Text);
                    command.Parameters.AddWithValue("@Email", txtEmail.Text);
                    command.Parameters.AddWithValue("@Phone", txtPhone.Text);
                    command.Parameters.AddWithValue("@Address", txtAddress.Text);
                    command.Parameters.AddWithValue("@Position", cmbPosition.Text);
                    command.Parameters.AddWithValue("@Salary", decimal.Parse(txtSalary.Text));
                    command.Parameters.AddWithValue("@Password", txtPassword.Password); // Lưu mật khẩu
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);
                    // thực hiện truy vấn
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0) // kiểm tra truy vấn thành công ko
                    {
                        MessageBox.Show("Cập nhật thông tin nhân viên thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        NavigationService?.Navigate(new ManageEmployee());

                    }
                    else
                    {
                        MessageBox.Show("Cập nhật thông tin nhân viên thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        // hàm clear các textbox
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            txtFullName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            cmbPosition.SelectedIndex = -1;
            txtSalary.Clear();
            txtPassword.Clear();
        }

        // hàm xóa nhân viên
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // hiển thị thông báo xác nhận
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa nhân viên này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes) // nếu chọn yes
            {
                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    connection.Open(); // mở kết nối
                    // câu truy vấn
                    string query = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // thêm tham số
                        command.Parameters.AddWithValue("@EmployeeID", employeeId);
                        int rowsAffected = command.ExecuteNonQuery(); // thực hiện truy vấn
                        if (rowsAffected > 0) // kiểm tra truy vấn thành công
                        {
                            MessageBox.Show("Xóa nhân viên thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            NavigationService?.Navigate(new ManageEmployee());

                        }
                        else
                        {
                            MessageBox.Show("Xóa nhân viên thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }
    }
}
