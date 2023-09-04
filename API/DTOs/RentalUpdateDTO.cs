namespace API.DTOs;
public class RentalUpdateDTO //: IValidatableObject
{
    [GuidNotEmpty(ErrorMessage = "Id is required")]
    public Guid Id { get; set; }
    [GuidNotEmpty(ErrorMessage = "Car Id  is required")]
    public Guid CarId { get; set; }
    [GuidNotEmpty(ErrorMessage = "Customer Id  is required")]
    public Guid CustomerId { get; set; }
    public Guid? DriverId { get; set; }
    [RequiredDate(ErrorMessage = "Start Date is required")]
    [FutureDate(ErrorMessage = "Start Date must be a future date.")]
    [StartAndEnd("StartDate", "EndDate", ErrorMessage = "StartDate must be before EndDate.")]
    public DateTime StartDate { get; set; }
    [RequiredDate(ErrorMessage = "End Date is required")]
    [FutureDate(ErrorMessage = "End Date must be a future date.")]
    public DateTime EndDate { get; set; }
    [Range(1 , int.MaxValue, ErrorMessage = "Daily rate must be a non-negative value.")]
    [DefaultValue(1)]
    public int DailyRate { get; set; }

    // public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    // {
    //     // bool HasEndDate = EndDate > DateTime.MinValue;
    //     // bool HasStartDate = StartDate > DateTime.MinValue;
    //     // if(!HasEndDate) 
    //     //     yield return new ValidationResult("End Date is required." , new[] { "EndDate" });
    //     // if(!HasStartDate) 
    //     //     yield return new ValidationResult("Start Date is required.." , new[] { "StartDate" });
    //     // if(HasEndDate && EndDate < StartDate) 
    //     //     yield return new ValidationResult("End Date must be greater than or equal to Start Date." ,
    //     //          new[] { "EndDate" , "StartDate" });
    //     // if(HasStartDate && StartDate < DateTime.Now.Date)
    //     //     yield return new ValidationResult("Start Date must be a future date." , new[] { "StartDate" });
    //     // if(HasEndDate && EndDate < DateTime.Now.Date) 
    //     //     yield return new ValidationResult("End Date must be a future date." , new[] { "EndDate" });
    //     // if(CarId == Guid.Empty)
    //     //      yield return new ValidationResult("Car id is required", new[] { "CarId" });
    //     // if(CustomerId == Guid.Empty)
    //     //      yield return new ValidationResult("Customer id is required", new[] { "CustomerId" });
    //     // if(Id == Guid.Empty)
    //     //      yield return new ValidationResult("Id is required", new[] { "Id" });
    //     //  if(DailyRate <= 0)
    //     //      yield return new ValidationResult("Daily rate must be a non-negative value and bigger than zero.", 
    //     //     new[] { "DailyRate" });
    // }
}