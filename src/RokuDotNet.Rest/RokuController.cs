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

        [Route("devices/{id}/query/device-info")]
        public async Task<DeviceInfo> GetDeviceInfoAsync(string id)
        {
            var device = await this.deviceProvider.GetDeviceFromIdAsync(id);

            return await device.QueryApi.GetDeviceInfoAsync();
        }
    }
}
