namespace API.Validation;
public class GuidNotEmptyAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is Guid guidValue)
            return guidValue != Guid.Empty;
        return false;
    }
}