using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using WpfBusanFestivalApp.Models;

namespace WpfBusanFestivalApp.Services
{
    public class FestivalApiService
    {
        // 서비스키가 공개됨. github 등
        // private const string ServiceKey = "...e8367301815669";
        private string? ServiceKey { get; set; }
        
        public FestivalApiService() { 
        
            ServiceKey = Environment.GetEnvironmentVariable("BUSAN_FESTIVAL_API_KEY");
        }

        // OpenApi를 네트워크 요청시 응답이 늦으면 UI스레드 충돌 때문에 응답 없음 뜰 수 있음
        public async Task<ObservableCollection<FestivalItem>> GetFestivalsAsync(int pageNo = 1, int numOfRows = 10)
        {
            if (ServiceKey == null)
            {
                return null;
            }

            string serviceUrl = $"https://apis.data.go.kr/6260000/FestivalService/getFestivalKr" +
                                $"?serviceKey={ ServiceKey}" +
                                $"&pageNo={pageNo}" +
                                $"&numOfRows={numOfRows}" +
                                $"&resultType=json";

            using HttpClient client = new HttpClient();

            string json = await client.GetStringAsync(serviceUrl);

            // 데이터 직렬화(Serialization)로 네트워크 다운로드
            // 역직렬화로 데이터 변환
            FestivalResponse? response = JsonConvert.DeserializeObject <FestivalResponse>(json);

            return response.FestivalData.Items;
        }
    }
}
