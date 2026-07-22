using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace ResponseAiServer.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class NetServiceController : ControllerBase {

        private readonly IHttpClientFactory _httpClientFactory;

        public NetServiceController(IHttpClientFactory httpClientFactory) {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        [Route("/net_service")]
        public async Task<IActionResult> ProxyRequest([FromForm] string message, [FromForm] IFormFile file) {
            // 1. 파일 선택 안 했으면
            if (file == null || file.Length == 0) {
                return BadRequest(new { Message = "파일을 선택하세요." });
            }

            // 2. Program.cs에 등록한 PythonAI 서버 이름으로 클라이언트 생성
            var client = _httpClientFactory.CreateClient("PythonAiService");

            // 3. Python RestAPI로 전달할 데이터 할당
            using var content = new MultipartFormDataContent();

            // 3.1 Request Body 중 message 키할당
            content.Add(new StringContent(message), "message");

            // 3.2 Request Body 중 file 키할당
            //// 파일 스트림 추가
            using var stream = file.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.FileName);


            // 4. Python API로 Post 요청
            var response = await client.PostAsync("/detect", content);
            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, "파이썬 AI 서비스 호출 실패!");
            }

            // 5. 돌아온 결과를 읽어서 json으로 출력
            var result = await response.Content.ReadAsStringAsync();
            return Content(result, "application/json");

        }
    }
}
