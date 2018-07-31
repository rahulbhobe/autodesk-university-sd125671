using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;

namespace KeynotesDemo
{
    public class DynamoDBKeynotesServer : IExternalResourceServer
    {
        public DynamoDBKeynotesServer() { }
        public System.Guid GetServerId() => Application.DBServerId;
        public ExternalServiceId GetServiceId() => ExternalServices.BuiltInExternalServices.ExternalResourceService;
        public string GetShortName() => ServerName;
        public System.String GetName() => ServerName;
        public static string ServerName => "DynamoDBKeynotes";
        public System.String GetVendorId() => "AU Demo";
        public System.String GetDescription() => "Provides Keynotes from DynamoDB";

        public bool SupportsExternalResourceType(ExternalResourceType resourceType)
        {
            return (resourceType == ExternalResourceTypes.BuiltInExternalResourceTypes.KeynoteTable);
        }

        public String GetInSessionPath(ExternalResourceReference resourceReference, String originalPath)
        {
            return ServerName + "://" + resourceReference.GetReferenceInformation()["TableName"] + ".txt";
        }

        public void LoadResource(Guid loadRequestId, ExternalResourceType resourceType,
           ExternalResourceReference resourceReference, ExternalResourceLoadContext loadContext,
           ExternalResourceLoadContent content)
        {
            KeyBasedTreeEntriesLoadContent kdrlc = (KeyBasedTreeEntriesLoadContent)content;
            string tableName = resourceReference.GetReferenceInformation()["TableName"];

            DBUtils.GetAllDBItems(tableName).ForEach(item =>
            {
                kdrlc.AddEntry(new KeynoteEntry(item.Key, item.ParentKey, item.Text));
            });

            kdrlc.BuildEntries();
            kdrlc.LoadStatus = ExternalResourceLoadStatus.Success;
            return;
        }

        public void SetupBrowserData(ExternalResourceBrowserData browserData)
        {
            Application.DynamoDBTables.ForEach(table =>
            {
                IDictionary<String, String> referenceInformation = new Dictionary<String, String>();
                referenceInformation["TableName"] = table;
                browserData.AddResource(table + ".txt", "1", referenceInformation);
            });
        }

        public bool AreSameResources(IDictionary<string, string> refInfo1, IDictionary<string, string> refInfo2)
        {
            if ((refInfo1 == null) != (refInfo2 == null))
                return false;
            if ((refInfo1 == null) == (refInfo2 == null))
                return true;

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
