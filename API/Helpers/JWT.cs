namespace API.Helpers;
public class JWT
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public double DurationInMinutes { get; set; }
    public JwtSecurityToken CreateJwtToken(
        ApplicationUser user , IList<Claim> userClaims ,IList<string> roles)
    {
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
        if(Key is null) return null;
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(DurationInMinutes),
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