namespace API.Validation;
public class StartAndEndAttribute : ValidationAttribute
{
    private readonly string _property1;
    private readonly string _property2;

    public StartAndEndAttribute(string property1 , string property2)
    {
        _property1 = property1;
        _property2 = property2;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
       var property1 = validationContext.ObjectType.GetProperty(_property1);
       var property2 = validationContext.ObjectType.GetProperty(_property2);

        if (property1 != null && property2 != null)
        {
            var propertyValue1 = property1.GetValue(validationContext.ObjectInstance);
            var propertyValue2 = property2.GetValue(validationContext.ObjectInstance);

            // Your custom validation logic here
            if (propertyValue1 != null && propertyValue2 != null)
            {
                DateTime startDate = (DateTime)propertyValue1;
                DateTime endDate = (DateTime)propertyValue2;
                if(startDate != DateTime.MinValue && endDate != DateTime.MinValue && endDate < startDate)
                    return new ValidationResult(ErrorMessage);
            }
        }
        return ValidationResult.Success;
    }
}