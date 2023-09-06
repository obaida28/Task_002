using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
namespace Infrastructure.Services;
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JWT _jwt;
    protected readonly IMapper _mapper ;
    public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager
        ,IOptions<JWT> jwt , IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwt = jwt.Value;
        _mapper = mapper;
    }
    public async Task<TokenDTO> RegisterAsync(UserCreateDTO model)
    {
        if (await _userManager.FindByEmailAsync(model.Email) is not null)
            return new TokenDTO { Message = "Email is already registered!" };
        if (await _userManager.FindByNameAsync(model.Username) is not null)
            return new TokenDTO { Message = "Username is already registered!" };
        var user = _mapper.Map<ApplicationUser>(model);
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            var errors = string.Empty;
            foreach (var error in result.Errors) errors += $"{error.Description},";
            return new TokenDTO { Message = errors };
        }
        await _userManager.AddToRoleAsync(user, "User");
        var jwtSecurityToken = await CreateJwtToken(user);
        if(jwtSecurityToken is null) return new TokenDTO { Message = "You did not add JWT props in your appSettings file!" };
        var refreshToken = GenerateRefreshToken();
        user.RefreshTokens?.Add(refreshToken);
        await _userManager.UpdateAsync(user);
        return new TokenDTO
        {
            Email = user.Email,
            ExpiresOn = jwtSecurityToken.ValidTo,
            IsAuthenticated = true,
            Roles = new List<string> { "User" },
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            Username = user.UserName,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiration = refreshToken.ExpiresOn
        };
    }
    public async Task<TokenDTO> GetTokenAsync(TokenRequestDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null || ! await _userManager.CheckPasswordAsync(user , model.Password))
            return new TokenDTO { Message = "Email or Password is not correct!" };
        var jwtSecurityToken = await CreateJwtToken(user);
        if(jwtSecurityToken is null)
            return new TokenDTO { Message = "You did not add JWT props in your appSettings file!" };
        var Role = await _userManager.GetRolesAsync(user);
        RefreshToken refreshToken;
        if(user.RefreshTokens.Any(t => t.IsActive))
               refreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
        else
        {
            refreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);            
            var result = await _userManager.UpdateAsync(user);
            if(!result.Succeeded)
                return new TokenDTO { Message = string.Join(",", 
                    result.Errors.Select(o => o.Description).ToArray()) };
        
        }
        return new TokenDTO
        {
            Email = user.Email ,
            ExpiresOn = jwtSecurityToken.ValidTo,
            IsAuthenticated = true,
            Roles = Role.ToList(),
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            Username = user.UserName,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiration = refreshToken.ExpiresOn
        };
    }
    public async Task<string> AddRoleAsync(UserCreateRoleDTO model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if(user is null || !await _roleManager.RoleExistsAsync(model.Role))
            return "Invalid user Id or Role";
        if(await _userManager.IsInRoleAsync(user , model.Role))
            return "User already assigned to this role !";

        var result = await _userManager.AddToRoleAsync(user , model.Role);
        return !result.Succeeded ? "Something went wrong !" : string.Empty;
    }
    public async Task<TokenDTO> RefreshTokenAsync(string token)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(
            u => u.RefreshTokens.Any(t => t.Token == token));
        if(user is null)
            return new TokenDTO { Message = "Invalid Token"};
        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
        if(!refreshToken.IsActive)
            return new TokenDTO{ Message = "Inactive Token"};
        refreshToken.RevokedOn = DateTime.Now;
        var newRefreshToken = GenerateRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);
        await _userManager.UpdateAsync(user);
        var Role = await _userManager.GetRolesAsync(user);
        var new_jwt = await CreateJwtToken(user);
        return new TokenDTO
        {
            Email = user.Email,
            ExpiresOn = new_jwt.ValidTo,
            IsAuthenticated = true,
            Roles = Role.ToList(),
            Token = new JwtSecurityTokenHandler().WriteToken(new_jwt),
            Username = user.UserName,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiration = refreshToken.ExpiresOn
        };
    }
    public async Task<bool> RevokeTokenAsync(string token)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(
            u => u.RefreshTokens.Any(t => t.Token == token));
        if(user is null) return false;
        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
        if(!refreshToken.IsActive) return false;
        refreshToken.RevokedOn = DateTime.Now;
        await _userManager.UpdateAsync(user);
        return true;
    }
    
    private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        foreach (var role in roles)
            roleClaims.Add(new Claim("roles", role));
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("uid", user.Id.ToString())
        }.Union(userClaims).Union(roleClaims);
        if(_jwt.Key is null) return null;
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }
    public RefreshToken GenerateRefreshToken()
    {
        var random = new byte[32];
        using var generate = new RNGCryptoServiceProvider();
        generate.GetBytes(random);
        return new RefreshToken
        {
            Token = Convert.ToBase64String(random) ,
            CreatedOn = DateTime.Now,
            ExpiresOn = DateTime.Now.AddDays(10),
        };
    }
}