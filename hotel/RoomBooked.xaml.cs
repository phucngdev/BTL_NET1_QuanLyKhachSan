using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Collections.ObjectModel;
using hotel.models;

namespace hotel
{
    /// <summary>
    /// Interaction logic for RoomBooked.xaml
    /// </summary>
    public partial class RoomBooked : Page
    {
        public class ReservationViewModel // class chứa thông tin 1 room đã đặt hoặc thuê
        {
            public int RoomID { get; set; }
            public int ReservationID { get; set; }
            public int CustomerID { get; set; }
            public string Roomname { get; set; }
            public string RoomType { get; set; }
            public string Floor { get; set; }
            public DateTime CheckInDate { get; set; }
            public DateTime CheckOutDate { get; set; }
            public decimal TotalPrice { get; set; }
            public string Status { get; set; }
            public string CustomerName { get; set; }
            public string Phone { get; set; }
        }
        ObservableCollection<ReservationViewModel> reservationList = new ObservableCollection<ReservationViewModel>(); // danh sách room


        public RoomBooked()
        {
            InitializeComponent();
            reservationList.Clear(); // clear danh sách room
        }


        // tải dữ liệu theo status được lấy ở combobox
        private async Task LoadReservationsAsync(string status, DateTime? checkInDate = null, DateTime? checkOutDate = null)
        {
            try
            {
                reservationList.Clear(); // Làm trống ObservableCollection trước

                // Kết nối tới SQL Server
                using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    await conn.OpenAsync(); // Mở kết nối bất đồng bộ

                    // Truy vấn kết hợp dữ liệu giữa Room và Reservation
                    string query = @"
            SELECT r.RoomID, r.Roomname, r.RoomType, r.Floor, 
                   res.CheckInDate, res.CheckOutDate, res.TotalPrice, res.ReservationID, 
                   res.Status, c.CustomerID, c.FullName AS CustomerName, c.Phone 
            FROM Reservations res
            INNER JOIN Rooms r ON res.RoomID = r.RoomID
            INNER JOIN Customers c ON res.CustomerID = c.CustomerID
            WHERE res.Status = @Status";

                    // Thêm điều kiện cho ngày check-in và check-out nếu có
                    if (checkInDate.HasValue)
                    {
                        query += " AND res.CheckInDate >= @CheckInDate";
                    }
                    if (checkOutDate.HasValue)
                    {
                        query += " AND res.CheckOutDate <= @CheckOutDate";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Status", status);
                        if (checkInDate.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@CheckInDate", checkInDate.Value);
                        }
                        if (checkOutDate.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@CheckOutDate", checkOutDate.Value);
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            await Task.Run(() => adapter.Fill(dt)); // Thực hiện truy vấn bất đồng bộ

                            // Chuyển đổi dữ liệu từ DataTable thành ObservableCollection
                            foreach (DataRow row in dt.Rows)
                            {
                                reservationList.Add(new ReservationViewModel
                                {
                                    RoomID = Convert.ToInt32(row["RoomID"]),
                                    Roomname = row["Roomname"].ToString(),
                                    RoomType = row["RoomType"].ToString(),
                                    Floor = row["Floor"].ToString(),
                                    CheckInDate = Convert.ToDateTime(row["CheckInDate"]),
                                    CheckOutDate = Convert.ToDateTime(row["CheckOutDate"]),
                                    TotalPrice = Convert.ToDecimal(row["TotalPrice"]),
                                    Status = row["Status"].ToString(),
                                    CustomerName = row["CustomerName"].ToString(),
                                    Phone = row["Phone"].ToString(),
                                    ReservationID = Convert.ToInt32(row["ReservationID"])
                                });
                            }
                        }
                    }
                }

                roomListRentNow.ItemsSource = reservationList; // Gán ObservableCollection mới vào ItemsSource
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void CheckInDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            await ReloadReservations();
        }

        private async void CheckOutDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            await ReloadReservations();
        }

        // Hàm để tải lại dữ liệu dựa trên trạng thái và ngày đã chọn
        private async Task ReloadReservations()
        {
            ComboBoxItem selectedItem = (ComboBoxItem)StatusComboBox.SelectedItem;
            string selectedStatus = selectedItem?.Content.ToString();

            DateTime? checkInDate = CheckInDate.SelectedDate;
            DateTime? checkOutDate = CheckOutDate.SelectedDate;

            if (selectedStatus != null)
            {
                await LoadReservationsAsync(selectedStatus, checkInDate, checkOutDate);
            }
        }

        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Hủy liên kết sự kiện tạm thời
            CheckInDate.SelectedDateChanged -= CheckInDate_SelectedDateChanged;
            CheckOutDate.SelectedDateChanged -= CheckOutDate_SelectedDateChanged;

            // Clear ngày trong DatePicker
            CheckInDate.SelectedDate = null;
            CheckOutDate.SelectedDate = null;

            // Gắn lại sự kiện
            CheckInDate.SelectedDateChanged += CheckInDate_SelectedDateChanged;
            CheckOutDate.SelectedDateChanged += CheckOutDate_SelectedDateChanged;

            // Gọi LoadReservations một lần duy nhất
            ComboBoxItem selectedItem = (ComboBoxItem)StatusComboBox.SelectedItem;
            string selectedStatus = selectedItem?.Content.ToString();

            if (selectedStatus != null)
            {
                await LoadReservationsAsync(selectedStatus);
            }
        }




        // Xử lý sự kiện thay đổi lựa chọn của ComboBox
        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                // lấy lựa chọn của combobox và clear list 
                ComboBoxItem selectedItem = (ComboBoxItem)e.AddedItems[0];
                string selectedStatus = selectedItem.Content.ToString();
                reservationList.Clear();
                await LoadReservationsAsync(selectedStatus); // Gọi bất đồng bộ
            }
        }


        private void ComboBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.SelectedIndex = -1; // Reset SelectedIndex để kích hoạt lại SelectionChanged
        }
        // click chọn 1 room để xem chi tiết
        private void roomListRentNow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (roomListRentNow.SelectedItem != null)
            {
                var selectedRoom = roomListRentNow.SelectedItem as ReservationViewModel;

                if (selectedRoom != null)
                {
                    // Điều hướng đến RoomRentedDetail và truyền dữ liệu
                    NavigationService?.Navigate(new RoomRentedDetail(selectedRoom));
                }
            }
        }


    }
}
