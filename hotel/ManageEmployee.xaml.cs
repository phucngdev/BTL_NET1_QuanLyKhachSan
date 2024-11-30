using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for ManageEmployee.xaml
    /// </summary>
    public partial class ManageEmployee : Page
    {
        public ManageEmployee()
        {
            InitializeComponent();
            // lấy thông tin của người đang đăng nhập được lưu trong UserSession
            var loggedInEmployee = UserSession.Instance.LoggedInEmployee;
            if (loggedInEmployee.Position != "Quản lý")
            {
                MessageBox.Show("Bạn không có quyền truy cập vào trang này.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                NavigationService.Navigate(new Dashboard()); // đến trang dashboard

                return;
            }

            // Nếu là quản lý, tải dữ liệu nhân viên
            LoadEmployeeData();
        }
        // hàm lấy danh sách nhân viên
        private void LoadEmployeeData()
        {
            // câu truy vấn
            string query = "SELECT EmployeeID, FullName, Email, Phone, Address, Position, Salary FROM Employees";
            // tạo danh sách nhân viên
            List<Employee> employees = new List<Employee>();

            using (SqlConnection conn = new SqlConnection(DatabaseConfig.ConnectionString))
            {
                conn.Open(); // mở kết nối
                // thực hiện truy vấn
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                // đọc truy vấn
                while (reader.Read())
                {
                    // tạo các đối tượng và add vào danh sách
                    employees.Add(new Employee
                    {
                        EmployeeID = reader.GetInt32(0),
                        FullName = reader.GetString(1),
                        Email = reader.GetString(2),
                        Phone = reader.GetString(3),
                        Address = reader.GetString(4),
                        Position = reader.GetString(5),
                        Salary = reader.GetDecimal(6)
                    });
                }
            }
            // hiển thị ra màn hình
            EmployeeDataGrid.ItemsSource = employees;
        }
        // hàm chuyển đến trang tạo mới
        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CreateEmployee());

        }
        // hàm chuyển hướng đến trang edit 
        private void EmployeeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lấy thông tin nhân viên được chọn
            var selectedEmployee = EmployeeDataGrid.SelectedItem as Employee;
            if (selectedEmployee == null)
            {
                return; // Nếu không có nhân viên nào được chọn, thoát khỏi hàm
            }

            // Tạo instance của trang EditEmployee và truyền thông tin nhân viên được chọn
            EditEmployee editEmployeePage = new EditEmployee(selectedEmployee.EmployeeID);

            // Chuyển đến trang EditEmployee
            NavigationService.Navigate(editEmployeePage);

            // Reset lại lựa chọn để tránh gọi sự kiện nhiều lần
            EmployeeDataGrid.SelectedItem = null;
        }

    }
}
