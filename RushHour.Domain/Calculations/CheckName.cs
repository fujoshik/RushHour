namespace RushHour.Domain.Calculations
{
    public static class CheckName
    {
        public static bool IsValidFullName(string name)
        {
            var result = name.Replace("-", "");
            result = result.Replace("'", "");

            return result.All(Char.IsLetter);
        }
    }
}
