using Microsoft.AspNetCore.Mvc;
using RokuDotNet.Client;
using RokuDotNet.Client.Apps;
using RokuDotNet.Client.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace RokuDotNet.Rest
{
    [Route("api/[controller]")]
    public class RokuController : ControllerBase
    {
        private readonly IRokuDeviceProvider deviceProvider;

        public RokuController(IRokuDeviceProvider deviceProvider)
        {
            this.deviceProvider = deviceProvider ?? throw new ArgumentNullException(nameof(deviceProvider));
        }

        [Route("devices/{id}/query/device-info")]
        public async Task<DeviceInfo> GetDeviceInfoAsync(string id, CancellationToken cancellationToken)
        {
            var device = await this.deviceProvider.GetDeviceFromIdAsync(id, cancellationToken);

            return await device.GetDeviceInfoAsync(cancellationToken);
        }

        #region Apps API

        [Route("devices/{id}/query/apps")]
        public async Task<GetAppsResult> GetAppsAsync(string id, CancellationToken cancellationToken)
        {
            var device = await this.deviceProvider.GetDeviceFromIdAsync(id, cancellationToken);

            return await device.Apps.GetAppsAsync(cancellationToken);
        }

        [Route("devices/{id}/query/active-app")]
        public async Task<GetActiveAppResult> GetActiveAppAsync(string id, CancellationToken cancellationToken)
        {
            var device = await this.deviceProvider.GetDeviceFromIdAsync(id, cancellationToken);

            return await device.Apps.GetActiveAppAsync(cancellationToken);
        }
 
        [Route("devices/{id}/query/tv-channels")]
        public async Task<GetTvChannelsResult> GetTvChannelsAsync(string id, CancellationToken cancellationToken)
        {
            var device = await this.deviceProvider.GetDeviceFromIdAsync(id, cancellationToken);

            return await device.Apps.GetTvChannelsAsync(cancellationToken);
        }

        [Route("devices/{id}/query/tv-active-channel")]
        public async Task<GetActiveTvChannelResult> GetActiveTvChannelAsync(string id, CancellationToken cancellationToken)
        {
            var device = await this.deviceProvider.GetDeviceFromIdAsync(id, cancellationToken);

            return await device.Apps.GetActiveTvChannelAsync(cancellationToken);
        }

        [Route("devices/{deviceId}/install/{appId}")]
        public async Task PostInstallAppAsync(string deviceId, string appId, [FromQuery] IDictionary<string, string> parameters, CancellationToken cancellationToken)
        {
            var device = await this.deviceProvider.GetDeviceFromIdAsync(deviceId, cancellationToken);

            await device.Apps.InstallAppAsync(appId, parameters, cancellationToken);
        }

        [Route("devices/{deviceId}/launch/{appId}")]
        public async Task PostLaunchAppAsync(string deviceId, string appId, CancellationToken cancellationToken)
        {
            var device = await this.deviceProvider.GetDeviceFromIdAsync(deviceId, cancellationToken);

            var parameters = HttpContext.Request.Query.ToDictionary(parameter => parameter.Key, parameter => parameter.Value.ToString());

            await device.Apps.LaunchAppAsync(appId, parameters, cancellationToken);
        }

        [Route("devices/{deviceId}/launch/tvinput.dtv")]
        public async Task PostLaunchTvInputAsync(string deviceId, [FromQuery] string ch, CancellationToken cancellationToken)
        {
            var device = await this.deviceProvider.GetDeviceFromIdAsync(deviceId, cancellationToken);

            if (String.IsNullOrEmpty(ch))
            {
                await device.Apps.LaunchTvInputAsync(cancellationToken);
            }
            else
            {
                await device.Apps.LaunchTvInputAsync(ch, cancellationToken);
            }
        }

        #endregion

        #region Input API

        [Route("devices/{id}/keydown/{key}")]
        public Task PostKeyDownAsync(string id, string key, CancellationToken cancellationToken)
        {
            return this.KeyInputAsync(id, key, device => device.Input.KeyDownAsync, cancellationToken);
        }

        [Route("devices/{id}/keyup/{key}")]
        public Task PostKeyUpAsync(string id, string key, CancellationToken cancellationToken)
        {
            return this.KeyInputAsync(id, key, device => device.Input.KeyUpAsync, cancellationToken);
        }

        [Route("devices/{id}/keypress/{key}")]
        public Task PostKeyPressAsync(string id, string key, CancellationToken cancellationToken)
        {
            return this.KeyInputAsync(id, key, device => device.Input.KeyPressAsync, cancellationToken);
        }

        [Route("devices/{id}/keypresses")]
        public async Task PostKeyPressesLiteralAsync(string id, [FromQuery(Name="keys")] string[] keys, CancellationToken cancellationToken)
        {
            var device = await this.deviceProvider.GetDeviceFromIdAsync(id);

            var decodedKeys =
                keys
                    .Select(key => InputEncoding.DecodeString(key))
                    .Select(
                        decodedKey =>
                        {
                            if (decodedKey.Item1.HasValue)
                            {
                                return new PressedKey(decodedKey.Item1.Value);
                            }
                            else if (decodedKey.Item2.HasValue)
                            {
                                return new PressedKey(decodedKey.Item2.Value);
                            }

                            throw new InvalidOperationException();
                        });

            await device.Input.KeyPressAsync(decodedKeys, cancellationToken);
        }

        #endregion

        private async Task KeyInputAsync(string id, string key, Func<IRokuDevice, Func<PressedKey, CancellationToken, Task>> inputCharAction, CancellationToken cancellationToken)
        {
            var device = await this.deviceProvider.GetDeviceFromIdAsync(id);

            // CONSIDER: Refactor DecodeString() to return PressedKey.
            var (inputChar, specialKey) = InputEncoding.DecodeString(key);

            if (inputChar.HasValue)
            {
                await inputCharAction(device)(inputChar.Value, cancellationToken);
            }
            else if (specialKey.HasValue)
            {
                await inputCharAction(device)(specialKey.Value, cancellationToken);
            }

            // TODO: Return 404.
        }
    }
}
