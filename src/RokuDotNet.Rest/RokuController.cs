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
        private readonly IRokuDeviceProvider deviceProvider;

        public RokuController(IRokuDeviceProvider deviceProvider)
        {
            this.deviceProvider = deviceProvider ?? throw new ArgumentNullException(nameof(deviceProvider));
        }

        [Route("devices/{id}/query/apps")]
        public async Task<GetAppsResult> GetAppsAsync(string id)
        {
            var device = await this.deviceProvider.GetDeviceFromIdAsync(id);

            return await device.QueryApi.GetAppsAsync();
        }

        [Route("devices/{id}/query/active-app")]
        public async Task<GetActiveAppResult> GetActiveAppAsync(string id)
        {
            var device = await this.deviceProvider.GetDeviceFromIdAsync(id);

            return await device.QueryApi.GetActiveAppAsync();
        }

        [Route("devices/{id}/query/device-info")]
        public async Task<DeviceInfo> GetDeviceInfoAsync(string id)
        {
            var device = await this.deviceProvider.GetDeviceFromIdAsync(id);

            return await device.QueryApi.GetDeviceInfoAsync();
        }
 
        [Route("devices/{id}/query/tv-channels")]
        public async Task<GetTvChannelsResult> GetTvChannelsAsync(string id)
        {
            var device = await this.deviceProvider.GetDeviceFromIdAsync(id);

            return await device.QueryApi.GetTvChannelsAsync();
        }

        [Route("devices/{id}/query/tv-active-channel")]
        public async Task<GetActiveTvChannelResult> GetActiveTvChannelAsync(string id)
        {
            var device = await this.deviceProvider.GetDeviceFromIdAsync(id);

            return await device.QueryApi.GetActiveTvChannelAsync();
        }
    }
}
