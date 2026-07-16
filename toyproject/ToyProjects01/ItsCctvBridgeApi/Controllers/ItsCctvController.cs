using ItsCctvBridgeApi.Models;
using ItsCctvBridgeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ItsCctvBridgeApi.Controllers
{
    [ApiController]
    [Route("api/itscctv")]
    public class ItsCctvController : ControllerBase
    {
        private readonly ItsCctvService service;

        public ItsCctvController(ItsCctvService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> SearchCctv(CctvRequest request)
        {
            //var result = await service.GetCctvListAsync("https://openapi.its.go.kr:9443/cctvInfo?apiKey=a3fd2209bc274adda49137bdf9a87acc&type=ex&cctvType=1&minX=126.7757&maxX=127.16&minY=37.4133&maxY=37.71&getType=json");
            var result = await service.GetCctvSearchAsync(request);

            return Ok(result);
        }
    }
}