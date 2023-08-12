using System.ComponentModel.DataAnnotations;

namespace API.DTOs;
public class CarCreateDto
{
    [Required(ErrorMessage = "Car number is required.")]
    public string Number { get; set; }

    [Required(ErrorMessage = "Type is required.")]
    public string Type { get; set; }
    [Required(ErrorMessage = "EngineCapacity is required.")]

    [Range(1, double.MaxValue, ErrorMessage = "Engine capacity must be a non-negative value.")]
    public decimal EngineCapacity { get; set; }

    [Required(ErrorMessage = "Color is required.")]
    public string Color { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Daily rate must be a non-negative value.")]
    public int DailyRate { get; set; }
}