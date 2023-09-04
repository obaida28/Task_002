namespace API.DTOs;
public class RentalCreateDto //: IValidatableObject
{
    [GuidNotEmpty(ErrorMessage = "Car Id is required.")]
    public Guid CarId { get; set; }
    [GuidNotEmpty(ErrorMessage = "Customer Id is required")]
    public Guid CustomerId { get; set; }
    public Guid? DriverId { get; set; }
    [RequiredDate(ErrorMessage = "Start Date is required")]
    [FutureDate(ErrorMessage = "Start Date must be a future date.")]
    [StartAndEnd("StartDate", "EndDate", ErrorMessage = "StartDate must be before EndDate.")]
    public DateTime StartDate { get; set; }
    [RequiredDate(ErrorMessage = "End Date is required")]
    [FutureDate(ErrorMessage = "End Date must be a future date.")]
    public DateTime EndDate { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Daily rate must be a non-negative value.")]
    [DefaultValue(1)]
    public int DailyRate { get; set; }

    // public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    // {
    //     bool HasEndDate = EndDate > DateTime.MinValue;
    //     if(HasEndDate && EndDate < StartDate) 
    //         yield return new ValidationResult("End Date must be greater than or equal to Start Date." ,
    //              new[] { "EndDate" , "StartDate" });
    // }
}