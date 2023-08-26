namespace API.DTOs;
public class CustomerUpdateDTO
{
    [GuidNotEmpty(ErrorMessage = "Id must have a non-default value.")]
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Customer name is required.")]
    public string Name { get; set; }
}