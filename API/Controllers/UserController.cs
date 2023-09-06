namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
[ApiValidationFilter]
public class UserController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly string refreshTokenName = "refToken";
    public UserController(IAuthService _authService)
    {
        this._authService =  _authService;
    }
    [HttpPost("Create")]
    public async Task<ApiResponse> CreateUserAsync(UserCreateDTO model)
    {
        var result = await _authService.RegisterAsync(model);
        if (!result.IsAuthenticated)
            return ApiResponse.BAD(result.Message);
        SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
        return ApiResponse.OK(result);
    }
    [HttpPost("Login")]
    public async Task<ApiResponse> LoginAsync(TokenRequestDTO model)
    {
        var result = await _authService.GetTokenAsync(model);
        if (!result.IsAuthenticated)
            return ApiResponse.BAD(result.Message);
        if(!string.IsNullOrEmpty(result.RefreshToken))
            SetRefreshTokenInCookie(result.RefreshToken , result.RefreshTokenExpiration);
        return ApiResponse.OK(result);
    }
    [HttpPost("CreateRole")]
    [Authorize(Roles = "Admin")]
    public async Task<ApiResponse> CreateRoleAsync(UserCreateRoleDTO model)
    {
        var result = await _authService.AddRoleAsync(model);
        if (!string.IsNullOrEmpty(result))
            return ApiResponse.BAD(result);
        return ApiResponse.OK(model);
    }
    
    [HttpGet("refreshToken")]
    public async Task<ApiResponse> RefreshToken()
    {
        var refreshToken = GetRefreshTokenInCookie();
        var result = await _authService.RefreshTokenAsync(refreshToken);
        if(!result.IsAuthenticated)
            return ApiResponse.BAD(result.Message);
        SetRefreshTokenInCookie(result.RefreshToken , result.RefreshTokenExpiration);
        return ApiResponse.OK(result);
    }
    [HttpPost("revokeToken")]
    public async Task<ApiResponse> RevokeToken(TokenRevokeDTO dto)
    {
        var refreshToken = dto.Token ?? GetRefreshTokenInCookie();
        if(string.IsNullOrEmpty(refreshToken)) 
            return ApiResponse.BAD("Token is required!");
        var result = await _authService.RevokeTokenAsync(refreshToken);
        if(!result)
            return ApiResponse.BAD("Token is invalid!");
        return ApiResponse.OK(result);
    }
    
    private void SetRefreshTokenInCookie(string refreshToken , DateTime expired)
    {
        var CookiesOption = new CookieOptions
        {
            HttpOnly = true,
            Expires = expired.ToLocalTime()
        };
        Response.Cookies.Append(refreshTokenName , refreshToken , CookiesOption);
    }
    private string? GetRefreshTokenInCookie()
    {
        return Request.Cookies[refreshTokenName];
    }
}