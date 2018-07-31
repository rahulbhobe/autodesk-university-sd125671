using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;

namespace LinksDemo
{
    public class LinksMultiLanguageServer : IExternalResourceServer
    {
        public LinksMultiLanguageServer() { }
        public System.Guid GetServerId() => Application.DBServerId;
        public ExternalServiceId GetServiceId() => ExternalServices.BuiltInExternalServices.ExternalResourceService;
        public string GetShortName() => ServerName;
        public System.String GetName() => ServerName;
        public static string ServerName => "LinksMulti";
        public System.String GetVendorId() => "AU Demo";
        public System.String GetDescription() => "Provides CAD and Revit Links";

        public bool SupportsExternalResourceType(ExternalResourceType resourceType)
        {
            return ((resourceType == ExternalResourceTypes.BuiltInExternalResourceTypes.CADLink) || (resourceType ==ExternalResourceTypes.BuiltInExternalResourceTypes.RevitLink));
        }

        public String GetInSessionPath(ExternalResourceReference resourceReference, String originalPath)
        {
            string file = resourceReference.GetReferenceInformation()["File"];
            DBUtils.DBItem dbItem = DBUtils.FindDBItem(file);
            return GetPath(dbItem);
        }

        public static String GetPath(DBUtils.DBItem item)
        {
            return ServerName + "://" + DBUtils.GetDBItemName(item, Application.CurrentLanguage);
        }

        public void LoadResource(Guid loadRequestId, ExternalResourceType resourceType,
           ExternalResourceReference resourceReference, ExternalResourceLoadContext loadContext,
           ExternalResourceLoadContent content)
        {
            LinkLoadContent linkLoadContent = (LinkLoadContent)content;

            string file = resourceReference.GetReferenceInformation()["File"];
            DBUtils.DBItem dbItem = DBUtils.FindDBItem(file);
            if ((dbItem == null) || (dbItem.Private))
            {
                content.LoadStatus = ExternalResourceLoadStatus.Failure;
                return;
            }

            ModelPath linksPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(Application.BaseFolder + file);
            linkLoadContent.SetLinkDataPath(linksPath);
            content.LoadStatus = ExternalResourceLoadStatus.Success;
            return;
        }

        public void SetupBrowserData(ExternalResourceBrowserData browserData)
        {
            string curPath = browserData.FolderPath;

            DBUtils.GetAllDBItems().ForEach(item =>
            {
                Dictionary<string, string> referenceInformation = new Dictionary<string, string>();
                referenceInformation["File"] = item.File;

                string toDisplay = DBUtils.GetDBItemName(item, Application.CurrentLanguage);
                if (!FileUtils.FileBelongsToFolder(curPath, toDisplay))
                    return;

                string []dirInfo = FileUtils.SplitRelativePath(curPath, toDisplay);
                if (dirInfo.Length == 1)
                    browserData.AddResource(dirInfo[0], "1", referenceInformation);
                else if (dirInfo.Length > 1)
                    if (!browserData.GetSubFolders().Contains(dirInfo[0]))
                        browserData.AddSubFolder(dirInfo[0]);
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
