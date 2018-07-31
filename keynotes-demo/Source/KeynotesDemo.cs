using System;
using System.Collections.Generic;
using Amazon;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;

namespace KeynotesDemo
{
    public class Application : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            RegisterServer(new DynamoDBKeynotesServer());
            return Result.Succeeded;
        }

        private static void RegisterServer(IExternalServer server)
        {
            ExternalService service = ExternalServiceRegistry.GetService(server.GetServiceId());
            service.AddServer(server);
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public static string AwsProfile => "rvtiodev";
        public static RegionEndpoint AwsRegion => RegionEndpoint.USEast1;
        public static Guid DBServerId => new Guid("a2699662-946a-495a-9649-a1d4af84675a");

        public static List<string> DynamoDBTables => new List<string>
        {
            "external-resource-demo-Keynotes",
            "external-resource-demo-Keynotes2",
        };
    }
}
