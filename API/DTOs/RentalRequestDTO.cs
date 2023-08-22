using System.ComponentModel.DataAnnotations;
using API.Helpers;
using Core.Entites;
namespace API.DTOs;
public class RentalRequestDTO : RequestDTO<Rental> //, IValidatableObject
{
    [DataType (DataType.Date)]
         [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime SearchDate { get; set; }
    //  public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    // {
    //     if(EndDate == DateTime.MinValue) 
    //         yield return new ValidationResult("End Date is required." , new[] { "EndDate" });
    // }
}