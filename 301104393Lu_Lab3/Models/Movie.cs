using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;

namespace _301104393Lu_Lab3.Models
{
    [DynamoDBTable("Movies")]
    public class Movie
    {
        [DynamoDBHashKey]
        public string MovieId { get; set; }

        [DynamoDBRangeKey]
        public string Title { get; set; }

        public string Genre { get; set; }

        public List<string> Cast { get; set; }

        public List<string> Director { get; set; }

        public string FileName { get; set; }

        public double Rate { get; set; }

        public int Year { get; set; }
    }
}