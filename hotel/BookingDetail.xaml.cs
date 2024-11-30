using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.DirectoryServices;
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
using hotel.data;
using hotel.models;

namespace hotel
{
    /// <summary>
    /// Interaction logic for BookingDetail.xaml
    /// </summary>
    public partial class BookingDetail : Page
    {
        private Room _room;

        // Constructor nhận đối tượng phòng từ trang trước
        public BookingDetail(Room room)
        {
            InitializeComponent();
            _room = room;
            LoadRoomDetails();
        }

        // Hàm tải thông tin chi tiết phòng
        private void LoadRoomDetails()
        {
            // hiển thị thông tin phòng vào các textbox
            RoomNameTextBlock.Text = $"Tên phòng: {_room.Roomname}";
            RoomTypeTextBlock.Text = $"Loại phòng: {_room.RoomType}";
            CapacityTextBlock.Text = $"Diện tích: {_room.Capacity.ToString()}";
            PricePerNightTextBlock.Text = $"Giá phòng: {_room.PricePerNight.ToString("C")}";
            StatusTextBlock.Text = $"Trạng thái: {_room.Status}";
            FloorTextBlock.Text = $"Tầng: {_room.Floor}";
            BedNumberTextBlock.Text = $"Số giường: {_room.BedNumber.ToString()}";
        }


