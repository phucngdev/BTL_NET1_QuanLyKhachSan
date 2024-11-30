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
using hotel.models;
using Microsoft.Win32;

namespace hotel
{
    /// <summary>
    /// Interaction logic for EditRoom.xaml
    /// </summary>
    public partial class EditRoom : Page
    {
        // biến lưu thông tin room được chọn
        private Room _room;
        // lấy thông tin room do page trước truyền vào
        public EditRoom(Room selectedRoom)
        {
            InitializeComponent();
            _room = selectedRoom; // gán thông tin ,page trước truyền vào biến đã tạo

            // Hiển thị thông tin phòng
            RoomNameTextBox.Text = _room.Roomname;
            RoomTypeComboBox.Text = _room.RoomType;
            CapacityTextBox.Text = _room.Capacity.ToString();
            PriceTextBox.Text = _room.PricePerNight.ToString("F2");
            DescriptionTextBox.Text = _room.Description;
            StatusTextBox.Text = _room.Status;
            BedNumberTextBox.Text = _room.BedNumber.ToString();

            // Hiển thị tầng
            FloorComboBox.Text = _room.Floor;

            // Hiển thị ảnh
            if (!string.IsNullOrEmpty(_room.Image))
            {
                ImagePathTextBox.Text = _room.Image;
                RoomImage.Source = new BitmapImage(new Uri(_room.Image, UriKind.Absolute));
            }
        }

        // chọn ảnh 
        private void BrowseImageButton_Click(object sender, RoutedEventArgs e)
        {
            // mở file của window
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                // chỉ lấy ảnh có đuôi
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // lấy đường dẫn file và gán vào textbox
                string selectedFile = openFileDialog.FileName;
                ImagePathTextBox.Text = selectedFile;
                // hiển thị ảnh
                RoomImage.Source = new BitmapImage(new Uri(selectedFile, UriKind.Absolute));

                // Cập nhật ảnh vào đối tượng _room
                _room.Image = selectedFile;
            }
        }



        // lưu thông tin chỉnh sửa
        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(RoomNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(CapacityTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PriceTextBox.Text) ||
                    string.IsNullOrWhiteSpace(BedNumberTextBox.Text) ||
                    string.IsNullOrWhiteSpace(ImagePathTextBox.Text))
                {
                    throw new ArgumentException("Please fill in all required fields, including the image.");
                }

                if (!int.TryParse(CapacityTextBox.Text, out int capacity))
                {
                    throw new FormatException("Capacity must be a valid number.");
                }

                if (!decimal.TryParse(PriceTextBox.Text, out decimal pricePerNight))
                {
                    throw new FormatException("Price per night must be a valid decimal number.");
                }

                if (!int.TryParse(BedNumberTextBox.Text, out int bedNumber))
                {
                    throw new FormatException("Bed number must be a valid number.");
                }

                // Kiểm tra xem file ảnh có tồn tại không
                if (!System.IO.File.Exists(ImagePathTextBox.Text))
                {
                    throw new ArgumentException("The selected image file does not exist.");
                }

                // Cập nhật thông tin từ giao diện vào đối tượng
                _room.Roomname = RoomNameTextBox.Text;
                _room.RoomType = (RoomTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                _room.Capacity = capacity;
                _room.PricePerNight = pricePerNight;
                _room.Description = DescriptionTextBox.Text;
                _room.Status = StatusTextBox.Text;
                _room.Floor = (FloorComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                _room.BedNumber = bedNumber;
                _room.Image = ImagePathTextBox.Text; // Thêm đường dẫn ảnh

                // Gọi phương thức lưu vào cơ sở dữ liệu
                UpdateRoomInDatabase(_room);
                MessageBox.Show("Room details updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.Navigate(new RoomDetail(_room)); // quay về trang roomDetail

            }
            catch (FormatException ex)
            {
                MessageBox.Show($"Input error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Validation error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // update 
        private void UpdateRoomInDatabase(Room room)
        {
            // câu truy vấn
            string query = @"
        UPDATE Rooms
        SET 
            RoomType = @RoomType,
            Capacity = @Capacity,
            PricePerNight = @PricePerNight,
            Status = @Status,
            Description = @Description,
            Image = @Image,
            Floor = @Floor,
            Roomname = @Roomname,
            BedNumber = @BedNumber
        WHERE RoomID = @RoomID";

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                // connect
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Thêm các tham số
                    command.Parameters.AddWithValue("@RoomID", room.RoomID);
                    command.Parameters.AddWithValue("@RoomType", room.RoomType);
                    command.Parameters.AddWithValue("@Capacity", room.Capacity);
                    command.Parameters.AddWithValue("@PricePerNight", room.PricePerNight);
                    command.Parameters.AddWithValue("@Status", room.Status);
                    command.Parameters.AddWithValue("@Description", room.Description);
                    command.Parameters.AddWithValue("@Image", room.Image ?? string.Empty); // Nếu không có ảnh, để trống
                    command.Parameters.AddWithValue("@Floor", room.Floor);
                    command.Parameters.AddWithValue("@Roomname", room.Roomname);
                    command.Parameters.AddWithValue("@BedNumber", room.BedNumber);

                    // Thực thi lệnh
                    int rowsAffected = command.ExecuteNonQuery();
                    // kiểm tra thất bại
                    if (rowsAffected <= 0)
                    {
                        throw new Exception("No rows were updated. Please check if the RoomID exists.");
                    }
                }
            }
        }

        // click quay lại
        private void Back_click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RoomDetail(_room)); // quay lại trang roomDetail

        }
    }
}
