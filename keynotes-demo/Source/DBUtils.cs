using System.Collections.Generic;
using System.Linq;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.DynamoDBv2.Model;

namespace KeynotesDemo
{
    public class DBUtils
    {
        public class DBItem
        {
            public string Key { get; set; } = "";
            public string ParentKey { get; set; } = "";
            public string Text { get; set; } = "";
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

        internal static List<DBItem> GetAllDBItems(string tableName)
        {
            ScanResponse response = GetAwsClient().Scan(new ScanRequest { TableName = tableName });
            return response.Items.Select(item => new DBItem
            {
                Key = item["Key"].S,
                Text = item["Text"].S,
                ParentKey = item.ContainsKey("ParentKey") ? item["ParentKey"].S : ""
            }).ToList();
        }
    }
}
