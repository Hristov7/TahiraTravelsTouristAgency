using System.ComponentModel.DataAnnotations;

namespace Models.CustomAttributes
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value is DateTime date && date > DateTime.UtcNow;
        }
    }
}
