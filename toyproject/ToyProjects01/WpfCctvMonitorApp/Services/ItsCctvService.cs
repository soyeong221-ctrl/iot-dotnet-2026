using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using WpfCctvMonitorApp.Common;
using WpfCctvMonitorApp.Models;

namespace WpfCctvMonitorApp.Services
{
    public class ItsCctvService
    {
        private readonly HttpClient httpClient = new();

        // TODO
        public async Task<CctvResponse> GetCctvListAsync(string apiUrl)
        {

            string json = await httpClient.GetStringAsync(apiUrl);

            var result = JsonConvert.DeserializeObject<CctvResponse>(json);

            if (result == null)
                return new CctvResponse();
            else
                return result;
        }

        public async Task<List<CctvResultDto>> GetBrideApiAsync(CctvRequest request)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, AppCommon.baseUrl);

            req.Content = new StringContent(
                JsonConvert.SerializeObject(request),
                Encoding.UTF8,
                "application/json"
                );
            var result = await httpClient.SendAsync(req);
        }
    }
}