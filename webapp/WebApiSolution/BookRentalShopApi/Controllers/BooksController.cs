using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using BookRentalShopApi.Models;

namespace BookRentalShopApi.Controllers
{
    // 이 컨트롤러는 /api/books 주소로 접근하게 됩니다.
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        // DB 연결 문자열을 저장할 변수
        private readonly string connString;

        // IConfiguration을 통해 appsettings.json의 값을 읽어옵니다.
        public BooksController(IConfiguration configuration)
        {
            // appsettings.json에 있는 ConnectionStrings:BookRentalShopConnection 값을 읽어옵니다.
            connString = configuration.GetConnectionString("BookRentalShopConnection")!;
        }

        /// <summary>
        /// 책 목록 조회
        /// </summary>
        /// <returns>책 목록</returns>
        [HttpGet]
        public async Task<IActionResult> GetBooksAsync()
        {
            // 결과를 담을 리스트
            List<Book> books = new();

            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            // books 테이블의 모든 책을 가져오는 SQL
            // 실제 테이블명과 컬럼명이 다르면 수정해야 함
            string query = @"
        SELECT
            book_idx,
            author,
            div_code,
            book_name,
            release_dt,
            isbn,
            price
        FROM books
        ORDER BY book_idx DESC
    ";

            using var cmd = new MySqlCommand(query, conn);

            using var reader = await cmd.ExecuteReaderAsync();

            // 한 줄씩 읽어서 Book 객체로 변환
            while (await reader.ReadAsync())
            {
                Book book = new Book
                {
                    BookIdx = reader.GetInt32("book_idx"),

                    Author = reader.IsDBNull(reader.GetOrdinal("author"))
                        ? null
                        : reader.GetString("author"),

                    DivCode = reader.IsDBNull(reader.GetOrdinal("div_code"))
                        ? null
                        : reader.GetString("div_code"),

                    BookName = reader.IsDBNull(reader.GetOrdinal("book_name"))
                        ? null
                        : reader.GetString("book_name"),

                    ReleaseDt = reader.IsDBNull(reader.GetOrdinal("release_dt"))
                        ? null
                        : reader.GetDateTime("release_dt"),

                    Isbn = reader.IsDBNull(reader.GetOrdinal("isbn"))
                        ? null
                        : reader.GetString("isbn"),

                    Price = reader.GetDecimal("price")
                };

                books.Add(book);
            }

            return Ok(books);
        }

        /// <summary>
        /// 책 1건 조회
        /// </summary>
        /// <param name="id">책 번호</param>
        /// <returns>책 1권 정보</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookAsync(int id)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            // book_idx가 id와 같은 책 1건만 조회
            string query = @"
        SELECT
            book_idx,
            author,
            div_code,
            book_name,
            release_dt,
            isbn,
            price
        FROM books
        WHERE book_idx = @BookIdx
    ";

            using var cmd = new MySqlCommand(query, conn);

            // SQL의 @BookIdx에 화면에서 받은 id 값을 넣음
            cmd.Parameters.AddWithValue("@BookIdx", id);

            using var reader = await cmd.ExecuteReaderAsync();

            // 결과가 없으면 404 처리
            if (!await reader.ReadAsync())
            {
                return NotFound($"책 번호 {id}를 찾을 수 없습니다.");
            }

            // DB 한 줄을 Book 객체로 변환
            Book book = new Book
            {
                BookIdx = reader.GetInt32("book_idx"),

                Author = reader.IsDBNull(reader.GetOrdinal("author"))
                    ? null
                    : reader.GetString("author"),

                DivCode = reader.IsDBNull(reader.GetOrdinal("div_code"))
                    ? null
                    : reader.GetString("div_code"),

                BookName = reader.IsDBNull(reader.GetOrdinal("book_name"))
                    ? null
                    : reader.GetString("book_name"),

                ReleaseDt = reader.IsDBNull(reader.GetOrdinal("release_dt"))
                    ? null
                    : reader.GetDateTime("release_dt"),

                Isbn = reader.IsDBNull(reader.GetOrdinal("isbn"))
                    ? null
                    : reader.GetString("isbn"),

                Price = reader.GetDecimal("price")
            };

