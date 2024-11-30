using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using hotel.config;
using hotel.models;

namespace hotel
{
    public partial class EditService : Page
    {
        public Service ServiceToEdit { get; set; } // biến lưu thông tin service

        // 
        public EditService(Service service)
        {
            InitializeComponent();
            ServiceToEdit = service; // lấy thông tin service trang trước truyền vào gán cho biến đã tạo

            // hiển thị thông tin 
            txtServiceName.Text = service.ServiceName;
            txtDescription.Text = service.Description;
            txtPrice.Text = service.Price.ToString("0.00");
        }

        public event Action ServiceUpdated;

        // Click lưu
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(txtServiceName.Text) || string.IsNullOrEmpty(txtDescription.Text) || string.IsNullOrEmpty(txtPrice.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            // Update dữ liệu mới 
            ServiceToEdit.ServiceName = txtServiceName.Text;
            ServiceToEdit.Description = txtDescription.Text;
            ServiceToEdit.Price = decimal.TryParse(txtPrice.Text, out decimal price) ? price : 0;
            // câu truy vấn
            string query = @"UPDATE Services SET ServiceName = @ServiceName, Description = @Description, Price = @Price WHERE ServiceID = @ServiceID";

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    // connect db
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    // thêm các tham số vào query
                    command.Parameters.AddWithValue("@ServiceName", ServiceToEdit.ServiceName);
                    command.Parameters.AddWithValue("@Description", ServiceToEdit.Description);
                    command.Parameters.AddWithValue("@Price", ServiceToEdit.Price);
                    command.Parameters.AddWithValue("@ServiceID", ServiceToEdit.ServiceID);
                    // thực hiện truy vấn
                    int rowsAffected = command.ExecuteNonQuery();
                    // kiểm tra kết quả
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Service updated successfully.");
                        
                        ServiceUpdated?.Invoke(); 

                        NavigationService.GoBack(); // quay lại trang trước
                    }
                    else
                    {
                        MessageBox.Show("Error updating service.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // Click clear
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear các textbox
            txtServiceName.Clear();
            txtDescription.Clear();
            txtPrice.Clear();
        }

        // xóa service
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // hiển thị hộp tùy chọn và lấy kết quả
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this service?", "Delete Service", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            // nếu chọn xóa
            if (result == MessageBoxResult.Yes)
            {
                string query = @"DELETE FROM Services WHERE ServiceID = @ServiceID";

                try
                {
                    using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                    {
                        // connect db
                        connection.Open();
                        SqlCommand command = new SqlCommand(query, connection);
                        // thêm tham số id service vào query
                        command.Parameters.AddWithValue("@ServiceID", ServiceToEdit.ServiceID);
                        // thực hiện truy vấn
                        int rowsAffected = command.ExecuteNonQuery();
                        // kiểm tra kq
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Service deleted successfully.");
                            ServiceUpdated?.Invoke();
                            NavigationService.GoBack();// quay về trang trước
                        }
                        else
                        {
                            MessageBox.Show("Error deleting service.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

    }
}
