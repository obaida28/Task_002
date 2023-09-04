namespace API.DTOs;
public class CustomerUpdateDTO
{
    [GuidNotEmpty(ErrorMessage = "Id is required")]
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Customer name is required.")]
    public string Name { get; set; }
}