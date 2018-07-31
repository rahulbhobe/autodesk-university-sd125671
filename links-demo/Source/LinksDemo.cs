using System;
using Amazon;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;

namespace LinksDemo
{
    public class Application : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            RegisterServer(new LinksMultiLanguageServer());
            RegisterServer(new LinksMultiLanguageUIServer());
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

        public static string CurrentLanguage => "English";
        public static string BaseFolder => @"\\usbospdfilsrv1\QA_TestData\Reg_Misc\RFAAS\AutodeskUniversity\LinksDemo\";
        public static string AwsProfile => "rvtiodev";
        public static RegionEndpoint AwsRegion => RegionEndpoint.USEast1;
        public static string DynamoDBTable => "external-resource-demo-Links";
        public static Guid DBServerId => new Guid("be5e293b-26e5-428f-9606-ca16ed046311");
        public static Guid UIServerId => new Guid("d46f23a1-8efb-4e8f-a22e-2cd1228be813");
    }
}
