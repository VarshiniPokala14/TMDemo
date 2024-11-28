using System.ComponentModel.DataAnnotations;

namespace TMDemo.Validation
{
    public class FutureOrTodayDate : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if(value == null) return false;
            if(DateTime.TryParse(value.ToString(),out DateTime startDate))
            {
                return startDate >= DateTime.Today;
            }
            return false;
        }
        public override string FormatErrorMessage(string name)
        {
            return $"{name} can't be past date"; 
        }
    }
}
