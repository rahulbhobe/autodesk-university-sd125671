using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;
using System.Windows.Forms;

namespace LinksDemo
{
    public class LinksMultiLanguageUIServer : IExternalResourceUIServer
    {
        public LinksMultiLanguageUIServer() { }
        public System.Guid GetServerId() => Application.UIServerId;
        public ExternalServiceId GetServiceId() => ExternalServices.BuiltInExternalServices.ExternalResourceUIService;
        public System.String GetName() => ServerName;
        public static string ServerName => "LinksMultiUI";
        public System.String GetVendorId() => "AU Demo";
        public System.String GetDescription() => "Provides CAD and Revit Links";

        public Guid GetDBServerId() => Application.DBServerId;

        public void HandleBrowseResult(ExternalResourceUIBrowseResultType resultType, string browsingItemPath)
        {

        }

        public void HandleLoadResourceResults(Document document, IList<ExternalResourceLoadData> loadData)
        {
            loadData.ToList().ForEach(data =>
            {
                if (data.LoadStatus == ExternalResourceLoadStatus.Success)
                    return;

                string file = data.GetExternalResourceReference().GetReferenceInformation()["File"];
                DBUtils.DBItem item = DBUtils.FindDBItem(file);
                if (item==null)
                {
                    MessageBox.Show($"The file {file} could not be found in the database.", ServerName);
                    data.ErrorsReported = true;
                }
                if (item.Private)
                {
                    string path = LinksMultiLanguageServer.GetPath(item);
                    MessageBox.Show($"You do not have permissions to open {path}.", ServerName);
                    data.ErrorsReported = true;
                }
            });
        }
    }
}