        // click search khách hàng
        private void SearchCustomer_Click(object sender, RoutedEventArgs e)
        {
            // lấy giá trị nhập vào (.Trim() để bỏ khoảng trắng 2 đầu)
            string cccdOrPhone = CCCDPhoneTextBox.Text.Trim();
            // kiểm tra đã nhập chưa
            if (string.IsNullOrEmpty(cccdOrPhone))
            {
                // nếu chưa thì hiện cảnh báo
                MessageBox.Show("Please enter CCCD or Phone number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // câu lệnh truy vấn
            string query = "SELECT * FROM Customers WHERE CCCD LIKE @CCCDOrPhone OR Phone LIKE @CCCDOrPhone";
            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    // connect database
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    // truyền key muốn tìm kiếm vào query
                    command.Parameters.AddWithValue("@CCCDOrPhone", $"%{cccdOrPhone}%");
                    // tạo danh sách khách hàng
                    List<Customer> customers = new List<Customer>();
                    // thực hiện truy vấn
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // lặp qua kết quả truy vấn để tạo Customer và add vào danh sách
                        while (reader.Read())
                        {
                            customers.Add(new Customer
                            {
                                CustomerID = (int)reader["CustomerID"],
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Address = reader["Address"].ToString(),
                                DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                                CCCD = reader["CCCD"].ToString()
                            });
                        }
                    }

                    // nếu  danh sách có dữ liệu
                    if (customers.Any())
                    {
                        // Bind dữ liệu vào ListBox
                        CustomerListBox.ItemsSource = customers;

                        // Hiển thị phần kết quả (lúc đầu bị ẩn, có danh sách thì hiện lên)
                        SearchResultsPanel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        // Ẩn phần kết quả nếu không có kết quả
                        CustomerListBox.ItemsSource = null;
                        SearchResultsPanel.Visibility = Visibility.Collapsed;
                        MessageBox.Show("No results found!", "Search", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // click chọn khách hàng 
        private void CustomerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // nếu chọn 1 khách hàng từ list
            if (CustomerListBox.SelectedItem is Customer selectedCustomer)
            {
                // gán giá trị các textbox bằng thông tin khách hàng
                FullNameTextBox.Text = selectedCustomer.FullName;
                EmailTextBox.Text = selectedCustomer.Email;
                PhoneTextBox.Text = selectedCustomer.Phone;
                AddressTextBox.Text = selectedCustomer.Address;
                DateOfBirthTextBox.Text = selectedCustomer.DateOfBirth.ToShortDateString();
                CCCDTextBox.Text = selectedCustomer.CCCD;
            }
        }


        // Lưu thông tin đặt phòng
        private void SaveBooking_Click(object sender, RoutedEventArgs e)
        {
            // Lấy thông tin khách hàng
            string fullName = FullNameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string phone = PhoneTextBox.Text.Trim();
            string address = AddressTextBox.Text.Trim();
            string dob = DateOfBirthTextBox.Text.Trim();
            string cccd = CCCDTextBox.Text.Trim();

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(cccd) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Please fill in all required customer details.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // tạo biến lưu id khách hàng
            int CustomerID;
            // thực hiện truy vấn với using
            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    // connect db
                    connection.Open();

                    // Kiểm tra khách hàng đã tồn tại
                    string checkQuery = "SELECT CustomerID FROM Customers WHERE CCCD = @CCCD OR Phone = @Phone";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@CCCD", cccd);
                    checkCommand.Parameters.AddWithValue("@Phone", phone);

                    object result = checkCommand.ExecuteScalar();
                    // nếu khách hàng đã tồn tại, nghĩa là khác null
                    if (result != null)
                    {
                        CustomerID = (int)result; // Lấy CustomerID
                    }
                    // nếu chưa tồn tại khách hàng trong database
                    else
                    {
                        // Tạo mới khách hàng
                        string insertCustomerQuery = "INSERT INTO Customers (FullName, Email, Phone, Address, DateOfBirth, CCCD) " +
                                                     "OUTPUT INSERTED.CustomerID VALUES (@FullName, @Email, @Phone, @Address, @DateOfBirth, @CCCD)";
                        SqlCommand insertCustomerCommand = new SqlCommand(insertCustomerQuery, connection);
                        insertCustomerCommand.Parameters.AddWithValue("@FullName", fullName);
                        insertCustomerCommand.Parameters.AddWithValue("@Email", email);
                        insertCustomerCommand.Parameters.AddWithValue("@Phone", phone);
                        insertCustomerCommand.Parameters.AddWithValue("@Address", address);
                        insertCustomerCommand.Parameters.AddWithValue("@DateOfBirth", DateTime.Parse(dob));
                        insertCustomerCommand.Parameters.AddWithValue("@CCCD", cccd);

                        CustomerID = (int)insertCustomerCommand.ExecuteScalar();
                    }

                    // Lưu thông tin đặt phòng
                    string insertReservationQuery = "INSERT INTO Reservations (CustomerID, RoomID, EmployeeID, CheckInDate, CheckOutDate, TotalPrice, Status) " +
                                                    "VALUES (@CustomerID, @RoomID, @EmployeeID, @CheckInDate, @CheckOutDate, @TotalPrice, @Status)";
                    // lấy thông tin của người đang đăng nhập được lưu trong UserSession
                    var loggedInEmployee = UserSession.Instance.LoggedInEmployee;
                    SqlCommand insertReservationCommand = new SqlCommand(insertReservationQuery, connection);
                    insertReservationCommand.Parameters.AddWithValue("@CustomerID", CustomerID);
                    insertReservationCommand.Parameters.AddWithValue("@RoomID", _room.RoomID);
                    insertReservationCommand.Parameters.AddWithValue("@EmployeeID", loggedInEmployee.EmployeeID);
                    insertReservationCommand.Parameters.AddWithValue("@CheckInDate", CheckInDatePicker.SelectedDate);
                    insertReservationCommand.Parameters.AddWithValue("@CheckOutDate", CheckOutDatePicker.SelectedDate);
                    insertReservationCommand.Parameters.AddWithValue("@TotalPrice", Convert.ToDecimal(TotalPriceTextBox.Text));
                    insertReservationCommand.Parameters.AddWithValue("@Status", ((ComboBoxItem)ReservationStatusComboBox.SelectedItem).Content.ToString());
                    // thực hiện insert
                    insertReservationCommand.ExecuteNonQuery();

                    // quay về trang danh sách phòng đã đặt
                    NavigationService.Navigate(new RoomBooked());


                    MessageBox.Show("Reservation saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving reservation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        // Hủy bỏ thông tin
        private void CancelBooking_Click(object sender, RoutedEventArgs e)
        {
            // Quay lại trang danh sách
            NavigationService?.GoBack();
        }



        // Cập nhật giá tiền khi chọn ngày
        private void CheckInDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // gọi đến hàm tính tiền khi thay đổi ngày
            CalculateTotalPrice();
        }

        private void CheckOutDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // gọi đến hàm tính tiền khi thay đổi ngày
            CalculateTotalPrice();
        }

        // hàm tính tiền phòng
        private void CalculateTotalPrice()
        {
            // nếu đã chọn ngày check in, out
            if (CheckInDatePicker.SelectedDate.HasValue && CheckOutDatePicker.SelectedDate.HasValue && ReservationStatusComboBox.SelectedItem != null)
            {
                // lấy ngày check in, out
                DateTime checkIn = CheckInDatePicker.SelectedDate.Value;
                DateTime checkOut = CheckOutDatePicker.SelectedDate.Value;
                // tính số ngày ở
                int numberOfDays = (checkOut - checkIn).Days;

                if (numberOfDays > 0)
                {

                    decimal roomPrice = _room.PricePerNight; // lấy giá phòng
                    // hiển thị tổng số tiền vào textbox
                    TotalPriceTextBox.Text = (roomPrice * numberOfDays).ToString(); // số tiền bằng số ngày nhân giá phòng
                }
            }
        }



    }
}
