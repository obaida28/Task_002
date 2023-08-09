using System.ComponentModel.DataAnnotations;

namespace API.DTOs;
public class DriverCreateDTO
{
    [Required(ErrorMessage = "Driver name is required.")]
    public string Name { get; set; }
}