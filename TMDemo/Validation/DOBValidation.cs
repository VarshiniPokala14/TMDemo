namespace TMDemo.Validation
{
    public class DOBValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;
            if (DateTime.TryParse(value.ToString(), out DateTime DOB))
            {
                return DOB < DateTime.Today;
            }
            return false;
        }
        public override string FormatErrorMessage(string name)
        {
            return $"{name} can't be Future date";
        }
    }
}
