using System.Fabric;
using Microsoft.ServiceFabric.Data;

namespace Common
{
    public interface IServiceContextInfo
    {
        StatefulServiceContext StatefulServiceContext { get; set; }
        IStatefulServicePartition StatefulServicePartition { get; set; }
        IReliableStateManager StateManager { get; set; }
    }
}