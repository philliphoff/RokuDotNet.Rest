using System.Threading;
using System.Threading.Tasks;
using RokuDotNet.Client;
using RokuDotNet.Rest;

namespace Sample
{
    internal sealed class RokuDeviceProvider : IRokuDeviceProvider
    {
        #region IRokuDeviceProvider Members

        public async Task<IRokuDevice> GetDeviceFromIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var discoveryClient = new UdpRokuDeviceDiscoveryClient();

            var device = await discoveryClient.DiscoverFirstDeviceAsync().ConfigureAwait(false);

            return device;
        }

        #endregion
    }
}