using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Configuration;

namespace hotel
{
    public partial class MainWindow : Window
    {
       

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Dashboard()); // vào trang dashboard đầu tiên
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Dashboard()); // click đến trang dashboard
        }

        private void RoomBookingButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Booking()); // click đến trang booking
        }

        private void RoomManagementButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManageRoom()); // click đến trang 
        }

        private void RoomBookedButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RoomBooked()); // click đến trang

        }

        private void ServicesButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManageService()); // click đến trang
        }
        private void CustomerButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ListCustomer()); // click đến trang
        }
        private void EmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManageEmployee()); // click đến trang
        }
    }
}