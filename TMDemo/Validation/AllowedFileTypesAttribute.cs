using System.ComponentModel.DataAnnotations;
using System.IO;

public class AllowedFileTypesAttribute : ValidationAttribute
{
    private readonly string[] _allowedFileExtensions;

    public AllowedFileTypesAttribute(string[] allowedFileExtensions)
    {
        _allowedFileExtensions = allowedFileExtensions;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName)?.ToLower();
            if (!_allowedFileExtensions.Contains(extension))
            {
                return new ValidationResult($"File type not allowed. Allowed types: {string.Join(", ", _allowedFileExtensions)}");
            }
        }

        return ValidationResult.Success;
    }
}
