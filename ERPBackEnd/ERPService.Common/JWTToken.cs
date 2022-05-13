using ERPService.Common.Shared;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ERPService.Common
{
    public static class JWTToken
    {
              
            public static string GenerateToken(UserContext userInfo)
            {
                byte[] key = Encoding.ASCII.GetBytes(ERPSettings.JWTSecretKey);
                SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
                SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                      new Claim(ClaimTypes.Name, userInfo.UserName),
                      new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(userInfo))
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(int.Parse(ERPSettings.JWTTokenExpiry)),
                    SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature),
                    Issuer = ConfigurationManager.AppSettings["TokenIssuer"].ToString()
                };

                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
                return handler.WriteToken(token);
            }
            public static ClaimsPrincipal GetPrincipal(string token)
            {
                try
                {
                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                    JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                    if (jwtToken == null)
                        return null;
                    byte[] key = Encoding.ASCII.GetBytes(ERPSettings.JWTSecretKey);
                    TokenValidationParameters parameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        RequireExpirationTime = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                    SecurityToken securityToken;
                    ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                          parameters, out securityToken);
                    return principal;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            public static UserContext ValidateToken(string token)
            {
                UserContext userInfo = null;
                ClaimsPrincipal principal = GetPrincipal(token);
                if (principal == null)
                    return null;
                ClaimsIdentity identity = null;
                try
                {
                    identity = (ClaimsIdentity)principal.Identity;
                }
                catch (NullReferenceException)
                {
                    return null;
                }
                Claim usernameClaim = identity.FindFirst(ClaimTypes.UserData);
                userInfo = JsonConvert.DeserializeObject<UserContext>(usernameClaim.Value);
                return userInfo;
            }

        
    }
}
