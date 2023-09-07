
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data;
public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IdentityResult> AddRefreshTokenToUser(ApplicationUser entity, RefreshToken refreshToken)
    {
        entity.RefreshTokens?.Add(refreshToken);
        var result = await _userManager.UpdateAsync(entity);
        return result;
    }
    public async Task<IdentityResult> AddRoleToUser(ApplicationUser entity , string role)
    {
        var result = await _userManager.AddToRoleAsync(entity, "User");
        return result;
    }
    public async Task<bool> CheckPasswordAsync(ApplicationUser user , string paswword)
    {
        var result = await _userManager.CheckPasswordAsync(user,paswword);
        return result;
    }
    public async Task<IdentityResult> CreateAsync(ApplicationUser entity , string password)
    {
        var result = await _userManager.CreateAsync(entity, password);
        return result;
    }
    public async Task<ApplicationUser> GetByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user;
    }
    public async Task<ApplicationUser> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        return user;
    }   
    public async Task<IList<Claim>> GetClaimsAsync(ApplicationUser entity)
    {
        var userClaims = await _userManager.GetClaimsAsync(entity);
        return userClaims;
    }
    public async Task<IList<string>> GetRolesAsync(ApplicationUser entity)
    {
        var userRoles = await _userManager.GetRolesAsync(entity);
        return userRoles;
    }
    public IQueryable<ApplicationUser> GetQueryable()
    {
        var result = _userManager.Users.AsQueryable();
        return result;
    }
    public async Task<bool> IsExistEmail(string email)
    {
        var result = await _userManager.FindByEmailAsync(email);
        return result is not null;
    }
    public async Task<bool> IsExistRole(string role)
    {
        var result = await _roleManager.RoleExistsAsync(role);
        return result;
    }
    public async Task<bool> IsExistUserName(string userName)
    {
        var result = await _userManager.FindByNameAsync(userName);
        return result is not null;
    }
    public async Task<bool> IsInRoleAsync(ApplicationUser user , string role)
    {
        var result = await _userManager.IsInRoleAsync(user,role);
        return result;
    }
    public async Task<IdentityResult> UpdateAsync(ApplicationUser entity)
    {
        var result = await _userManager.UpdateAsync(entity);
        return result;
        //await _context.SaveChangesAsync();
    }
}