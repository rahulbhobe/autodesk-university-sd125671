using System;
using Amazon;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;

namespace DetailsDemo
{
    public class Application : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            RegisterServer(new StandardDetailsLibraryServer());
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

        public static string BaseFolder => @"\\usbospdfilsrv1\QA_TestData\Reg_Misc\RFAAS\AutodeskUniversity\DetailsDemo\";
        public static Guid DBServerId => new Guid("aae434c7-0520-4c03-b05c-fd6ff5a0839c");
    }
}
