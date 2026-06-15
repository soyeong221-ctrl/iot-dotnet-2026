using Newtonsoft.Json;

namespace WpfBusanFoodApp.Models
{
    // FoodItem 클래스 = 맛집 정보 1개 표현.
    // API에서 item 배열 안에 들어있는 데이터 하나 = 클래스 하나로 변환.
    public class FoodItem
    {
        // 고유번호 또는 순번
        [JsonProperty("UC_SEQ")]
        public int UcSeq { get; set; }

        // 맛집 이름
        [JsonProperty("MAIN_TITLE")]
        public string MainTitle { get; set; }

        // 구/군 이름
        [JsonProperty("GUGUN_NM")]
        public string GugunNm { get; set; }

        // 주소
        [JsonProperty("ADDR1")]
        public string Addr1 { get; set; }

        // 상세 주소.
        [JsonProperty("ADDR2")]
        public string Addr2 { get; set; }

        // 전화번호
        [JsonProperty("CNTCT_TEL")]
        public string CntctTel { get; set; }

        // 대표 메뉴
        // RPRSNTV_MENU는 Representative Menu의 의미로 보면 됩니다.
        [JsonProperty("RPRSNTV_MENU")]
        public string RepresentativeMenu { get; set; }

        // 홈페이지 주소
        [JsonProperty("HOMEPAGE_URL")]
        public string HomepageUrl { get; set; }

        // 작은 이미지 주소(목록)
        [JsonProperty("MAIN_IMG_THUMB")]
        public string MainImgThumb { get; set; }

        // 큰 이미지 주소(상세화면))
        [JsonProperty("MAIN_IMG_NORMAL")]
        public string MainImgNormal { get; set; }

        // 상세 설명
        [JsonProperty("ITEMCNTNTS")]
        public string ItemCntnts { get; set; }

        // 위도(지도 표시)
        [JsonProperty("LAT")]
        public double Lat { get; set; }

        // 경도(지도 표시)
        [JsonProperty("LNG")]
        public double Lng { get; set; }

        // 화면에 표시하기 위한 순번
        public int DisplayNo { get; set; }

    }
}