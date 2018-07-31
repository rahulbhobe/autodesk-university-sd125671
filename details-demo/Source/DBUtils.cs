using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;

namespace DetailsDemo
{
    public class DBUtils
    {
        public class DBItem
        {
            public string File { get; set; } = "";
            public string Path { get; set; } = "";
        }

        internal static List<DBItem> GetAllDBItems()
        {
            return Directory.GetFiles(Application.BaseFolder, "*.dwg", SearchOption.AllDirectories).Select(path => new DBItem
            {
                File = Path.GetFileName(path),
                Path = path
            }).ToList();
        }

        internal static DBItem FindDBItem(string file)
        {
            return GetAllDBItems().Where(item => item.File == file).Single();
        }
    }
}
