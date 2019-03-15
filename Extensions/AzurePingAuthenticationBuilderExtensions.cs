//Author : Sumod Madhavan
//Org : Microsoft.
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Microsoft.AspNetCore.Authentication
{
    public static class AzurePingAuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddAzurePing(this AuthenticationBuilder builder)
            => builder.AddAzurePing(_ => { });

        public static AuthenticationBuilder AddAzurePing(this AuthenticationBuilder builder, Action<AzurePingOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, ConfigureAzureOptions>();
            builder.AddOpenIdConnect();
            return builder;
        }

        private class ConfigureAzureOptions : IConfigureNamedOptions<OpenIdConnectOptions>
        {
            private readonly AzurePingOptions _azureOptions;

            public ConfigureAzureOptions(IOptions<AzurePingOptions> azureOptions)
            {
                _azureOptions = azureOptions.Value;
            }

            public void Configure(string name, OpenIdConnectOptions options)
            {
                options.Authority = _azureOptions.Authority;
                options.ClientId = _azureOptions.ClientId;
                options.ClientSecret = _azureOptions.ClientSecret;
                options.UseTokenLifetime = true;
                options.CallbackPath = _azureOptions.CallbackPath;
                options.RequireHttpsMetadata = false;
                options.SaveTokens = true;
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.TokenValidationParameters.ValidateIssuer = true;
                options.TokenValidationParameters.ValidateAudience = true;
                options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                options.Scope.Add("openid");
                options.Scope.Add("first_name");
                options.Scope.Add("last_name");
                options.Scope.Add("email");
                options.Events.OnTicketReceived = ctx =>
                {
                    ctx.Principal = TransformClaims(ctx.Principal);
                    return Task.CompletedTask;
                };
            }
            private ClaimsPrincipal TransformClaims(ClaimsPrincipal claimsPrincipal)
            {
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)claimsPrincipal.Identity;
                ClaimsPrincipal newClaims = null;
                if (claimsIdentity != null)
                {
                    string fullName = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
                    string email = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Email).Value;
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, fullName));
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
                    newClaims = new ClaimsPrincipal(claimsIdentity);
                }
                return newClaims;
            }
            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }
        }
    }
}
