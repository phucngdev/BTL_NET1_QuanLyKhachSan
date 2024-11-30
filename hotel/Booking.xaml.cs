using hotel.config;
using hotel.models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace hotel
{
    public partial class Booking : Page
    {

        public Booking()
        {
            InitializeComponent();
            LoadAvailableRooms();
        }


        private void LoadAvailableRooms()
        {
            try
            {
                // Lấy ngày hiện tại làm CheckIn và CheckOut
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);

                // Lấy danh sách phòng trống
                List<Room> rooms = GetAvailableRooms(today, tomorrow);

                RoomWrapPanel.Children.Clear(); // Xóa danh sách cũ nếu có

                foreach (var room in rooms)
                {
                    // Tạo nút động cho mỗi phòng
                    Button roomButton = new Button
                    {
                        Content = new StackPanel
                        {
                            Children =
                    {
                        // Load ảnh an toàn
                        new Image
                        {
                            Source = LoadImage(room.Image),
                            Height = 100,
                            Width = 100,
                            Margin = new Thickness(0, 0, 0, 10)
                        },
                        new TextBlock
                        {
                            Text = $"{room.RoomType} - {room.Roomname}",
                            TextWrapping = TextWrapping.Wrap,
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(0, 5, 0, 5)
                        },
                        new TextBlock
                        {
                            Text = $"{room.PricePerNight:C}",
                            Foreground = System.Windows.Media.Brushes.Green
                        }
                    }
                        },
                        Tag = room, // Lưu chi tiết phòng trong nút
                        Margin = new Thickness(10),
                        Padding = new Thickness(5)
                    };

                    // Gắn sự kiện click
                    roomButton.Click += RoomButton_Click;

                    // Thêm vào giao diện
                    RoomWrapPanel.Children.Add(roomButton);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading available rooms: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        // click chọn 1 room
        private void RoomButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Room room)
            {
                // Chuyển đến trang BookingDetail và truyền thông tin phòng
                NavigationService?.Navigate(new BookingDetail(room));
            }
        }

        // Lấy image
        private BitmapImage LoadImage(string imagePath)
        {
            try
            {
                return new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            }
            catch
            {
                return new BitmapImage(new Uri("/Assets/default-room.jpg", UriKind.Relative));
            }
        }
       
        // hàm lấy phòng trống, truyền vào ngày check in, check out, loại phòng, tầng
        private List<Room> GetAvailableRooms(DateTime checkInDate, DateTime checkOutDate, string roomType = null, string floor = null)
        {
            // tạo danh sách phòng
            List<Room> rooms = new List<Room>();

            try
            {
                // thực hiện truy vấn với using
                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    // connect với database
                    connection.Open();
                    // câu lệnh truy vấn
                    string query = @"
                SELECT r.RoomID, r.RoomType, r.Capacity, r.PricePerNight, r.Status, r.Description, r.Floor, r.Image, r.Roomname, r.BedNumber
                FROM Rooms r
                WHERE r.Status = 'Available'
                AND NOT EXISTS (
                    SELECT 1
                    FROM Reservations res
                    WHERE res.RoomID = r.RoomID
                    AND (@CheckInDate < res.CheckOutDate AND @CheckOutDate > res.CheckInDate))";
                    // NOT EXISTS được sử dụng để loại bỏ các phòng đã được đặt trong khoảng thời gian
                    // Thêm lọc loại phòng
                    if (!string.IsNullOrEmpty(roomType) && roomType != "Tất cả")
                    {
                        query += " AND r.RoomType = @RoomType";
                    }
                    // Thêm lọc tầng
                    if (!string.IsNullOrEmpty(floor) && floor != "Tất cả")
                    {
                        query += " AND r.Floor = @floor";
                    }
                    // Tạo lệnh SQL
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // truyền ngày check in, out vào câu truy vấn
                        command.Parameters.AddWithValue("@CheckInDate", checkInDate);
                        command.Parameters.AddWithValue("@CheckOutDate", checkOutDate);
                        // truyền loại phòng vào câu truy vấn nếu có
                        if (!string.IsNullOrEmpty(roomType) && roomType != "Tất cả")
                        {
                            command.Parameters.AddWithValue("@RoomType", roomType);
                        }
                        // truyền tầng vào câu truy vấn nếu có
                        if (!string.IsNullOrEmpty(floor) && floor != "Tất cả")
                        {
                            command.Parameters.AddWithValue("@Floor", floor);
                        }
                        // thực hiện truy vấn
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // lặp qua kết quả truy vấn để tạo room và add vào danh sách phòng
                            while (reader.Read())
                            {
                                rooms.Add(new Room
                                {
                                    RoomID = (int)reader["RoomID"],
                                    RoomType = reader["RoomType"].ToString(),
                                    Capacity = (int)reader["Capacity"],
                                    PricePerNight = (decimal)reader["PricePerNight"],
                                    Status = reader["Status"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    Image = reader["Image"].ToString(),
                                    Floor = reader["Floor"].ToString(),
                                    Roomname = reader["Roomname"].ToString(),
                                    BedNumber = (int)reader["BedNumber"]
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching rooms: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // trả về danh sách phòng
            return rooms;
        }



        // click tìm kiếm
        private void SearchAvailableRooms_Click(object sender, RoutedEventArgs e)
        {
            // Lấy giá trị từ DatePicker
            DateTime? checkInDate = CheckInDate.SelectedDate;
            DateTime? checkOutDate = CheckOutDate.SelectedDate;

            if (checkInDate == null || checkOutDate == null || checkInDate >= checkOutDate)
            {
                MessageBox.Show("Please select valid check-in and check-out dates.", "Invalid Dates", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Lấy giá trị từ ComboBox
            string selectedRoomType = ((ComboBoxItem)RoomTypeComboBox.SelectedItem)?.Content.ToString();
            string selectedFloor = ((ComboBoxItem)FloorComboBox.SelectedItem)?.Content.ToString();

            // Lấy danh sách phòng khả dụng
            List<Room> rooms = GetAvailableRooms(checkInDate.Value, checkOutDate.Value, selectedRoomType, selectedFloor);

            RoomWrapPanel.Children.Clear(); // Xóa danh sách cũ

            foreach (var room in rooms)
            {
                // Tạo nút động cho mỗi phòng
                Button roomButton = new Button
                {
                    Content = new StackPanel
                    {
                        Children =
                    {
                        // Load ảnh an toàn
                        new Image
                        {
                            Source = LoadImage(room.Image),
                            Height = 100,
                            Width = 100,
                            Margin = new Thickness(0, 0, 0, 10)
                        },
                        new TextBlock
                        {
                            Text = $"{room.RoomType} - {room.Roomname}",
                            TextWrapping = TextWrapping.Wrap,
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(0, 5, 0, 5)
                        },
                        new TextBlock
                        {
                            Text = $"{room.PricePerNight:C}",
                            Foreground = System.Windows.Media.Brushes.Green
                        }
                    }
                    },
                    Tag = room, // Lưu chi tiết phòng trong nút
                    Margin = new Thickness(10),
                    Padding = new Thickness(5)
                };
                // thêm sự kiện click
                roomButton.Click += RoomButton_Click;
                // Thêm vào giao diện
                RoomWrapPanel.Children.Add(roomButton);
            }
        }


       

    }
}
