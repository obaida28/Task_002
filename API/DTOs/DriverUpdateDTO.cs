namespace API.DTOs;
public class DriverUpdateDTO
{
    [GuidNotEmpty(ErrorMessage = "Id is required")]
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Driver name is required.")]
    public string Name { get; set; }
}