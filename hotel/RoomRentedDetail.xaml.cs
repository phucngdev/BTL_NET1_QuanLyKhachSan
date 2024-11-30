using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using hotel.config;
using hotel.models;
using static hotel.RoomBooked;

namespace hotel
{
    public partial class RoomRentedDetail : Page
    {
        private ReservationViewModel selectedRoom;
        public class ServiceUsed
        {
            public int ServiceID { get; set; }
            public string ServiceName { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice { get; set; }
        }

        List<ServiceUsed> usedServices = new List<ServiceUsed>();
        private List<Service> availableServices = new List<Service>();

        public RoomRentedDetail(ReservationViewModel reservation)
        {
            InitializeComponent();
            usedServices.Clear();
            selectedRoom = reservation;
            if (reservation.Status == "Thuê ngay")
            {
                BookingButtonsPanel.Visibility = Visibility.Collapsed;
                ListServiceUsed.Visibility = Visibility.Visible;
                RentNowSection.Visibility = Visibility.Visible;

            }
            else if (reservation.Status == "Đặt phòng")
            {
                BookingButtonsPanel.Visibility = Visibility.Visible;
                ListServiceUsed.Visibility = Visibility.Collapsed;
                RentNowSection.Visibility = Visibility.Collapsed;

            }
            else if(reservation.Status == "Đã thanh toán")
            {
                BookingButtonsPanel.Visibility = Visibility.Collapsed;
                ListServiceUsed.Visibility = Visibility.Visible;
                RentNowSection.Visibility = Visibility.Collapsed;
                LoadPaymentDetails();
            }
            LoadRoomDetails();
            //LoadAvailableServicesUsed();
            LoadAvailableServices();
            UpdatePaymentDetails();

        }



       

        private void CheckInButton_Click(object sender, RoutedEventArgs e)
        {
            string deleteQuery = @"UPDATE Reservations SET Status = @Status WHERE ReservationID = @ReservationID";

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                    {
                        // Thêm tham số
                        command.Parameters.AddWithValue("@Status", "Thuê ngay");
                        command.Parameters.AddWithValue("@ReservationID", selectedRoom.ReservationID);

                        // Thực thi lệnh DELETE
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new Exception("No room was deleted. The room may not exist.");
                        }
                        NavigationService?.Navigate(new RoomBooked());

                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error while deleting room: {ex.Message}");
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            string deleteQuery = @"DELETE FROM Reservations WHERE ReservationID = @ReservationID";

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                    {
                        // Thêm tham số @ReservationID
                        command.Parameters.AddWithValue("@ReservationID", selectedRoom.ReservationID);

                        // Thực thi lệnh DELETE
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new Exception("No room was deleted. The room may not exist.");
                        }
                        NavigationService?.Navigate(new RoomBooked());

                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error while deleting room: {ex.Message}");
                }
            }
        }


        private async void LoadRoomDetails()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    await conn.OpenAsync();

                    string roomQuery = @"
                        SELECT r.Roomname, r.RoomType, r.Floor, r.Capacity, r.BedNumber, r.PricePerNight
                        FROM Rooms r
                        WHERE r.RoomID = @RoomID";

