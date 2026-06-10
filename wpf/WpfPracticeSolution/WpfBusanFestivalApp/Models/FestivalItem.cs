using Newtonsoft.Json;

namespace WpfBusanFestivalApp.Models
{
    public class FestivalItem
    {
        // JSON에 UC_SEQ 키값을 UcSeq로 변환해주는 작업
        // 메서드로 자동 변환
        [JsonProperty("UC_SEQ")]
        public int UcSeq { get; set; }

        [JsonProperty("MAIN_TITLE")]
        public string MainTitle { get; set; }

        [JsonProperty("GUGUN_NM")]
        public string GugunNm { get; set; }

        [JsonProperty("LAT")]
        public double Lat { get; set; }

        [JsonProperty("LNG")]
        public double Lng { get; set; }

        [JsonProperty("PLACE")]
        public string Place { get; set; }

        [JsonProperty("TITLE")]
        public string Title { get; set; }

        [JsonProperty("SUBTITLE")]
        public string SubTitle { get; set; }

        [JsonProperty("MAIN_PLACE")]
        public string MainPlace { get; set; }

        [JsonProperty("ADDR1")]
        public string Addr1 { get; set; }

        [JsonProperty("ADDR2")]
        public string Addr2 { get; set; }

        [JsonProperty("CNTCT_TEL")]
        public string CntctTel { get; set; }

        [JsonProperty("HOMEPAGE_URL")]
        public string HomepageUrl { get; set; }

        [JsonProperty("TRFC_INFO")]
        public string TrfcInfo { get; set; }

        [JsonProperty("USAGE_DAY")]
        public string UsageDay { get; set; }

        [JsonProperty("USAGE_DAY_WEEK_AND_TIME")]
        public string UsageDayWeekAndTime { get; set; }

        [JsonProperty("USAGE_AMOUNT")]
        public string UsageAmount { get; set; }

        [JsonProperty("MAIN_IMG_NORMAL")]
        public string MainImgNormal { get; set; }

        [JsonProperty("MAIN_IMG_THUMB")]
        public string MainImgThumb { get; set; }

        [JsonProperty("ITEMCNTNTS")]
        public string ItemCntnts { get; set; }

        [JsonProperty("MIDDLE_SIZE_RM1")]
        public string MiddleSizeRm1 { get; set; }
    }
}
