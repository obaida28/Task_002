using System.ComponentModel.DataAnnotations;

namespace API.DTOs;
public class CustomerUpdateDTO
{
    [Required(ErrorMessage = "Customer Id name is required.")]
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Customer name is required.")]
    public string Name { get; set; }
}