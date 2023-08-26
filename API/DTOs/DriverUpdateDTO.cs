using System.ComponentModel.DataAnnotations;
using API.Validation;

namespace API.DTOs;
public class DriverUpdateDTO
{
    [GuidNotEmpty(ErrorMessage = "Id must have a non-default value.")]
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Driver name is required.")]
    public string Name { get; set; }
}