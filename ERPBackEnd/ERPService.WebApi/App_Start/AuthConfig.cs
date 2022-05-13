using Microsoft.Owin.Security.Jwt;
using Owin;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ERPService.Common;
using System.Web.Http;
using System;
using System.Configuration;

namespace ERPService.WebApi
{
    public static class AuthConfig
    {
        public static void UseAuthentication(this IAppBuilder app)
        {
            app.UseJwtBearerAuthentication(
                    new JwtBearerAuthenticationOptions()
                    {
                         AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                         TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                         {
                             ValidateIssuer = false,
                             ValidateAudience = false,
                             ValidateIssuerSigningKey = true,
                             ValidIssuer = ConfigurationManager.AppSettings["TokenIssuer"].ToString(),
                             ValidAudience = "",
                             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ERPSettings.JWTSecretKey))
                         }
                    }
                );
        }

    }
}