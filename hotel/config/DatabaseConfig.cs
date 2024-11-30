using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hotel.config
{
    public static class DatabaseConfig
    {
        // Chuỗi kết nối được khai báo là static để truy cập dễ dàng mà không cần tạo đối tượng
        // thay tên server = tên server trong máy
        public static readonly string ConnectionString = "Server=DESKTOP-U7LD9P5;Database=QLKhachSan;Trusted_Connection=True;";
    }
}
