using Microsoft.EntityFrameworkCore;
using RushHour.Domain.DTOs;

namespace RushHour.Data.Extensions
{
    public static class QueryExtensions
    {
        public static async Task<PaginatedResult<T>> PaginateAsync<T>(this IQueryable<T> collection, int index, int pageSize)
        {
            var result = await collection.ConstructResult<T>(index, pageSize).ToListAsync();

            return result.PaginateResult<T>(index, pageSize);      
        }

        public static IQueryable<T> ConstructResult<T>(this IQueryable<T> collection, int index, int pageSize)
        {
            var skip = (index - 1) * pageSize;
            
            return collection.Skip(skip).Take(pageSize);
        }

        public static PaginatedResult<T> PaginateResult<T>(this List<T> result, int index, int pageSize)
        {
            var skip = (index - 1) * pageSize;
            
            int count = result.Count;
            
            if (count == 0 || count < skip)
            {
                return PaginatedResult<T>.Empty();
            }
            
            return new PaginatedResult<T>(result, count);
        }
	}
}
