// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;

namespace Common.Utils
{
    public class ServiceUriBuilder : IServiceUriBuilder
    {
        public ServiceUriBuilder(string serviceInstance)
        {
            ActivationContext = FabricRuntime.GetActivationContext();
            ServiceInstance = serviceInstance;
        }

        public ServiceUriBuilder(ICodePackageActivationContext context, string serviceInstance)
        {
            ActivationContext = context;
            ServiceInstance = serviceInstance;
        }

        public ServiceUriBuilder(ICodePackageActivationContext context, string applicationInstance, string serviceInstance)
        {
            ActivationContext = context;
            ApplicationInstance = applicationInstance;
            ServiceInstance = serviceInstance;
        }

        /// <summary>
        /// The name of the application instance that contains he service.
        /// </summary>
        public string ApplicationInstance { get; set; }

        /// <summary>
        /// The name of the service instance.
        /// </summary>
        public string ServiceInstance { get; set; }

        /// <summary>
        /// The local activation context
        /// </summary>
        public ICodePackageActivationContext ActivationContext { get; set; }

        public Uri ToUri()
        {
            var applicationInstance = ApplicationInstance;
            if (string.IsNullOrEmpty(applicationInstance))
            {
                // the ApplicationName property here automatically prepends "fabric:/" for us
                applicationInstance = ActivationContext.ApplicationName.Replace("fabric:/", String.Empty);
            }
            return new Uri("fabric:/" + applicationInstance + "/" + ServiceInstance);
        }
    }
}