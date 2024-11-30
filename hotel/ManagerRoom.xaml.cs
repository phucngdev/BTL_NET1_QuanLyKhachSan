using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using hotel.config;
using hotel.models;

namespace hotel
{
    /// <summary>
    /// Interaction logic for ManageRoom.xaml
    /// </summary>
    public partial class ManageRoom : Page
    {
       

        public ManageRoom()
        {
            InitializeComponent();
            LoadRoomData(); // gọi hàm tải dữ liệu
        }
       
       

        // hàm tải dữ liệu
        private void LoadRoomData()
        {
            // tạo danh sách phòng
            List<Room> roomList = new List<Room>();

            string query = @"
            SELECT 
                r.RoomID, r.Roomname, r.RoomType, r.Capacity, r.PricePerNight, r.Status, r.Floor, r.BedNumber, r.Description, r.Image,
                d.DeviceID, d.DeviceName, d.Quantity, d.Description AS DeviceDescription, d.InstallDate, d.Image AS DeviceStatus
            FROM Rooms r
            LEFT JOIN Devices d ON r.RoomID = d.RoomID";

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    // connect db
                    connection.Open();
                    // thực hiện truy vấn
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    // lặp qua kết quả 
                    while (reader.Read())
                    {
                        // tìm 1 phòng trong danh sách 
                        Room room = roomList.FirstOrDefault(r => r.RoomID == reader.GetInt32(0));
                        if (room == null)
                        {
                            // nếu chưa có thì tạo mới và add vào list
                            room = new Room
                            {
                                RoomID = reader.GetInt32(0),
                                Roomname = reader.GetString(1),
                                RoomType = reader.GetString(2),
                                Capacity = reader.GetInt32(3),
                                PricePerNight = reader.GetDecimal(4),
                                Status = reader.GetString(5),
                                Floor = reader.GetString(6),
                                BedNumber = reader.GetInt32(7),
                                Description = reader.GetString(8),
                                Image = reader.GetString(9),
                            };
                            roomList.Add(room);
                        }
                        // kiểm tra và thêm 1 thiết bị vào room đã tạo
                        if (!reader.IsDBNull(10))
                        {
                            room.Devices.Add(new Device
                            {
                                DeviceID = reader.GetInt32(10),
                                DeviceName = reader.GetString(11),
                                Quantity = reader.GetInt32(12),
                                Description = reader.GetString(13),
                                InstallDate = reader.GetDateTime(14),
                                Image = reader.GetString(15)
                            });
                        }
                    }
                }

                RoomDataGrid.ItemsSource = roomList; // hiển thị danh sách room
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading room data: {ex.Message}");
            }
        }

        // click chọn 1 room
        private void RoomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // nếu click chọn 1 room
            if (RoomDataGrid.SelectedItem != null)
            {
                // lấy dữ liệu room đã chọn và chuyển đến trang roomDetail
                Room selectedRoom = (Room)RoomDataGrid.SelectedItem;
                NavigationService?.Navigate(new RoomDetail(selectedRoom));
            }
        }


        // click tạo mới room
        private void CreateRoom_Click(object sender, RoutedEventArgs e)
        {
            // chuyển đến trang tạo mới
            NavigationService?.Navigate(new CreateRoom()); // Replace with the Booking page

           

        }







    }
}