            return Ok(book);
        }

        /// <summary>
        /// 책 등록
        /// </summary>
        /// <param name="book">등록할 책 정보</param>
        /// <returns>등록된 책 정보</returns>
        [HttpPost]
        public async Task<IActionResult> CreateBookAsync(Book book)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            // books 테이블에 새 책을 넣는 SQL
            // book_idx는 보통 자동 증가(Auto Increment)라고 가정
            string query = @"
        INSERT INTO books
        (
            author,
            div_code,
            book_name,
            release_dt,
            isbn,
            price
        )
        VALUES
        (
            @Author,
            @DivCode,
            @BookName,
            @ReleaseDt,
            @Isbn,
            @Price
        );

        SELECT LAST_INSERT_ID();
    ";

            using var cmd = new MySqlCommand(query, conn);

            // SQL 파라미터에 C# 값 넣기
            cmd.Parameters.AddWithValue("@Author", book.Author);
            cmd.Parameters.AddWithValue("@DivCode", book.DivCode);
            cmd.Parameters.AddWithValue("@BookName", book.BookName);
            cmd.Parameters.AddWithValue("@ReleaseDt", book.ReleaseDt);
            cmd.Parameters.AddWithValue("@Isbn", book.Isbn);
            cmd.Parameters.AddWithValue("@Price", book.Price);

            // 새로 추가된 book_idx 가져오기
            var newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

            // 방금 생성된 PK 값을 객체에 넣어줌
            book.BookIdx = newId;

            return Ok(book);
        }
        /// <summary>
        /// 책 수정
        /// </summary>
        /// <param name="id">수정할 책 번호</param>
        /// <param name="book">수정할 책 정보</param>
        /// <returns>수정 결과</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBookAsync(int id, Book book)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            // book_idx가 id와 같은 행을 수정
            string query = @"
        UPDATE books
           SET
               author = @Author,
               div_code = @DivCode,
               book_name = @BookName,
               release_dt = @ReleaseDt,
               isbn = @Isbn,
               price = @Price
         WHERE book_idx = @BookIdx
    ";

            using var cmd = new MySqlCommand(query, conn);

            // 바꿀 값들을 파라미터로 전달
            cmd.Parameters.AddWithValue("@Author", book.Author);
            cmd.Parameters.AddWithValue("@DivCode", book.DivCode);
            cmd.Parameters.AddWithValue("@BookName", book.BookName);
            cmd.Parameters.AddWithValue("@ReleaseDt", book.ReleaseDt);
            cmd.Parameters.AddWithValue("@Isbn", book.Isbn);
            cmd.Parameters.AddWithValue("@Price", book.Price);
            cmd.Parameters.AddWithValue("@BookIdx", id);

            // 실제로 몇 행이 수정되었는지 확인
            int result = await cmd.ExecuteNonQueryAsync();

            // 수정된 행이 없으면 없는 책 번호로 판단
            if (result == 0)
            {
                return NotFound($"책 번호 {id}를 찾을 수 없습니다.");
            }

            return Ok("책 정보가 수정되었습니다.");
        }


        /// <summary>
        /// 책 삭제
        /// </summary>
        /// <param name="id">삭제할 책 번호</param>
        /// <returns>삭제 결과</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookAsync(int id)
        {
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();

            // book_idx가 id와 같은 책 1건 삭제
            string query = @"
        DELETE FROM books
        WHERE book_idx = @BookIdx
    ";

            using var cmd = new MySqlCommand(query, conn);

            // URL로 받은 id를 SQL 파라미터에 넣음
            cmd.Parameters.AddWithValue("@BookIdx", id);

            // 실제로 삭제된 행 수 확인
            int result = await cmd.ExecuteNonQueryAsync();

            // 삭제된 행이 없으면 없는 책 번호로 판단
            if (result == 0)
            {
                return NotFound($"책 번호 {id}를 찾을 수 없습니다.");
            }

            return Ok("책이 삭제되었습니다.");
        }

    }
}