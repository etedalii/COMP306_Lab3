using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _301104393Lu_Lab3.Data
{
    public class DynamoDbContext<T> : DynamoDBContext, IDynamoDbContext<T> where T : class
    {
        private DynamoDBOperationConfig _config;

        public DynamoDbContext(IAmazonDynamoDB client, string tableName)
            : base(client)
        {
            _config = new DynamoDBOperationConfig()
            {
                OverrideTableName = tableName
            };
        }

        public async Task<IEnumerable<T>> GetByIdAsync(string id)
        {
            var scanConditions = new List<ScanCondition>();
            scanConditions.Add(new ScanCondition("MovieId", ScanOperator.Equal, id));
            var res = await base.ScanAsync<T>(scanConditions).GetRemainingAsync();
            return res;
        }

        public async Task<IEnumerable<T>> GetByTitleAsync(string title)
        {
            var scanConditions = new List<ScanCondition>();
            scanConditions.Add(new ScanCondition("Title", ScanOperator.Equal, title));
            var res = await base.ScanAsync<T>(scanConditions).GetRemainingAsync();
            return res;
        }

        public async Task SaveAsync(T item)
        {
            await base.SaveAsync(item);
        }

        public async Task DeleteAsync(T item)
        {
            await base.DeleteAsync(item);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            var conditions = new List<ScanCondition>();
            var res = await base.ScanAsync<T>(conditions).GetRemainingAsync();
            return res;
        }

        public async Task Update(T item)
        {
            base.DeleteAsync(item).GetAwaiter().GetResult();
            await base.SaveAsync(item);
        }

        public async Task<IEnumerable<T>> GetByRating(int rating)
        {
            var scanConditions = new List<ScanCondition>();
            scanConditions.Add(new ScanCondition("Rate", ScanOperator.GreaterThanOrEqual, rating));
            var res = await base.ScanAsync<T>(scanConditions).GetRemainingAsync();
            return res;
        }
    }
}
