using Microsoft.AspNetCore.Mvc;
using RokuDotNet.Client;
using RokuDotNet.Client.Query;
using System;
using System.Threading.Tasks;

namespace RokuDotNet.Rest
{
    [Route("[controller]")]
    public class RokuController : Controller
    {
        [Route("devices/{id}/query/device-info")]
        public async Task<DeviceInfo> GetDeviceInfoAsync(string id)
        {
            var discoveryClient = new RokuDeviceDiscoveryClient();

            var device = await discoveryClient.DiscoverSpecificDeviceAsync($"uuid:roku:ecp:{id}");
            var deviceInfo = await device.QueryApi.GetDeviceInfoAsync();

            return deviceInfo;
        }

        [HttpGet("test")]
        public Task<string> Test()
        {
            return Task.FromResult("Hello world!");
        }
    }
}
