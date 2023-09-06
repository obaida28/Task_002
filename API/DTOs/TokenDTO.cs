namespace Core.Entites;
public class TokenDTO
{
    public string? Message { get; set; }
    public bool IsAuthenticated { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public List<string>? Roles { get; set; }
    public string? Token { get; set; }
    public DateTime? ExpiresOn { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public string? RefreshToken { get; set; }

    public DateTime RefreshTokenExpiration { get; set; }
}