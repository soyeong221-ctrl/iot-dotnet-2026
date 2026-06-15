using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace WpfBusanFoodApp.Models
{
    // API 응답 중 item 배열을 담는 클래스.
    public class FoodData
    {
        // JSON의 "item" 배열을 Items 속성에 연결.
        // ObservableCollection은 WPF 화면과 잘 연결되는 목록 타입.
        [JsonProperty("item")]
        public ObservableCollection<FoodItem> Items { get; set; }
    }
}