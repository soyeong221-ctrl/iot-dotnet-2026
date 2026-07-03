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
            // var result = await service.GetCctvListAsync("https://openapi.its.go.kr:9443/cctvInfo");
            var result = await service.GetCctvSearchAsync("");
            return Ok(result);
        }
    }
}

