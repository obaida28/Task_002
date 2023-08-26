namespace API.Validation;
public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is DateTime date)
            return date != DateTime.MinValue && date > DateTime.Now.Date;
        return false;
    }
}