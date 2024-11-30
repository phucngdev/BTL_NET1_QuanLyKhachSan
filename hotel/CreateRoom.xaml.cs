using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using hotel.config;
using Microsoft.Win32;

namespace hotel
{
    /// <summary>
    /// Interaction logic for CreateRoom.xaml
    /// </summary>
    public partial class CreateRoom : Page
    {
       

        public CreateRoom()
        {
            InitializeComponent();
        }

       
        // click chọn ảnh
        private void BrowseImage_Click(object sender, RoutedEventArgs e)
        {
            // mở cửa sổ file của window
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                // chỉ lấy ảnh có đuôi
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // lấy đường dẫn ảnh và hiển thị vào textbox
                txtImagePath.Text = openFileDialog.FileName; 
            }
        }
        // click clear 
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear tất cả textbox
            txtRoomName.Text = string.Empty;
            cmbRoomType.SelectedIndex = -1;
            txtDescription.Text = string.Empty;
            txtCapacity.Text = string.Empty;
            txtPrice.Text = string.Empty;
            cmbFloor.SelectedIndex = -1;
            txtImagePath.Text = string.Empty;
            txtStatus.Text = "Available";
        }
        // click save
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // gọi hàm save
            SaveRoom();
            // quay về trang danh sách phòng
            NavigationService?.Navigate(new ManageRoom());
        }
        // hàm lưu phòng
        private void SaveRoom()
        {
            // lấy thông tin nhập vào
            string roomName = txtRoomName.Text;
            string roomType = ((ComboBoxItem)cmbRoomType.SelectedItem)?.Content.ToString();
            // kiểm tra xem ô diện tích phòng có phải kiểu int không           
            if (!int.TryParse(txtCapacity.Text, out int capacity))
            {
                MessageBox.Show("Invalid Capacity. Please enter a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // kiểm tra xem ô giá phòng có phải kiểu decimal ko
            if (!decimal.TryParse(txtPrice.Text, out decimal pricePerNight))
            {
                MessageBox.Show("Invalid Price. Please enter a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            string status = "Available";
            string description = txtDescription.Text;
            string bedNumber = txtBedNumber.Text;
            string floor = ((ComboBoxItem)cmbFloor.SelectedItem)?.Content.ToString();
            string image = txtImagePath.Text;
            // kiểm tra xem tên, loại phòng, tầng có null ko
            if (string.IsNullOrEmpty(roomName) || string.IsNullOrEmpty(roomType) || string.IsNullOrEmpty(floor))
            {
                MessageBox.Show("Please fill in all required fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // 
                using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    // connect db
                    conn.Open();
                    // câu truy vấn
                    string query = @"INSERT INTO Rooms (RoomName, RoomType, Capacity, PricePerNight, Status, Description, Floor, Image, BedNumber) 
                                     VALUES (@RoomName, @RoomType, @Capacity, @PricePerNight, @Status, @Description, @Floor, @Image, @BedNumber)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // truyền giá trị vào câu query
                        cmd.Parameters.AddWithValue("@RoomName", roomName);
                        cmd.Parameters.AddWithValue("@RoomType", roomType);
                        cmd.Parameters.AddWithValue("@Capacity", capacity);
                        cmd.Parameters.AddWithValue("@PricePerNight", pricePerNight);
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@Floor", floor);
                        cmd.Parameters.AddWithValue("@Image", image);
                        cmd.Parameters.AddWithValue("@BedNumber", bedNumber);
                        // thực hiện truy vấn
                        int rows = cmd.ExecuteNonQuery();
                        // kiểm tra thành công
                        if (rows > 0)
                        {
                            MessageBox.Show("Room added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            ClearFields();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add room. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // clear các textbox và combobox
        private void ClearFields()
        {
            txtRoomName.Clear();
            cmbRoomType.SelectedIndex = -1;
            txtCapacity.Clear();
            txtPrice.Clear();
            txtDescription.Clear();
            cmbFloor.SelectedIndex = -1;
            txtImagePath.Clear();
        }
    }
}
