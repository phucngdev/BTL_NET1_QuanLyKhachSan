using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using hotel.config;
using hotel.data;
using hotel.models;

namespace hotel
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {

        public Login()
        {
            InitializeComponent();
        }
        // khi nhập thì ẩn hoặc hiện placeholder
        private void txtEmailOrPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            placeholderEmailOrPhone.Visibility = string.IsNullOrEmpty(txtEmailOrPhone.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        // khi nhập thì ẩn hoặc hiện placeholder
        private void txtPasscode_PasswordChanged(object sender, RoutedEventArgs e)
        {
            placeholderPasscode.Visibility = string.IsNullOrEmpty(txtPasscode.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        // click đăng nhập
        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            // lấy thông tin nhập vào
            string emailOrPhone = txtEmailOrPhone.Text.Trim();
            string passcode = txtPasscode.Password;
            // kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(emailOrPhone) || string.IsNullOrEmpty(passcode))
            {
                MessageBox.Show("Please enter both email/phone and passcode.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // kiểm tra người dùng có tồn tại không
                if (ValidateLogin(emailOrPhone, passcode, out Employee employee))
                {
                    // Lưu thông tin người dùng vào UserSession
                    UserSession.Instance.SetLoggedInEmployee(employee);
                    // tạo vào hiển thị mainwindow 
                    MainWindow mainwindow = new MainWindow();
                    mainwindow.Show();
                    this.Close(); // đóng window đăng nhập
                }

                else
                {
                    MessageBox.Show("Invalid email/phone or passcode.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateLogin(string emailOrPhone, string password, out Employee employee)
        {
            employee = null; // clear thông tin employee
            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                // connect db
                connection.Open();

                string query = @"
            SELECT * 
            FROM Employees 
            WHERE (Email = @Identifier OR Phone = @Identifier) 
            AND Password = @Password";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // thêm tham số vào query
                    command.Parameters.AddWithValue("@Identifier", emailOrPhone);
                    command.Parameters.AddWithValue("@Password", password);
                    // thực hiện truy vấn
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // đọc kết quả
                        {
                            // Tạo đối tượng Employee từ dữ liệu đọc được
                            employee = new Employee
                            {
                                EmployeeID = (int)reader["EmployeeID"],
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Password = reader["Password"].ToString(),
                                Address = reader["Address"].ToString(),
                                Position = reader["Position"].ToString(),
                                Salary = (decimal)reader["Salary"]
                            };
                            return true;
                        }
                    }
                }
            }
            return false;
        }


    }
}
