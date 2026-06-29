using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net.Http;
using WpfProductAdmin.Models;

public class ApiService
{
    private readonly HttpClient client = new HttpClient();

    private readonly string serviceUrl = "http://localhost:5276/api/products";

    // 전체 조회 GET http://localhost:5276/api/products
    public async Task<ObservableCollection<Product>> GetProductsAsync()
    {
        try
        {
            // 공공데이터포털과 완전 동일
            //using HttpClient client = new HttpClient();
            string json = await client.GetStringAsync(serviceUrl);

            // 역직렬화 - 여러건 리스트
            var response = JsonConvert.DeserializeObject<ObservableCollection<Product>>(json);

            return response;
        }
        catch (Exception ex)
        {
            return new ObservableCollection<Product>();
        }
    }

    // 한건 저장 POST http://localhost:5276/api/products
    public async Task<bool> PostProductAsync(Product product)
    {
        var response = await client.PostAsJsonAsync(serviceUrl, product);

        return response.IsSuccessStatusCode;
    }

    // 한 건 가져오기 GET http://localhost:5276/api/products/id
    public async Task<Product> GetProductAsync(int productId)
    {
        string json = await client.GetStringAsync($"{serviceUrl}/{productId}");
        // 역직렬화 - 데이터 한건
        var response = JsonConvert.DeserializeObject<Product>(json);
        return response;
    }

    // 한 건 수정하기 PUT http://localhost:5276/api/products/id
    // 실무에서 PATCH 거의 안씀. PUT으로 가능
    public async Task<bool> UpdateProductAsync(Product product)
    {
        var response = await client.PutAsJsonAsync($"{serviceUrl}/{product.ProductId}", product);

        return response.IsSuccessStatusCode;  // 200(OK) -> true, 400(Error), 500(Server Error) -> false
    }

    // 한건 삭제하기 DELETE http://localhost:5276/api/products/id
    public async Task<bool> DeleteProductAsync(int productId)
    {
        var response = await client.DeleteAsync($"{serviceUrl}/{productId}");

        return response.IsSuccessStatusCode;
    }
}