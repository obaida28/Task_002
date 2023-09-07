using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Core.Interfaces;
public interface IUserRepository
{
    Task<IdentityResult> AddRefreshTokenToUser(ApplicationUser entity , RefreshToken refreshToken);
    Task<IdentityResult> AddRoleToUser(ApplicationUser entity , string role);
    Task<bool> CheckPasswordAsync(ApplicationUser user , string paswword);
    Task<IdentityResult> CreateAsync(ApplicationUser entity , string password);
    // void Delete(T entity);
    Task<ApplicationUser> GetByEmailAsync(string email);
    Task<ApplicationUser> GetByIdAsync(string id);
    Task<IList<Claim>> GetClaimsAsync(ApplicationUser entity);
    Task<IList<string>> GetRolesAsync(ApplicationUser entity);
    IQueryable<ApplicationUser> GetQueryable();
    Task<bool> IsExistEmail(string email);
    Task<bool> IsExistRole(string role);
    Task<bool> IsExistUserName(string userName);    
    Task<bool> IsInRoleAsync(ApplicationUser user , string role);
    Task<IdentityResult> UpdateAsync(ApplicationUser entity);
}