using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;

namespace Common
{
    public interface IBackupManager
    {
        Task<bool> OnDataLossAsync(RestoreContext restoreCtx, CancellationToken cancellationToken, IStatefulServicePartition servicePartition);
        Task PeriodicTakeBackupAsync(CancellationToken cancellationToken, IStatefulServicePartition servicePartition);
    }
}