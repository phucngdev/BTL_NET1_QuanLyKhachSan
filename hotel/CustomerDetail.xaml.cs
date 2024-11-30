using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Controls;
using hotel.config;
using hotel.models;

namespace hotel
{
    public partial class CustomerDetail : Page
    {
        private Customer _customer;

        public CustomerDetail(Customer customer)
        {
            InitializeComponent();
            _customer = customer;
            DisplayCustomerDetails();
            LoadReservationHistory();
        }

        private void DisplayCustomerDetails()
        {
            FullNameText.Text = _customer.FullName;
            EmailText.Text = _customer.Email;
            PhoneText.Text = _customer.Phone;
            AddressText.Text = _customer.Address;
            DOBText.Text = _customer.DateOfBirth.ToString("d");
            CCCDText.Text = _customer.CCCD;
        }

        private void LoadReservationHistory()
        {
            string query = "SELECT ReservationID, EmployeeID, RoomID, CheckInDate, CheckOutDate, TotalPrice, Status " +
                           "FROM Reservations WHERE CustomerID = @CustomerID";

            List<Reservation> reservations = new List<Reservation>();

            using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CustomerID", _customer.CustomerID);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    reservations.Add(new Reservation
                    {
                        ReservationID = reader.GetInt32(0),
                        EmployeeID = reader.GetInt32(1),
                        RoomID = reader.GetInt32(2),
                        CheckInDate = reader.GetDateTime(3),
                        CheckOutDate = reader.GetDateTime(4),
                        TotalPrice = reader.GetDecimal(5),
                        Status = reader.GetString(6)
                    });
                }
            }

            ReservationDataGrid.ItemsSource = reservations;
        }
    }

    
}
