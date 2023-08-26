namespace API.DTOs;
public class CustomerCreateDTO
{
    [Required(ErrorMessage = "Customer name is required.")]
    public string Name { get; set; }
}