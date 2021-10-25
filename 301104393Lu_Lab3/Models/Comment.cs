using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _301104393Lu_Lab3.Models
{
    [DynamoDBTable("Comments")]
    public class Comment
    {
        [DynamoDBHashKey]
        public string MovieId { get; set; }

        [DynamoDBRangeKey]
        public int Rate { get; set; }

        public string Username { get; set; }

        public string Statement { get; set; }

        public string StatementDate { get; set; }
    }
}
