namespace Api.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalcuateAge(this DateOnly dateOfBirth)
        {
            var today=DateOnly.FromDateTime(DateTime.UtcNow);

             int age = today.Year - dateOfBirth.Year;

             if (dateOfBirth > today.AddYears(-age))
            {
                 age--;
            }
               return age;
        }
    }
}