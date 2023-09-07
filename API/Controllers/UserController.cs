using API.Extensions;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
[ApiValidationFilter]
public class UserController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _map;
    private readonly JWT _jwt;
    private readonly string refreshTokenName = "refToken";
    public UserController(IUnitOfWork unitOfWork , IMapper map , IOptions<JWT> jwt )
    {
        _jwt = jwt.Value;
        _unitOfWork = unitOfWork;
        _map = map;
    }
    [HttpPost("Create")]
    public async Task<ApiResponse> CreateAsync(UserCreateDTO model)
    {
        if(await _unitOfWork.Users.IsExistEmail(model.Email))
            return ApiResponse.BAD("Email is already registered!");
        if(await _unitOfWork.Users.IsExistUserName(model.Username))
            return ApiResponse.BAD("Username is already registered!");
        var user = _map.Map<ApplicationUser>(model);
        var resultCreate = await _unitOfWork.Users.CreateAsync(user,model.Password);
        if (!resultCreate.Succeeded)
            return ApiResponse.BAD(resultCreate.GetErrors());
        var resultAddRole = await _unitOfWork.Users.AddRoleToUser(user , "user");
        if (!resultAddRole.Succeeded)
            return ApiResponse.BAD(resultAddRole.GetErrors());
        var claims = await _unitOfWork.Users.GetClaimsAsync(user);
        var roles = await _unitOfWork.Users.GetRolesAsync(user);
        var jwtSecurityToken = _jwt.CreateJwtToken(user,claims,roles);
        if(jwtSecurityToken is null)
            return ApiResponse.BAD("You did not add JWT props in your appSettings file!");
        var refreshToken = _jwt.GenerateRefreshToken();
        var resultAddRefreshTokenToUser = await _unitOfWork.Users.AddRefreshTokenToUser(user , refreshToken);
        if (!resultAddRefreshTokenToUser.Succeeded)
            return ApiResponse.BAD(resultAddRefreshTokenToUser.GetErrors());
        var result = new TokenDTO
        {
            Email = user.Email,
            ExpiresOn = jwtSecurityToken.ValidTo,
            // IsAuthenticated = true,
            Roles = new List<string> { "User" },
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            Username = user.UserName,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiration = refreshToken.ExpiresOn
        };
        SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
        return ApiResponse.OK(result);
    }
    [HttpPost("Login")]
    public async Task<ApiResponse> LoginAsync(TokenRequestDTO model)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(model.Email);
        var checkPassword = await _unitOfWork.Users.CheckPasswordAsync(user , model.Password);
        if (user is null)
            return ApiResponse.NOT("Email is not correct!");
        if (!checkPassword)
            return ApiResponse.BAD("Password is not correct!");
        var claims = await _unitOfWork.Users.GetClaimsAsync(user);
        var roles = await _unitOfWork.Users.GetRolesAsync(user);
        var jwtSecurityToken = _jwt.CreateJwtToken(user,claims,roles);
        if(jwtSecurityToken is null)
            return ApiResponse.BAD("token error!");
        RefreshToken refreshToken;
        if(user.RefreshTokens.Any(t => t.IsActive))
               refreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
        else
        {
            refreshToken = _jwt.GenerateRefreshToken();
            var resultAddRefreshTokenToUser = await _unitOfWork.Users.AddRefreshTokenToUser(user , refreshToken);
            if (!resultAddRefreshTokenToUser.Succeeded)
                return ApiResponse.BAD(resultAddRefreshTokenToUser.GetErrors());
        }
        var result = new TokenDTO
        {
            Email = user.Email ,
            ExpiresOn = jwtSecurityToken.ValidTo,
            // IsAuthenticated = true,
            Roles = roles.ToList(),
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            Username = user.UserName,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiration = refreshToken.ExpiresOn
        };
        if(!string.IsNullOrEmpty(result.RefreshToken))
            SetRefreshTokenInCookie(result.RefreshToken , result.RefreshTokenExpiration);
        return ApiResponse.OK(result);
    }
    [HttpPost("CreateRole")]
    [Authorize(Roles = "Admin")]
    public async Task<ApiResponse> CreateRoleAsync(UserCreateRoleDTO model)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(model.UserId);
        var isExist = await _unitOfWork.Users.IsExistRole(model.Role);
        if(user is null)
            return ApiResponse.NOT("Invalid user Id");
        if(isExist)
            return ApiResponse.BAD("Invalid role");
        if(await _unitOfWork.Users.IsInRoleAsync(user , model.Role))
            return ApiResponse.BAD("User already assigned to this role !");
        var result = await _unitOfWork.Users.AddRoleToUser(user , model.Role);
        return !result.Succeeded ? ApiResponse.BAD(result.GetErrors()) : ApiResponse.OK(model);
    }
    [HttpGet("refreshToken")]
    public async Task<ApiResponse> RefreshToken()
    {
        var cookieToken = GetRefreshTokenInCookie();
        var user = _unitOfWork.Users.GetQueryable().SingleOrDefault(
            u => u.RefreshTokens.Any(t => t.Token == cookieToken));
        if(user is null)
            return ApiResponse.NOT("Invalid Token");
        var refreshToken = user.RefreshTokens.Single(t => t.Token == cookieToken);
        if(!refreshToken.IsActive)
            return ApiResponse.BAD("Inactive Token");
        refreshToken.RevokedOn = DateTime.Now;
        var newRefreshToken = _jwt.GenerateRefreshToken();
        var resultAddRefreshTokenToUser = await _unitOfWork.Users.AddRefreshTokenToUser(user , newRefreshToken);
        if (!resultAddRefreshTokenToUser.Succeeded)
            return ApiResponse.BAD(resultAddRefreshTokenToUser.GetErrors());
        var roles = await _unitOfWork.Users.GetRolesAsync(user);
        var claims = await _unitOfWork.Users.GetClaimsAsync(user);
        var new_jwt = _jwt.CreateJwtToken(user,claims,roles);
        var result = new TokenDTO
        {
            Email = user.Email,
            ExpiresOn = new_jwt.ValidTo,
            // IsAuthenticated = true,
            Roles = roles.ToList(),
            Token = new JwtSecurityTokenHandler().WriteToken(new_jwt),
            Username = user.UserName,
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiration = newRefreshToken.ExpiresOn
        };
        SetRefreshTokenInCookie(result.RefreshToken , result.RefreshTokenExpiration);
        return ApiResponse.OK(result);
    }
    [HttpPost("revokeToken")]
    public async Task<ApiResponse> RevokeToken(TokenRevokeDTO dto)
    {
        var cookieToken = dto.Token ?? GetRefreshTokenInCookie();
        if(string.IsNullOrEmpty(cookieToken)) 
            return ApiResponse.BAD("Token is required!");    
        var user = _unitOfWork.Users.GetQueryable().SingleOrDefault(
            u => u.RefreshTokens.Any(t => t.Token == cookieToken));
        if(user is null)
            return ApiResponse.NOT("Invalid Token");
        var refreshToken = user.RefreshTokens.Single(t => t.Token == cookieToken);
        if(!refreshToken.IsActive)
            return ApiResponse.BAD("Token is invalid!");
        refreshToken.RevokedOn = DateTime.Now;
        var resultUpdate = await _unitOfWork.Users.UpdateAsync(user);
        return !resultUpdate.Succeeded ? ApiResponse.BAD(resultUpdate.GetErrors()) : ApiResponse.OK();
    }
    //Cookies
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