using System;
using System.Windows;
using System.Data.SqlClient;
using Microsoft.Win32;
using hotel.models;
using hotel.config;

namespace hotel
{
    public partial class AddDeviceWindow : Window
    {
        private int _roomId;  // Biến để lưu RoomID

        // Constructor nhận RoomID từ cửa sổ gọi
        public AddDeviceWindow(int roomId)
        {
            InitializeComponent();
            _roomId = roomId;  // Lưu RoomID
            InstallDatePicker.SelectedDate = DateTime.Now; // Mặc định chọn thời gian hiện tại
        }

        // Xử lý sự kiện khi nhấn nút "Browse" để chọn hình ảnh
        private void BrowseImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImagePathTextBox.Text = openFileDialog.FileName; // Set the selected file path to TextBox
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra các trường hợp đầu vào
            if (string.IsNullOrWhiteSpace(DeviceNameTextBox.Text) || string.IsNullOrWhiteSpace(QuantityTextBox.Text))
            {
                MessageBox.Show("Device Name and Quantity are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Tạo đối tượng Device
                Device newDevice = new Device
                {
                    RoomID = _roomId,  // Sử dụng RoomID đã truyền vào
                    DeviceName = DeviceNameTextBox.Text,
                    Quantity = int.Parse(QuantityTextBox.Text),
                    Description = DescriptionTextBox.Text,
                    InstallDate = InstallDatePicker.SelectedDate ?? DateTime.Now,  // Lấy ngày cài đặt từ DatePicker
                    Image = ImagePathTextBox.Text
                };

                string query = @"
                INSERT INTO Devices (RoomID, DeviceName, Quantity, Description, InstallDate, Image)
                VALUES (@RoomID, @DeviceName, @Quantity, @Description, @InstallDate, @Image)";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Cung cấp tham số để tránh SQL Injection
                        command.Parameters.AddWithValue("@RoomID", newDevice.RoomID);
                        command.Parameters.AddWithValue("@DeviceName", newDevice.DeviceName);
                        command.Parameters.AddWithValue("@Quantity", newDevice.Quantity);
                        command.Parameters.AddWithValue("@Description", newDevice.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@InstallDate", newDevice.InstallDate);
                        command.Parameters.AddWithValue("@Image", newDevice.Image ?? (object)DBNull.Value);

                        command.ExecuteNonQuery();
                    }
                }

                // Đóng cửa sổ và thông báo thành công
                MessageBox.Show("Device added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add device. Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

   
}
