namespace API.DTOs;
public class TokenRequestDTO
{
    [Required , StringLength(128) , EmailAddress]
    public string Email { get; set; }

    [Required , StringLength(256)]
    public string Password { get; set; }
}
