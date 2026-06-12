namespace WpfSmartHomeSensingApp.Models
{
    internal class SensorData
    {
        public string HomeId { get; set; }

        public string RoomName { get; set; }

        public string SensingDateTime { get; set; }

        public double Temp {  get; set; }

        public double Humid { get; set; }
    }
}
