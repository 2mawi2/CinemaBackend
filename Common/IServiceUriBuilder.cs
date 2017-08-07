using System;
using System.Fabric;

namespace Common
{
    public interface IServiceUriBuilder
    {
        ICodePackageActivationContext ActivationContext { get; set; }
        string ApplicationInstance { get; set; }
        string ServiceInstance { get; set; }
        Uri ToUri();
    }
}