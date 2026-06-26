using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using MySqlConnector;
using ProductApi.Models;

namespace ProductApi.Controllers
{
    [ApiController] // RestAPI 서비스용 정의
    [Route("api/[controller]")] // 요청할 URL이 localhost:port/api/products
    public class ProductsController : ControllerBase
    {
        readonly string connString;

        public ProductsController(IConfiguration configuration)
        {
            connString = configuration.GetConnectionString("TestDbConnection");
        }

        /// <summary>
        /// 상품 리스트 조회
        /// </summary>
        /// <returns></returns>
        [HttpGet]   // GET 메서드 선언(없어도 기본)
        public async Task<IActionResult> GetProductsAsync()
        {
            List<Product> products = new(); // new List<Product>(); 와 동일기능

            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                @"
                 SELECT product_id, product_name, category, price, stock, created_at
                 FROM products
                 ORDER BY product_id DESC
                ";      // 여러 줄 문자열: @" 또는 """

            using var cmd = new MySqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            { //  
                Product product = new Product
                {
                    ProductId = reader.GetInt32("product_id"),
                    ProductName = reader.GetString("product_name"),
                    Category = reader.GetString("category"),
                    Price = reader.GetDecimal("price"),
                    Stock = reader.GetInt32("stock"),
                    CreatedAt = reader.GetDateTime("created_at")
                };

                products.Add(product);

            }
            return Ok(products);
        }

        /// <summary>
        /// 상품 조회
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]   // GET /api/products/3
        public async Task<IActionResult> GetProductAsync(int id)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                @"
                 SELECT product_id, product_name, category, price, stock, created_at
                 FROM products
                 WHERE product_id =@ProductId
                ";      // MySQL, SQLServer 파라미터 문법 @파라미터명

            using var cmd = new MySqlCommand(query, conn);

            // @ProductId를 매핑하는 파라미터 생성
            cmd.Parameters.AddWithValue("@ProductId", id);

            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return NotFound($"상품번호 {id}를 찾을 수 없습니다.");
            }

            // 한 건 가져와서 Product 객체로 만듦
            Product product = new Product
            {
                ProductId = reader.GetInt32("product_id"),
                ProductName = reader.GetString("product_name"),
                Category = reader.GetString("category"),
                Price = reader.GetDecimal("price"),
                Stock = reader.GetInt32("stock"),
                CreatedAt = reader.GetDateTime("created_at")
            };

            return Ok(product);
        }

        /// <summary>
        /// 상품 등록
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                @"
                    INSERT INTO products
                    (
                       product_name, 
                       category, 
                       price, 
                       stock
                    )
                    VALUES
                    (
                       @ProductName, 
                       @Category, 
                       @Price, 
                       @Stock
                    );

                    SELECT LAST_INSERT_ID();
                ";  // PK가 AutoIncrement이면 LAST_INSERT_ID() 새로 추가된 PK를 가져와야 함

            using var cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
            cmd.Parameters.AddWithValue("@Category", product.Category);
            cmd.Parameters.AddWithValue("@Price", product.Price);
            cmd.Parameters.AddWithValue("@Stock", product.Stock);

            // ExecuteSvalarAsync 리턴값은 object? -> int로 형변환
            var newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

            product.ProductId = newId;
            product.CreatedAt = DateTime.Now;   // DB에서 안 받는 이유. DB에 저장되는 시간과 거의 동일

            // 정상적으로 Post가 발생해서 데이터생성되었다. 생성된 상품으로 URL에서 다시 조회할 수 있음
            /*
            return CreatedAtAction(
                nameof(GetProductAsync),
                new { id = product.ProductId },
                product
                );
            */
            return Ok(product);
        }

        /// <summary>
        /// 상품 수정
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                @"
                    UPDATE products
                       SET 
                           product_name = @ProductName, 
                           category = @Category, 
                           price = @Price, 
                           stock = @Stock
                     WHERE product_id = @ProductId
                ";

            using var cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
            cmd.Parameters.AddWithValue("@Category", product.Category);
            cmd.Parameters.AddWithValue("@Price", product.Price);
            cmd.Parameters.AddWithValue("@Stock", product.Stock);
            cmd.Parameters.AddWithValue("@ProductId", id);

            int result = await cmd.ExecuteNonQueryAsync();

            if (result == 0)
            {
                return NotFound($"상품번호 {id}를 찾을 수 없습니다.");
            }

            return Ok("상품이 수정되었습니다.");
        }

        /// <summary>
        /// 상품 단일 컬럼 수정
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateProductStock(int id, ProductStock product)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query =
                @"
                    UPDATE products
                       SET 
                           stock = @Stock
                     WHERE product_id = @ProductId
                ";

            using var cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@Stock", product.Stock);
            cmd.Parameters.AddWithValue("@ProductId", id);

            int result = await cmd.ExecuteNonQueryAsync();

            if (result == 0)
            {
                return NotFound($"상품번호 {id}를 찾을 수 없습니다.");
            }

            return Ok("상품 재고가 수정되었습니다.");
        }

        /// <summary>
        /// 상품 삭제
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            string query = @"DELETE FROM products WHERE product_id = @ProductId ";

            using var cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@ProductId", id);

            int result = await cmd.ExecuteNonQueryAsync();

            if (result == 0)
            {
                return NotFound($"상품번호 {id}를 찾을 수 없습니다.");
            }
            return Ok("상품이 삭제되었습니다.");
        }

        [HttpHead("{id}")]
        public IActionResult Head(int id)
        {
            return Ok();
        }

        [HttpOptions]
        public IActionResult Options() {

            Response.Headers.Append(
                "Allow",
                "GET, POST, PUT, PATCH, DELETE"
                );
            return Ok();
        }

        // [HttpMethod("GET, POST")] 쓸모 없음

    }
}
