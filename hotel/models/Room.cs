namespace hotel.models
{
    public class Room
    {
        public int RoomID { get; set; }
        public string RoomType { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerNight { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Floor { get; set; }
        public string Roomname { get; set; }
        public int BedNumber { get; set; }
        public List<Device> Devices { get; set; } = new List<Device>();
    }

    
}
