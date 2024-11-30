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

namespace hotel
{
    /// <summary>
    /// Interaction logic for CreateServiceWindow.xaml
    /// </summary>
    public partial class CreateServiceWindow : Window
    {
        public CreateServiceWindow()
        {
            InitializeComponent();
        }

        // click clear
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear các textbox 
            txtServiceName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtPrice.Text = string.Empty;
        }

        // click save
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // gọi hàm save
            SaveService();
            // đổi trạng thái dialog = true, để bên danh sách tải lại dữ liệu 
            this.DialogResult = true;
            // đóng window hiện tại
            this.Close();
        }

        // tạo mới dịch vụ
        private void SaveService()
        {
            // lấy dữ liệu nhập vào
            string serviceName = txtServiceName.Text;
            string description = txtDescription.Text;
            // kiểm tra giá có phải kiểu decimal không
            if (!decimal.TryParse(txtPrice.Text, out decimal price))
            {
                MessageBox.Show("Invalid Price. Please enter a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // kiểm tra tên có null ko
            if (string.IsNullOrEmpty(serviceName))
            {
                MessageBox.Show("Please fill in all required fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    // connect db
                    conn.Open();
                    // câu truy vấn
                    string query = @"INSERT INTO Services (ServiceName, Price, Description) 
                                     VALUES (@ServiceName, @Price, @Description)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // gán giá trị vào câu truy vấn
                        cmd.Parameters.AddWithValue("@ServiceName", serviceName);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@Description", description);
                        // thực hiện truy vấn
                        int rows = cmd.ExecuteNonQuery();
                        // kiểm tra thành công
                        if (rows > 0)
                        {
                            MessageBox.Show("Service added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            ClearFields();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add service. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // clear các textbox
        private void ClearFields()
        {
            txtServiceName.Clear();
            txtPrice.Clear();
            txtDescription.Clear();
        }
    }
}
