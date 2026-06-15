namespace WpfSmartHomeSubscribeApp.Models
{
    internal class SensorData
    {
        public string HomeId { get; set; }
        public string RoomName { get; set; }
        public string SensingDateTime { get; set; }   // Publish 때는 String이 효과적, DB 저장시에는 DateTime 사용
        public double Temp {  get; set; }
        public double Humid { get; set; }
    }
}
