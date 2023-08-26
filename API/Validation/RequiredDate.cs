namespace API.Validation;
public class RequiredDateAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is DateTime date)
            return date != DateTime.MinValue;
        return false;
    }
}