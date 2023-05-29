using System.ComponentModel.DataAnnotations;

namespace RushHour.Domain.CustomAttributes
{
    public class PasswordAttribute : RegularExpressionAttribute
    {
        public PasswordAttribute()
            : base(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[+=_^@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
        { }
    }
}
