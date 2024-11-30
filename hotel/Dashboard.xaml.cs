using hotel.config;
using hotel.data;
using LiveCharts;
using LiveCharts.Defaults;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace hotel
{
    public partial class Dashboard : Page
    {
        public decimal CurrentMonthRevenue { get; set; } // doanh thu tháng hiện tại
        public int RoomsRented { get; set; } // số phòng đang cho thuê, không tính đặt trước
        public int availableRooms { get; set; } // số phòng trống hiện tại
        public List<RevenueByMethod> RevenueByMethods { get; set; } // dsach doanh thu theo phương thức thanh toán
        public Dashboard()
        {
            InitializeComponent();
            LoadStatistics(); // gọi hàm lấy dữ liệu
            LoadRecentReservations();
            LoadOverdueReservations();
            this.DataContext = this; // sử dụng data context cho binding
        }

        public class RevenueByMethod
        {
            public string PaymentMethod { get; set; }
            public decimal TotalRevenue { get; set; }
        }

        public class RecentReservation
        {
            public string Roomname { get; set; }
            public DateTime CheckInDate { get; set; }
            public DateTime CheckOutDate { get; set; }
            public string FullName { get; set; }
        }

        public class RoomsByFloor
        {
            public int Floor { get; set; }
            public int TotalRooms { get; set; }
            public int AvailableRooms { get; set; }
        }

        private void LoadStatistics()
        {

            // 
            string revenueQuery = @"
            SELECT SUM(AmountPaid) AS CurrentMonthRevenue
            FROM Payments
            WHERE MONTH(PaymentDate) = MONTH(GETDATE()) 
            AND YEAR(PaymentDate) = YEAR(GETDATE())";

            string roomsRentedQuery = @"
            SELECT COUNT(*) AS RoomsRented
            FROM Reservations
            WHERE Status IN ('Thuê ngay', 'Thuê')";

            string availableRoomsQuery = @"
        SELECT COUNT(*) AS AvailableRooms
        FROM Rooms r
        WHERE r.Status = 'Available'
        AND NOT EXISTS (
            SELECT 1
            FROM Reservations res
            WHERE res.RoomID = r.RoomID
            AND (res.CheckInDate <= @CheckOutDate AND res.CheckOutDate >= @CheckInDate)
        )";
            //lấy ngày hôm nay và mai
            DateTime checkInDate = DateTime.Now;
            DateTime checkOutDate = DateTime.Now.AddDays(1);

            string revenueByMethodQuery = @"
SELECT PaymentMethod, SUM(AmountPaid) AS TotalRevenue
FROM Payments
WHERE MONTH(PaymentDate) = MONTH(GETDATE()) 
AND YEAR(PaymentDate) = YEAR(GETDATE())
GROUP BY PaymentMethod";

            RevenueByMethods = new List<RevenueByMethod>();

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                // connect
                connection.Open();

                // 
                using (SqlCommand command = new SqlCommand(revenueQuery, connection))
                {
                    // thực hiện truy vấn
                    var result = command.ExecuteScalar();
                    // gán kết quả doanh thu tháng vào biến
                    CurrentMonthRevenue = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }

                //
                using (SqlCommand command = new SqlCommand(roomsRentedQuery, connection))
                {
                // thực hiện truy vấn
                    var result = command.ExecuteScalar();
                    // gán kq số phòng đang thuê vào biến
                    RoomsRented = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }

                using (SqlCommand command = new SqlCommand(availableRoomsQuery, connection))
                {
                    // gán giá trị vào query
                    command.Parameters.AddWithValue("@CheckInDate", checkInDate);
                    command.Parameters.AddWithValue("@CheckOutDate", checkOutDate);

                    // thực hiện truy vấn
                    var result = command.ExecuteScalar();
                    // gán kết quả số phòng trống vào biến
                    availableRooms = result != DBNull.Value ? Convert.ToInt32(result) : 0;

                }

                using (SqlCommand command = new SqlCommand(revenueByMethodQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RevenueByMethod revenue = new RevenueByMethod
                            {
                                PaymentMethod = reader["PaymentMethod"].ToString(),
                                TotalRevenue = reader["TotalRevenue"] != DBNull.Value ? Convert.ToDecimal(reader["TotalRevenue"]) : 0
                            };
                            RevenueByMethods.Add(revenue);
                        }
                    }
                }
            }
        }

        public List<RecentReservation> RecentReservations { get; set; } // Danh sách đặt phòng gần đây

        private void LoadRecentReservations()
        {
            RecentReservations = new List<RecentReservation>(); // Khởi tạo danh sách

            string recentReservationsQuery = @"
    SELECT 
        r.Roomname, 
        res.CheckInDate, 
        res.CheckOutDate, 
        c.FullName
    FROM 
        Reservations res
    JOIN 
        Rooms r ON res.RoomID = r.RoomID
    JOIN 
        Customers c ON res.CustomerID = c.CustomerID
    WHERE 
        res.Status = 'Đặt phòng'
    ORDER BY 
        res.CheckInDate DESC
    OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY";  // Lấy 10 đặt phòng gần đây

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(recentReservationsQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RecentReservation reservation = new RecentReservation
                            {
                                Roomname = reader["Roomname"].ToString(),
                                CheckInDate = reader.GetDateTime(reader.GetOrdinal("CheckInDate")),
                                CheckOutDate = reader.GetDateTime(reader.GetOrdinal("CheckOutDate")),
                                FullName = reader["FullName"] != DBNull.Value ? reader["FullName"].ToString() : "N/A"
                            };
                            RecentReservations.Add(reservation);
                        }
                    }
                }
            }
        }

        public List<RecentReservation> OverdueReservations { get; set; } // Danh sách phòng quá hạn

        private void LoadOverdueReservations()
        {
            OverdueReservations = new List<RecentReservation>(); // Khởi tạo danh sách

            string overdueReservationsQuery = @"
    SELECT 
        r.Roomname, 
        res.CheckInDate, 
        res.CheckOutDate, 
        c.FullName
    FROM 
        Reservations res
    JOIN 
        Rooms r ON res.RoomID = r.RoomID
    JOIN 
        Customers c ON res.CustomerID = c.CustomerID
    WHERE 
        res.Status = 'Đặt phòng' 
        AND res.CheckInDate < GETDATE()";  // Phòng đã quá hạn check-in

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(overdueReservationsQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RecentReservation reservation = new RecentReservation
                            {
                                Roomname = reader["Roomname"].ToString(),
                                CheckInDate = reader.GetDateTime(reader.GetOrdinal("CheckInDate")),
                                CheckOutDate = reader.GetDateTime(reader.GetOrdinal("CheckOutDate")),
                                FullName = reader["FullName"] != DBNull.Value ? reader["FullName"].ToString() : "N/A"
                            };
                            OverdueReservations.Add(reservation);
                        }
                    }
                }
            }
        }


        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            UserPopup.IsOpen = !UserPopup.IsOpen; // Bật hoặc tắt Popup đăng xuất
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // clear thông tin đăng nhập
            UserSession.Instance.Clear();
            // Xử lý đăng xuất, ví dụ quay về màn hình đăng nhập
            MessageBox.Show("Đăng xuất thành công!");
            // Ví dụ chuyển trang:
            Login login = new Login();
            login.Show();
            // Đóng MainWindow
            Window.GetWindow(this)?.Close();
        }

    }
}