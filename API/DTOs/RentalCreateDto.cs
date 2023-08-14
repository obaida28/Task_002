using System.ComponentModel.DataAnnotations;

namespace API.DTOs;
public class RentalCreateDto
{
    [Required(ErrorMessage = "Car id is required.")]
    public Guid CarId { get; set; }

    [Required(ErrorMessage = "Customer id is required.")]
    public Guid CustomerId { get; set; }
    public Guid? DriverId { get; set; }
    [Required(ErrorMessage = "Start Date id is required.")]
    public DateTime StartDate { get; set; }
    [Required(ErrorMessage = "End Date id is required.")]
    public DateTime EndDate { get; set; }
    public bool IsValidPeriod
    {
        get
        {
            return EndDate >= StartDate;
        }
    }
    public bool InFutureDate
    {
        get
        {
            return StartDate >= DateTime.Now && EndDate >= DateTime.Now;
        }
    }
    [Range(0, int.MaxValue, ErrorMessage = "Daily rate must be a non-negative value.")]
    public int DailyRate { get; set; }
}