using System;
using System.Linq;
using System.Collections.Generic;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;

namespace LinksDemo
{
    public class DBUtils
    {
        public class DBItem
        {
            public string File { get; set; } = "";
            public string English { get; set; } = "";
            public string German { get; set; } = "";
            public bool Private { get; set; } = false;
        }

        public static string GetDBItemName(DBItem item, string language)
        {
            if (language.Equals("English", StringComparison.OrdinalIgnoreCase))
                return item.English;
            if (language.Equals("German", StringComparison.OrdinalIgnoreCase))
                return item.German;

            throw new ArgumentException($"Invalid argument {nameof(language)}");
        }

        private static AmazonDynamoDBClient GetAwsClient()
        {
            CredentialProfile profile;
            AWSCredentials awsCredentials;
            var file = new NetSDKCredentialsFile();
            if (!file.TryGetProfile(Application.AwsProfile, out profile))
                return null;
            if (!AWSCredentialsFactory.TryGetAWSCredentials(profile, file, out awsCredentials))
                return null;

            AmazonDynamoDBConfig config = new AmazonDynamoDBConfig();
            config.RegionEndpoint = Application.AwsRegion;
            return new AmazonDynamoDBClient(awsCredentials, config);
        }

        internal static List<DBItem> GetAllDBItems()
        {
            ScanResponse response = GetAwsClient().Scan(new ScanRequest{TableName = Application.DynamoDBTable});
            return response.Items.Select(item => new DBItem
            {
                File = item["File"].S,
                English = item["English"].S,
                German = item["German"].S
            }).ToList();
        }

        internal static DBItem FindDBItem(string file)
        {
            var table = Table.LoadTable(GetAwsClient(), Application.DynamoDBTable);
            var item = table.GetItem(file).ToAttributeMap();
            return new DBItem
            {
                File = item["File"].S,
                English = item["English"].S,
                German = item["German"].S
            };
        }
    }
}
