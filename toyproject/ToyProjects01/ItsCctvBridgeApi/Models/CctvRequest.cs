using static System.Net.WebRequestMethods;

namespace WpfCctvMonitorApp.Models
{
    public class CctvRequest
    {
        // Type 클래스 키워드명 -> RoadType으로 변경
        public string RoadType { get; set; } = "ex";    // 고속도로 기본 선택
        public int CctvType { get; set; } = 1;
        // CCTV 유형(1: 실시간 스트리밍(HLS) / 2: 동영상(mp4) / 3: 정지 영상/ 4: 실시간 스트리밍(HLS)(HTTPS) / 5: 동영상(mp4)(HTTPS))
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MinY { get; set; }
        public double MaxY { get; set; }
        // GetType() 메서드로 이름 변경 GetRetType
        public string GetRetType { get; set; } = "json";    

    }
}