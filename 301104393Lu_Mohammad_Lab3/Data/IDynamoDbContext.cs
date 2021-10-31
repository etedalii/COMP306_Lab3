using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _301104393Lu_Mohammad_Lab3.Data
{
    public interface IDynamoDbContext<T> : IDisposable where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAllByUserId(string userId);
        Task<IEnumerable<T>> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetByTitleAsync(string title);
        Task<IEnumerable<T>> GetByRating(double rating);
        Task<IEnumerable<T>> GetByGenreAsync(string genre);
        Task SaveAsync(T item);
        Task DeleteAsync(T item);
        Task Update(T item);
    }
}