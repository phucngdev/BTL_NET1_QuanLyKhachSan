using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using hotel.config;
using hotel.models;

namespace hotel
{
    public partial class ManageService : Page
    {
        // ObservableCollection tự thay đổi giao diện khi có sự thay đổi
        public ObservableCollection<Service> serviceList; // lưu danh sách service

        public ManageService()
        {
            InitializeComponent();
            serviceList = new ObservableCollection<Service>(); // tạo mới
            LoadRoomData(); // tải dữ liệu
        }


        // tải dữ liệu
        private void LoadRoomData()
        {
            // Clear danh sach trc khi tải
            serviceList.Clear();

            string query = @"SELECT * FROM Services";

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    connection.Open(); // connect db
                    // thực hiện truy vấn
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    // lặp qua kq
                    while (reader.Read())
                    {
                        // tạo và add các service vào list
                        Service sv = new Service
                        {
                            ServiceID = reader.GetInt32(0),
                            ServiceName = reader.GetString(1),
                            Price = reader.GetDecimal(2),
                            Description = reader.GetString(3),
                        };

                        serviceList.Add(sv);  
                    }
                }

                ServiceDataGrid.ItemsSource = serviceList; // hiển thị ra giao diện
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading service data: {ex.Message}");
            }
        }

        // click tao mới service
        private void CreateService_Click(object sender, RoutedEventArgs e)
        {
            // tạo mới window create
            CreateServiceWindow createWindow = new CreateServiceWindow();

            // load lại data nếu tạo thành công
            if (createWindow.ShowDialog() == true)
            {
                LoadRoomData();
            }
        }

        // click chọn 1 service
        private void EditService_Click(object sender, RoutedEventArgs e)
        {
            // lấy data của service đó
            Service selectedService = (Service)ServiceDataGrid.SelectedItem;

            if (selectedService != null)
            {
                // tạo và chuyển đến trang edit
                EditService editPage = new EditService(selectedService);
                editPage.ServiceUpdated += LoadRoomData;
                this.NavigationService.Navigate(editPage);
            }
           
        }

    }
}
