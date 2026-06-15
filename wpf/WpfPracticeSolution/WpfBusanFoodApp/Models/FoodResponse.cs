using Newtonsoft.Json;

namespace WpfBusanFoodApp.Models
{
    // API 전체 응답을 표현하는 클래스.
    // 가장 바깥쪽 JSON 구조를 받습니다.
    public class FoodResponse
    {
        // JSON의 "getFoodKr" 객체를 FoodData 속성에 연결.
        [JsonProperty("getFoodKr")]
        public FoodData FoodData { get; set; }
    }
}