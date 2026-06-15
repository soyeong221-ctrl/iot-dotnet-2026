using WpfBusanFoodApp.Helpers;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net.Http;
using WpfBusanFoodApp.Models;

namespace WpfBusanFoodApp.Services
{
    // 부산맛집 OpenAPI와 통신하는 클래스.
    // MainWindow는 이 클래스를 통해 맛집 데이터 요청.
    public class FoodApiService
    {
        private readonly string? serviceKey;

        public FoodApiService()
        {
            // setx BUSAN_FOOD_API_KEY "발급받은키" 로 등록한 값을 가져옵니다.
            serviceKey = Environment.GetEnvironmentVariable("BUSAN_FOOD_API_KEY");

            if (string.IsNullOrWhiteSpace(serviceKey))
            {
                Common.Logger.Warn("BUSAN_FOOD_API_KEY 환경변수가 설정되지 않았습니다.");
            }
        }

        // 맛집 목록을 가져오는 비동기 메서드.
        // async/await -> API 응답을 기다리는 동안 앱 화면이 멈추지 않습니다.
        public async Task<ObservableCollection<FoodItem>> GetFoodsAsync(int pageNo = 1, int numOfRows = 10)
        {
            // API 키가 없으면 빈 목록 반환.
            if (string.IsNullOrWhiteSpace(serviceKey))
            {
                return new ObservableCollection<FoodItem>();
            }

            // pageNo: 몇 페이지를 가져올지
            // numOfRows: 한 페이지에 몇 개를 가져올지
            // resultType=json: JSON 형식으로 받겠다는 뜻.
            string url = $"https://apis.data.go.kr/6260000/FoodService/getFoodKr" +
                         $"?serviceKey={serviceKey}" +
                         $"&pageNo={pageNo}" +
                         $"&numOfRows={numOfRows}" +
                         $"&resultType=json";

            try
            {
                // HttpClient는 웹 API에 요청을 보내는 클래스.
                using HttpClient client = new HttpClient();

                // API 주소로 요청을 보내고 JSON 문자열을 받습니다.
                string json = await client.GetStringAsync(url);

                // JSON 문자열을 C# 객체로 변환합니다.
                FoodResponse? response = JsonConvert.DeserializeObject<FoodResponse>(json);

                // response, FoodData, Items 중 하나라도 null이면 빈 목록을 반환합니다.
                return response?.FoodData?.Items ?? new ObservableCollection<FoodItem>();
            }
            catch (Exception ex)
            {
                Common.Logger.Error(ex, "부산맛집정보 API 호출 실패");

                return new ObservableCollection<FoodItem>();
            }
        }
    }
}