using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using hotel.config;
using hotel.models;

namespace hotel
{
    /// <summary>
    /// Interaction logic for RoomDetail.xaml
    /// </summary>
    public partial class RoomDetail : Page
    {
        public ObservableCollection<Device> Devices { get; set; }
        public ObservableCollection<Device> SelectedDevices { get; set; } = new ObservableCollection<Device>();



        public RoomDetail(Room selectedRoom)
        {
            InitializeComponent();

            // Set DataContext to the selected room
            this.DataContext = selectedRoom;
            Devices = new ObservableCollection<Device>(GetDevicesForRoom(selectedRoom.RoomID));

            // Bind Devices collection to ItemsControl
            this.DevicesControl.ItemsSource = Devices;

            // Display room details
            RoomNameTextBlock.Text = $"Tên phòng: {selectedRoom.Roomname}";
            RoomTypeTextBlock.Text = $"Loại phòng: {selectedRoom.RoomType}";
            CapacityTextBlock.Text = $"Diện tích: {selectedRoom.Capacity.ToString()}";
            PricePerNightTextBlock.Text = $"Giá/đêm: {selectedRoom.PricePerNight.ToString("C")}";
            StatusTextBlock.Text = $"rạng thái: {selectedRoom.Status}";
            DescriptionTextBlock.Text = $"Chi tiết: {selectedRoom.Description}";
            FloorTextBlock.Text = $"Tầng: {selectedRoom.Floor}";
            BedNumberTextBlock.Text = $"Số giường: {selectedRoom.BedNumber.ToString()}";
        }

        

        private void Back_click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManageRoom());

        }

        private void EditRoom_Click(object sender, RoutedEventArgs e)
        {
            // Điều hướng tới trang chỉnh sửa, truyền thông tin phòng hiện tại
            Room selectedRoom = (Room)this.DataContext; // Lấy thông tin phòng hiện tại
            NavigationService.Navigate(new EditRoom(selectedRoom));
        }

        private void DeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this room?",
                                          "Confirm Delete",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Room selectedRoom = (Room)this.DataContext; // Lấy thông tin phòng hiện tại

                    // Thực hiện xóa phòng trong SQL Server
                    DeleteRoomFromDatabase(selectedRoom.RoomID);

                    MessageBox.Show("Room deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Quay lại trang quản lý phòng
                    NavigationService.Navigate(new ManageRoom());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete room. Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void DeleteRoomFromDatabase(int roomId)
        {

            // Câu lệnh SQL DELETE
            string deleteQuery = "DELETE FROM Rooms WHERE RoomID = @RoomID";

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                    {
                        // Thêm tham số @RoomID
                        command.Parameters.AddWithValue("@RoomID", roomId);

                        // Thực thi lệnh DELETE
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new Exception("No room was deleted. The room may not exist.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error while deleting room: {ex.Message}");
                }
            }
        }

        private void AddAsset_Click(object sender, RoutedEventArgs e)
        {
            // lấy room id từ DataContext
            Room selectedRoom = (Room)this.DataContext;
            int roomId = selectedRoom.RoomID;

            // mở window tạo mới thiết bị
            AddDeviceWindow addDeviceWindow = new AddDeviceWindow(roomId);
            if (addDeviceWindow.ShowDialog() == true)
            {
                // load lại khi tạo thành công
                 ReloadDeviceList(roomId);
            }
        }

        // reload
        private void ReloadDeviceList(int roomId)
        {
            try
            {
                // gọi hàm lấy thiết bị
                var devices = GetDevicesForRoom(roomId);
                // clear danh sách device
                Devices.Clear();
                foreach (var device in devices) // lặp và thêm device mới
                {
                    Devices.Add(device);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update device list. Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // lấy danh sach thiết bị theo id room
        private List<Device> GetDevicesForRoom(int roomId)
        {
            // danh sách tbi
            List<Device> devices = new List<Device>();

            string query = "SELECT * FROM Devices WHERE RoomID = @RoomID";

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open(); // mở kết nối
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // thêm tham số
                        command.Parameters.AddWithValue("@RoomID", roomId);
                        // thực thi
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read()) // lặp và tạo thiết bị mới, sau đó add vào danh sách
                        {
                            Device device = new Device
                            {
                                DeviceID = reader.GetInt32(reader.GetOrdinal("DeviceID")),
                                RoomID = reader.GetInt32(reader.GetOrdinal("RoomID")),
                                DeviceName = reader.GetString(reader.GetOrdinal("DeviceName")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                InstallDate = reader.GetDateTime(reader.GetOrdinal("InstallDate")),
                                Image = reader.IsDBNull(reader.GetOrdinal("Image")) ? null : reader.GetString(reader.GetOrdinal("Image"))
                            };
                            devices.Add(device);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error fetching devices: {ex.Message}");
                }
            }
            // trả về danh sach tbi
            return devices;
        }

        // click chọn checkbox của tbi
        private void DeviceCheckBox_Checked(object sender, RoutedEventArgs e)
        {
           
            var checkBox = sender as CheckBox;
            var device = checkBox?.DataContext as Device;
            if (device != null && !SelectedDevices.Contains(device))
            {
                SelectedDevices.Add(device);
            }
        }
        // click bỏ chọn checkbox
        private void DeviceCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Get the selected device (the data bound to the checkbox)
            var checkBox = sender as CheckBox;
            var device = checkBox?.DataContext as Device;
            if (device != null)
            {
                SelectedDevices.Remove(device);
            }
        }
        // click xóa tbi đã chọn
        private void DeleteSelectedDevices_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // lặp và xóa tbi
                foreach (var device in SelectedDevices)
                {
                    DeleteDeviceFromDatabase(device.DeviceID);
                    Devices.Remove(device);
                }
                // clear danh sách đã chọn
                SelectedDevices.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete devices. Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteDeviceFromDatabase(int deviceId)
        {
            string deleteQuery = "DELETE FROM Devices WHERE DeviceID = @DeviceID";
            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@DeviceID", deviceId);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error deleting device: {ex.Message}");
                }
            }
        }


    }
}
