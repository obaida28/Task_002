namespace Infrastructure.Services;
public interface IAuthService
{
    Task<TokenDTO> RegisterAsync(UserCreateDTO model);
    Task<TokenDTO> GetTokenAsync(TokenRequestDTO model);
    Task<string> AddRoleAsync(UserCreateRoleDTO model);
    Task<TokenDTO> RefreshTokenAsync(string token);
    Task<bool> RevokeTokenAsync(string token);
}