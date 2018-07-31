using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;

namespace DetailsDemo
{
    public class StandardDetailsLibraryServer : IExternalResourceServer
    {
        public StandardDetailsLibraryServer() { }
        public System.Guid GetServerId() => Application.DBServerId;
        public ExternalServiceId GetServiceId() => ExternalServices.BuiltInExternalServices.ExternalResourceService;
        public string GetShortName() => ServerName;
        public System.String GetName() => ServerName;
        public static string ServerName => "DetailsLib";
        public System.String GetVendorId() => "AU Demo";
        public System.String GetDescription() => "Provides CAD Details Library";

        public bool SupportsExternalResourceType(ExternalResourceType resourceType)
        {
            return (resourceType == ExternalResourceTypes.BuiltInExternalResourceTypes.CADLink);
        }

        public String GetInSessionPath(ExternalResourceReference resourceReference, String originalPath)
        {
            string file = resourceReference.GetReferenceInformation()["File"];
            DBUtils.DBItem dbItem = DBUtils.FindDBItem(file);
            return ServerName + "://" + FileUtils.GetRelativePath(Application.BaseFolder, dbItem.Path);
        }

        public void LoadResource(Guid loadRequestId, ExternalResourceType resourceType,
           ExternalResourceReference resourceReference, ExternalResourceLoadContext loadContext,
           ExternalResourceLoadContent content)
        {
            LinkLoadContent linkLoadContent = (LinkLoadContent)content;

            string file = resourceReference.GetReferenceInformation()["File"];
            DBUtils.DBItem dbItem = DBUtils.FindDBItem(file);
            ModelPath linksPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(dbItem.Path);
            linkLoadContent.SetLinkDataPath(linksPath);
            content.LoadStatus = ExternalResourceLoadStatus.Success;
            return;
        }

        public void SetupBrowserData(ExternalResourceBrowserData browserData)
        {
            string curPath = Application.BaseFolder + browserData.FolderPath;

            Directory.GetFiles(curPath).ToList().ForEach(path =>
            {
                Dictionary<string, string> referenceInformation = new Dictionary<string, string>();
                var file = Path.GetFileName(path);
                referenceInformation["File"] = file;
                browserData.AddResource(file, "1", referenceInformation);
            });

            Directory.GetDirectories(curPath).ToList().ForEach(path =>
            {
                var dir = Path.GetFileName(path);
                browserData.AddSubFolder(dir);
            });
        }

        public bool AreSameResources(IDictionary<string, string> refInfo1, IDictionary<string, string> refInfo2)
        {
            if (refInfo1.Count != refInfo2.Count)
                return false;
            return refInfo1.OrderBy(pair => pair.Key)
               .SequenceEqual(refInfo2.OrderBy(pair => pair.Key));
        }

        public bool IsResourceWellFormed(ExternalResourceReference extRef) => true;
        public string GetIconPath() => string.Empty;
        public ResourceVersionStatus GetResourceVersionStatus(ExternalResourceReference err) => ResourceVersionStatus.OutOfDate;
        public String GetInformationLink() => "http://www.autodesk.com";
        public void GetTypeSpecificServerOperations(ExternalResourceServerExtensions extensions) { }
    }
}
