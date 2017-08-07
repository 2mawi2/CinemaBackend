// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Web.Service
{
    using System;
    using System.Fabric;
    using System.Fabric.Description;
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Owin.Hosting;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;

    public class OwinCommunicationListener : ICommunicationListener
    {
        private readonly IOwinAppBuilder startup;
        private readonly string appRoot;
        private readonly ServiceContext serviceContext;
        private IDisposable serverHandle;
        private string listeningAddress;

        public OwinCommunicationListener(string appRoot, IOwinAppBuilder startup, ServiceContext serviceContext)
        {
            this.startup = startup;
            this.appRoot = appRoot;
            this.serviceContext = serviceContext;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            EndpointResourceDescription serviceEndpoint = serviceContext.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");
            int port = serviceEndpoint.Port;
            EndpointProtocol protocol = serviceEndpoint.Protocol;

            listeningAddress = String.Format(
                CultureInfo.InvariantCulture,
                "{0}://+:{1}/{2}",
                protocol,
                port,
                String.IsNullOrWhiteSpace(appRoot)
                    ? String.Empty
                    : appRoot.TrimEnd('/') + '/');

            try
            {
                serverHandle = WebApp.Start(listeningAddress, appBuilder => startup.Configuration(appBuilder));
            }
            catch (TargetInvocationException tie)
            {
                ServiceEventSource.Current.Message("TargetInvocationException during open, {0}", tie);
                throw;
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.Message("Exception during open, {0}", e);
                throw;
            }

            string resultAddress = listeningAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

            ServiceEventSource.Current.Message("Listening on {0}", resultAddress);

            return Task.FromResult(resultAddress);
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            StopWebServer();

            return Task.FromResult(true);
        }

        public void Abort()
        {
            StopWebServer();
        }

        private void StopWebServer()
        {
            if (serverHandle != null)
            {
                try
                {
                    serverHandle.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // no-op
                }
            }
        }
    }
}
