using System.ComponentModel.DataAnnotations;

namespace API.DTOs;
public class RentalUpdateDTO : IValidatableObject
{
    public Guid Id { get; set; }
    public Guid CarId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? DriverId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DailyRate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if(EndDate == DateTime.MinValue) 
            yield return new ValidationResult("End Date is required." , new[] { "EndDate" });
        if(StartDate == DateTime.MinValue) 
            yield return new ValidationResult("Start Date is required.." , new[] { "StartDate" });
        if(EndDate < StartDate) 
            yield return new ValidationResult("End Date must be greater than or equal to Start Date." ,
                 new[] { "EndDate" , "StartDate" });
        if(EndDate > DateTime.MinValue && StartDate < DateTime.Now.Date)
            yield return new ValidationResult("Start Date must be a future date." , new[] { "StartDate" });
        if(EndDate > DateTime.MinValue && EndDate < DateTime.Now.Date)
            yield return new ValidationResult("End Date must be a future date." , new[] { "EndDate" });
        if(Id == Guid.Empty)
             yield return new ValidationResult("Id is required", new[] { "Id" });
        if(CustomerId == Guid.Empty)
             yield return new ValidationResult("Customer id is required", new[] { "CustomerId" });
        if(CarId == Guid.Empty)
             yield return new ValidationResult("Car id is required", new[] { "CarId" });
         if(DailyRate <= 0)
             yield return new ValidationResult("Daily rate must be a non-negative value and bigger than zero.", 
            new[] { "DailyRate" });
    }
}