using System;
using System.Fabric;
using Mocks;
using Reservation.Service.Service;

namespace ReservationItem.UnitTests
{
    public static class TestUtils
    {
        private static readonly ICodePackageActivationContext CodePackageContext = new MockCodePackageActivationContext(
            "fabric:/someapp",
            "SomeAppType",
            "Code",
            "1.0.0.0",
            Guid.NewGuid().ToString(),
            @"C:\Log",
            @"C:\Temp",
            @"C:\Work",
            "ServiceManifest",
            "1.0.0.0"
        );

        public static StatefulServiceContext StatefulServiceContext = new StatefulServiceContext(
            new NodeContext("Node0", new NodeId(0, 1), 0, "NodeType1", "TEST.MACHINE"),
            CodePackageContext,
            ReservationService.ReservationServiceType,
            new Uri("fabric:/someapp/someservice"),
            null,
            Guid.NewGuid(),
            long.MaxValue
        );
    }
}