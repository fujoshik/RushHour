using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RushHour.API.Extensions
{
    public static class ControllerExtensions
    {
        public static Guid GetRequesterId(this ControllerBase controller)
        {
            return Guid.Parse(controller.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
        }
    }
}
