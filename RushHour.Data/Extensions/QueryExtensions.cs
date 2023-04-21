using Microsoft.EntityFrameworkCore;
using RushHour.Domain.DTOs;

namespace RushHour.Data.Extensions
{
    public static class QueryExtensions
    {
        public static async Task<PaginatedResult<T>> PaginateAsync<T>(this IQueryable<T> collection, int index, int pageSize)
        {
            var count = await collection.CountAsync();
            var skip = (index - 1) * pageSize;
            if (count == 0 || count < skip)
            {
                return PaginatedResult<T>.Empty();
            }

            var result = await collection.Skip(skip).Take(pageSize).ToListAsync();

            return new PaginatedResult<T>(result, count);
        }
    }
}
