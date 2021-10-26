using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _301104393Lu_Lab3.Data
{
    public interface IDynamoDbContext<T> : IDisposable where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetByCommentIdAsync(string id);

        Task<IEnumerable<T>> GetByTitleAsync(string title);
        Task<IEnumerable<T>> GetByRating(int rating);
        Task SaveAsync(T item);
        Task DeleteAsync(T item);

        Task Update(T item);
    }
}
