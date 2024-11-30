using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hotel.models
{
    public class Device
    {
        public int DeviceID { get; set; }
        public int RoomID { get; set; }
        public string DeviceName { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public DateTime InstallDate { get; set; }
        public string Image { get; set; }


    }
}