                    using (SqlCommand cmd = new SqlCommand(roomQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@RoomID", selectedRoom.RoomID);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                RoomNameText.Text = "Tên phòng: " + reader["Roomname"].ToString();
                                RoomTypeText.Text = "Loại phòng: " + reader["RoomType"].ToString();
                                FloorText.Text = "Tầng: " + reader["Floor"].ToString();
                                CapacityText.Text = "Diện tích: " + reader["Capacity"].ToString();
                                BedNumberText.Text = "Số giường: " + reader["BedNumber"].ToString();
                                PricePerNightText.Text = "Giá/đêm: " + reader["PricePerNight"].ToString();
                            }
                        }
                    }

                    string customerQuery = @"
                        SELECT c.FullName AS CustomerName, c.Phone, res.CheckInDate, res.CheckOutDate, res.TotalPrice
                        FROM Reservations res
                        INNER JOIN Customers c ON res.CustomerID = c.CustomerID
                        WHERE res.ReservationID = @ReservationID";

                    using (SqlCommand cmd = new SqlCommand(customerQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ReservationID", selectedRoom.ReservationID);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                CustomerNameText.Text = "Khách hàng: " + reader["CustomerName"].ToString();
                                PhoneText.Text = "Số điện thoại: " + reader["Phone"].ToString();
                                CheckInDateText.Text = "Check-In: " + Convert.ToDateTime(reader["CheckInDate"]).ToString("MM/dd/yyyy");
                                CheckOutDateText.Text = "Check-Out: " + Convert.ToDateTime(reader["CheckOutDate"]).ToString("MM/dd/yyyy");
                                TotalPriceText.Text = "Giá phòng: " + Convert.ToDecimal(reader["TotalPrice"]).ToString("C");
                            }
                        }
                    }

                   
                }

               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadAvailableServicesUsed()
        {
            try
            {
                usedServices.Clear();
                using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    await conn.OpenAsync();
                    string servicesQuery = @"
                SELECT s.ServiceID, s.ServiceName, rs.Quantity, rs.TotalPrice, s.Price
                FROM ReservationServices rs
                INNER JOIN Services s ON rs.ServiceID = s.ServiceID
                WHERE rs.ReservationID = @ReservationID";

                    using (SqlCommand cmd = new SqlCommand(servicesQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ReservationID", selectedRoom.ReservationID);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                usedServices.Add(new ServiceUsed
                                {
                                    ServiceID = Convert.ToInt32(reader["ServiceID"]),
                                    ServiceName = reader["ServiceName"].ToString(),
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    TotalPrice = Convert.ToDecimal(reader["TotalPrice"])
                                });
                            }
                        }
                    }
                }
                serviceListUsed.ItemsSource = null;
                serviceListUsed.ItemsSource = usedServices;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void LoadAvailableServices()
        {
            try
            {
                string query = @"SELECT * FROM Services";
                using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            availableServices.Clear();
                            while (reader.Read())
                            {
                                availableServices.Add(new Service
                                {
                                    ServiceID = Convert.ToInt32(reader["ServiceID"]),
                                    ServiceName = reader["ServiceName"].ToString(),
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    Description = reader["Description"].ToString()
                                });
                            }
                        }
                    }
                }
                if (availableServices.Count == 0)
                {
                    MessageBox.Show("No services available.");
                }
                ServiceComboBox.ItemsSource = availableServices;
                ServiceComboBox.DisplayMemberPath = "ServiceName";
                ServiceComboBox.SelectedValuePath = "ServiceID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



         private void QuantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ServiceComboBox.SelectedItem != null && int.TryParse(QuantityTextBox.Text, out int quantity))
            {
                var selectedService = (Service)ServiceComboBox.SelectedItem;
                decimal totalPrice = selectedService.Price * quantity;
                CalculatedTotalPriceText.Text = totalPrice.ToString();
            }
        }

        private void ServiceComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAvailableServices();
        }


        private void SaveService_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedRoom == null || selectedRoom.ReservationID <= 0)
                {
                    MessageBox.Show("Không có ReservationID hợp lệ!");
                    return;
                }

                if (ServiceComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn dịch vụ!");
                    return;
                }

                int serviceID = (int)ServiceComboBox.SelectedValue;
                int quantity = int.Parse(QuantityTextBox.Text);
                decimal totalPrice = decimal.Parse(CalculatedTotalPriceText.Text); // Assuming total price is already calculated

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    connection.Open();

                    string checkQuery = "SELECT Quantity, TotalPrice FROM ReservationServices WHERE ReservationID = @ReservationID AND ServiceID = @ServiceID";

                    // Khai báo biến để lưu giá trị trước
                    int existingQuantity = 0;
                    decimal existingTotalPrice = 0;
                    bool recordExists = false;

                    // Sử dụng SqlCommand riêng để kiểm tra
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@ReservationID", selectedRoom.ReservationID);
                        checkCommand.Parameters.AddWithValue("@ServiceID", serviceID);

                        // Sử dụng ExecuteScalar thay vì DataReader
                        using (SqlDataReader reader = checkCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                recordExists = true;
                                existingQuantity = reader.GetInt32(0);
                                existingTotalPrice = reader.GetDecimal(1);
                            }
                        }
                    }

                    // Xử lý logic update hoặc insert
                    if (recordExists)
                    {
                        // Update logic
                        int newQuantity = existingQuantity + quantity;
                        decimal newTotalPrice = existingTotalPrice + totalPrice;

                        string updateQuery = "UPDATE ReservationServices SET Quantity = @Quantity, TotalPrice = @TotalPrice WHERE ReservationID = @ReservationID AND ServiceID = @ServiceID";
                        using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@ReservationID", selectedRoom.ReservationID);
                            updateCommand.Parameters.AddWithValue("@ServiceID", serviceID);
                            updateCommand.Parameters.AddWithValue("@Quantity", newQuantity);
                            updateCommand.Parameters.AddWithValue("@TotalPrice", newTotalPrice);

                            updateCommand.ExecuteNonQuery();
                        }

                        LoadAvailableServicesUsed();
                        MessageBox.Show("Service updated successfully!");
                    }
                    else
                    {
                        // Insert logic
                        string insertQuery = "INSERT INTO ReservationServices (ReservationID, ServiceID, Quantity, TotalPrice) VALUES (@ReservationID, @ServiceID, @Quantity, @TotalPrice)";
                        using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@ReservationID", selectedRoom.ReservationID);
                            insertCommand.Parameters.AddWithValue("@ServiceID", serviceID);
                            insertCommand.Parameters.AddWithValue("@Quantity", quantity);
                            insertCommand.Parameters.AddWithValue("@TotalPrice", totalPrice);

                            insertCommand.ExecuteNonQuery();
                        }

                        LoadAvailableServicesUsed();
                        MessageBox.Show("Service saved successfully!");
                    }

                    ServiceComboBox.SelectedIndex = -1;
                    QuantityTextBox.Text = ""; // Đặt về giá trị mặc định
                    CalculatedTotalPriceText.Text = ""; // Đặt về giá trị mặc định
                    UpdatePaymentDetails();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving service: {ex.Message}");
            }
        }



        private void CancelService_Click(object sender, RoutedEventArgs e)
        {
            ServiceComboBox.SelectedIndex = -1;
            QuantityTextBox.Clear();
            CalculatedTotalPriceText.Text = "";
        }

       
        private async void UpdatePaymentDetails()
        {
            try
            {
                await LoadAvailableServicesUsed();
                // Tính tổng tiền dịch vụ
                DateTime checkInDate = Convert.ToDateTime(selectedRoom.CheckInDate);
                DateTime checkOutDate = Convert.ToDateTime(selectedRoom.CheckOutDate);
                int numberOfNights = (checkOutDate - checkInDate).Days;

                decimal roomTotal = selectedRoom.TotalPrice;
                RoomTotalText.Text = roomTotal.ToString("C");

                decimal serviceTotal = usedServices.Sum(service => service.TotalPrice);
                ServicesTotalText.Text = serviceTotal.ToString("C");

                decimal grandTotal = roomTotal + serviceTotal;
                GrandTotalText.Text = grandTotal.ToString("C");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error calculating total: {ex.Message}");
            }
        }

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra phương thức thanh toán
            if (PaymentMethodComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a payment method.", "Payment Method Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {

                decimal roomTotal = selectedRoom.TotalPrice;
                decimal serviceTotal = usedServices.Sum(service => service.TotalPrice);
                decimal grandTotal = roomTotal + serviceTotal;
                string paymentMethod = ((ComboBoxItem)PaymentMethodComboBox.SelectedItem).Content.ToString();

                // Thực hiện thanh toán và lưu vào cơ sở dữ liệu
                int paymentId = ProcessPayment(paymentMethod, grandTotal);

                MessageBox.Show($"Payment processed successfully!\nPayment ID: {paymentId}\nTotal Amount: {grandTotal:C}\nPayment Method: {paymentMethod}",
                    "Payment Confirmation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                NavigationService?.Navigate(new RoomBooked());

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing payment: {ex.Message}");
            }
        }

        private int ProcessPayment(string paymentMethod, decimal amountPaid)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();
                // update trạng thái đã thanh toán
                string updateQuery = @"UPDATE Reservations SET Status = @Status WHERE ReservationID = @ReservationID";
                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@Status", "Đã thanh toán");
                    updateCommand.Parameters.AddWithValue("@ReservationID", selectedRoom.ReservationID);
                    updateCommand.ExecuteNonQuery();
                }

                // Chuẩn bị câu lệnh SQL để chèn thanh toán
                string query = @"
            INSERT INTO Payments 
            (ReservationID, PaymentDate, PaymentMethod, AmountPaid)
            VALUES 
            (@ReservationID, @PaymentDate, @PaymentMethod, @AmountPaid);
            SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Thêm các tham số
                    command.Parameters.AddWithValue("@ReservationID", selectedRoom.ReservationID);
                    command.Parameters.AddWithValue("@PaymentDate", DateTime.Now);
                    command.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                    command.Parameters.AddWithValue("@AmountPaid", amountPaid);

                    // Thực thi và trả về ID của khoản thanh toán mới
                    return Convert.ToInt32(command.ExecuteScalar());
                }
               


            }
        }

        private void LoadPaymentDetails()
        {
            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    // Truy vấn thông tin thanh toán
                    string query = @"SELECT AmountPaid, PaymentDate, PaymentMethod 
                             FROM Payments 
                             WHERE ReservationID = @ReservationID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ReservationID", selectedRoom.ReservationID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Gán giá trị vào các TextBlock
                                decimal amountPaid = reader.GetDecimal(0);
                                DateTime paymentDate = reader.GetDateTime(1);
                                string paymentMethod = reader.GetString(2);

                                AmountPaidText.Text = $"Số tiền đã thanh toán: {amountPaid:C}";
                                PaymentDateText.Text = $"Ngày thanh toán: {paymentDate:dd/MM/yyyy}";
                                PaymentMethodText.Text = $"Hình thức thanh toán: {paymentMethod}";
                            }
                            else
                            {
                                // Không có thông tin thanh toán
                                AmountPaidText.Text = "Số tiền đã thanh toán: Không có dữ liệu";
                                PaymentDateText.Text = "Ngày thanh toán: Không có dữ liệu";
                                PaymentMethodText.Text = "Hình thức thanh toán: Không có dữ liệu";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi khi tải thông tin thanh toán: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }





    }
}
