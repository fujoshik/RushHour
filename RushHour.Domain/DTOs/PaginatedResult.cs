using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RushHour.Domain.DTOs
{
    public record PaginatedResult<T>(List<T> Result, int TotalCount)
    {
        public static PaginatedResult<T> Empty() => new(new List<T>(), 0);
    }
}
