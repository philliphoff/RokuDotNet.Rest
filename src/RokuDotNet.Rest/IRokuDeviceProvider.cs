using System.Threading;
using System.Threading.Tasks;
using RokuDotNet.Client;

namespace RokuDotNet.Rest
{
    public interface IRokuDeviceProvider
    {
        Task<IRokuDevice> GetDeviceFromIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken));
    }
}