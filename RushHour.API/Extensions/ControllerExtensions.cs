using RushHour.Domain.Exceptions;
using System.Security.Claims;

namespace RushHour.API.Extensions
{
    public static class ControllerExtensions
    {
        public static Guid GetRequesterId(this IHttpContextAccessor http)
        {
            Guid requesterId;

            var result = http.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if(result is null || !Guid.TryParse(result.Value, out requesterId))
            {
                throw new IncorrectAccountIdException(nameof(result));
            }

            return requesterId;
        }
    }
}
