namespace API.Validation;
public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {  
        if (value is DateTime date)
        {
            var hasValue = date != DateTime.MinValue;
            if(!hasValue)
                return true;
            return date > DateTime.Now.Date;
        }
        return false;
    }
}