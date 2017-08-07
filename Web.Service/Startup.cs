// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Common.Factorys;
using Microsoft.Practices.Unity;
using Reservation.Domain;
using Show.Domain;
using Unity.WebApi;
using Web.Service.Controllers;

namespace Web.Service
{
    using System.Web.Http;
    using Microsoft.Owin;
    using Microsoft.Owin.FileSystems;
    using Microsoft.Owin.StaticFiles;
    using Owin;

    internal class Startup : IOwinAppBuilder
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder.UseWebApi(GetHttpConfiguration());
            appBuilder.UseFileServer(GetFileServerOptions());
        }

        private static HttpConfiguration GetHttpConfiguration()
        {
            var config = new HttpConfiguration();
            FormatterConfig.ConfigureFormatters(config.Formatters);
            config.MapHttpAttributeRoutes();
            config = RegisterDependencys(config);
            return config;
        }

        private static HttpConfiguration RegisterDependencys(HttpConfiguration config)
        {
            var container = new UnityContainer();
            container.RegisterType<IServiceFactory<IReservationService>, ReservationServiceFactory>();
            container.RegisterType<IServiceFactory<IShowService>, ShowServiceFactory>();
            container.RegisterType<IPartitionManager, PartitionManager>();

//            container.RegisterTypes(
//                AllClasses.FromLoadedAssemblies(),
//                WithMappings.FromMatchingInterface,
//                WithName.Default);

            config.DependencyResolver = new UnityDependencyResolver(container);
            return config;
        }

        private static FileServerOptions GetFileServerOptions()
        {
            var physicalFileSystem = new PhysicalFileSystem(@".\wwwroot");
            var fileOptions = new FileServerOptions();
            fileOptions.EnableDefaultFiles = true;
            fileOptions.RequestPath = PathString.Empty;
            fileOptions.FileSystem = physicalFileSystem;
            fileOptions.DefaultFilesOptions.DefaultFileNames = new[] {"index.html"};
            fileOptions.StaticFileOptions.FileSystem = fileOptions.FileSystem = physicalFileSystem;
            fileOptions.StaticFileOptions.ServeUnknownFileTypes = true;
            fileOptions.EnableDirectoryBrowsing = true;
            return fileOptions;
        }
    }
}