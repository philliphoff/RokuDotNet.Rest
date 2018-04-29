using Microsoft.AspNetCore.Mvc;
using RokuDotNet.Client;
using RokuDotNet.Client.Input;
using RokuDotNet.Client.Query;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace RokuDotNet.Rest
{
    [Route("api/[controller]")]
    public class RokuController : Controller
    {
        private readonly IRokuDeviceProvider deviceProvider;

        public RokuController(IRokuDeviceProvider deviceProvider)
        {
            this.deviceProvider = deviceProvider ?? throw new ArgumentNullException(nameof(deviceProvider));
        }

        #region Query API

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

        #endregion

        #region Input API

        [Route("devices/{id}/keydown/{key}")]
        public Task PostKeyDownAsync(string id, string key, CancellationToken cancellationToken)
        {
            return this.KeyInputAsync(id, key, device => device.InputApi.KeyDownAsync, device => device.InputApi.KeyDownAsync, cancellationToken);
        }

        [Route("devices/{id}/keyup/{key}")]
        public Task PostKeyUpAsync(string id, string key, CancellationToken cancellationToken)
        {
            return this.KeyInputAsync(id, key, device => device.InputApi.KeyUpAsync, device => device.InputApi.KeyUpAsync, cancellationToken);
        }

        [Route("devices/{id}/keypress/{key}")]
        public Task PostKeyPressAsync(string id, string key, CancellationToken cancellationToken)
        {
            return this.KeyInputAsync(id, key, device => device.InputApi.KeyPressAsync, device => device.InputApi.KeyPressAsync, cancellationToken);
        }

        #endregion

        private async Task KeyInputAsync(string id, string key, Func<IRokuDevice, Func<char, CancellationToken, Task>> inputCharAction, Func<IRokuDevice, Func<SpecialKeys, CancellationToken, Task>> specialKeysAction, CancellationToken cancellationToken)
        {
            var device = await this.deviceProvider.GetDeviceFromIdAsync(id);

            var (inputChar, specialKey) = InputEncoding.DecodeString(key);

            if (inputChar.HasValue)
            {
                await inputCharAction(device)(inputChar.Value, cancellationToken);
            }
            else if (specialKey.HasValue)
            {
                await specialKeysAction(device)(specialKey.Value, cancellationToken);
            }

            // TODO: Return 404.
        }
    }
}
