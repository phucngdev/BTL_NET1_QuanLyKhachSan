using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using hotel.config;
using hotel.models;

namespace hotel
{
    public partial class ListCustomer : Page
    {
        public ListCustomer()
        {
            InitializeComponent();
            LoadCustomerData();
        }

        private void LoadCustomerData()
        {
            string query = "SELECT CustomerID, FullName, Email, Phone, Address, DateOfBirth, CCCD FROM Customers";

            List<Customer> customers = new List<Customer>();

            using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    customers.Add(new Customer
                    {
                        CustomerID = reader.GetInt32(0),
                        FullName = reader.GetString(1),
                        Email = reader.GetString(2),
                        Phone = reader.GetString(3),
                        Address = reader.GetString(4),
                        DateOfBirth = reader.GetDateTime(5),
                        CCCD = reader.GetString(6)
                    });
                }
            }

            CustomerDataGrid.ItemsSource = customers;
        }

        private void CustomerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomerDataGrid.SelectedItem is Customer selectedCustomer)
            {
                NavigationService.Navigate(new CustomerDetail(selectedCustomer));
            }
        }

        private void CreateCustomer_Click(object sender, RoutedEventArgs e)
        {
           
                NavigationService.Navigate(new CreateCustomer());
            
        }

        
    }

   
}
