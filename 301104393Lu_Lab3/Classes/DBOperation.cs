using _301104393Lu_Lab3.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _301104393Lu_Lab3.Classes
{
    public class DBOperation
    {
        #region Variables and Property

        IConfiguration configuration;
        AmazonDynamoDBClient client;
        string tableName;
        Table userTable;
        public Table UserTable { get { return userTable; } }

        public static string CurrentUser { get; set; }
        public static string CurrentUserBucketName { get; set; }

        #endregion

        #region Constructor

        public DBOperation(IConfiguration configuration, string tableName)
        {
            this.configuration = configuration;
            this.tableName = tableName;
            var accessKeyID = configuration["AccesskeyID"];
            var secretKey = configuration["Secretaccesskey"];
            var credentials = new BasicAWSCredentials(accessKeyID, secretKey);
            client = new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.USEast1);
            Table.TryLoadTable(client, tableName, out userTable);
        }

        #endregion

        #region Method

        public async Task<bool> CreateTable()
        {
            CreateTableRequest createTable = new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                     new AttributeDefinition
                    {
                        AttributeName= "MovieId",
                        AttributeType= "S"
                    },
                      new AttributeDefinition
                    {
                        AttributeName= "Title",
                        AttributeType= "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName="MovieId",
                        KeyType="HASH"
                    },
                      new KeySchemaElement
                    {
                        AttributeName="Title",
                        KeyType="RANGE"
                    }
                },
                BillingMode = BillingMode.PROVISIONED,
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 2,
                    WriteCapacityUnits = 1
                }
            };

            var response = await client.CreateTableAsync(createTable);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                userTable = Table.LoadTable(client, tableName, DynamoDBEntryConversion.V2);
                return true;
            }
            else
                return false;
        }

        public async Task<bool> CreateCommentsTable()
        {
            CreateTableRequest createTable = new CreateTableRequest
            {
                TableName = "Comments",
                AttributeDefinitions = new List<AttributeDefinition>
                {
                     new AttributeDefinition
                    {
                        AttributeName= "MovieId",
                        AttributeType= "S"
                    },
                      new AttributeDefinition
                    {
                        AttributeName= "Rate",
                        AttributeType= "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName="MovieId",
                        KeyType="HASH"
                    },
                      new KeySchemaElement
                    {
                        AttributeName="Rate",
                        KeyType="RANGE"
                    }
                },
                BillingMode = BillingMode.PROVISIONED,
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 2,
                    WriteCapacityUnits = 1
                }
            };

            var response = await client.CreateTableAsync(createTable);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                userTable = Table.LoadTable(client, tableName, DynamoDBEntryConversion.V2);
                return true;
            }
            else
                return false;
        }

        public async Task InsertBookshelfItem()
        {
            //if (GetBookshelfItemCount().GetAwaiter().GetResult() == 0)
            //{
            //    foreach (var item in userBooks)
            //    {
            //        await _context.SaveAsync(item);
            //    }
            //}
        }

        public async Task<int> GetBookshelfItemCount()
        {
            var conditions = new List<ScanCondition>();
            // var allDocs = await _context.ScanAsync<Bookshelf>(conditions).GetRemainingAsync();
            // return allDocs.Count;
            return 0;
        }

        public async Task<List<Movie>> GetBookByUserId(string username)
        {
            var scanConditions = new List<ScanCondition>();
            scanConditions.Add(new ScanCondition("UserId", ScanOperator.Equal, username));
            //var bookshelfs = null;// await _context.ScanAsync<Bookshelf>(scanConditions).GetRemainingAsync();

            //var sortedList = bookshelfs.OrderByDescending(_ => Convert.ToDateTime(_.LastSeenDate)).ToList();
            return null;
        }

        public async void UpdateBook(string isbn, int page)
        {
            var scanConditions = new List<ScanCondition>();
            scanConditions.Add(new ScanCondition("ISBN", ScanOperator.Equal, isbn));
            //var book = _context.ScanAsync<Bookshelf>(scanConditions).GetRemainingAsync().GetAwaiter().GetResult().FirstOrDefault();

            //book.LastSeenPage = page;
            //book.LastSeenDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            //_context.SaveAsync(book);
        }

        #endregion
    }
}