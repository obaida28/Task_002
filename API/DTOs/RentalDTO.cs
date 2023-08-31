namespace API.DTOs;
public class RentalDTO
{
    public Guid Id { get; set; }
    public Guid CarId { get; set; }
    public string CarNumber { get; set; }
    public string CarType { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public Guid DriverId { get; set; }
    public string? DriverName { get; set; }
    [DataType (DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime StartDate { get; set; }
    [DataType (DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime EndDate { get; set; }
    public int DailyRate { get; set; }
}