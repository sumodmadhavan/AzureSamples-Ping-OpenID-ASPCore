---
services: PING Identity Management System
platforms: dotnet asp core 2.X
author: Sumod Madhavan
---

# Integrating PING Identity into an ASP.NET Core web app

This sample shows how to build a .NET MVC web app that uses OpenID Connect to sign-in users from a single Ping Identity  using the ASP.NET Core OpenID Connect middleware.

For more information on how the protocols work in this scenario and other scenarios, see [Authentication Ping](https://www.pingidentity.com/en/resources/client-library/articles/openid-connect.html).

## How to run this sample

This sample is for ASP.NET Core 2.X

To run this sample:
- Install .NET Core for Windows by following the instructions at [.NET and C# - Get Started in 10 Minutes](https://www.microsoft.com/net/core). In addition to developing on Windows, you can develop on [Linux](https://www.microsoft.com/net/core#linuxredhat), [Mac](https://www.microsoft.com/net/core#macos), or [Docker](https://www.microsoft.com/net/core#dockercmd).
- Configure Ping

### Step 1: Register the sample with your Ping Identity

1. Configure Application in PING with Implicit Flow enabled.

2. You will receive ClientID, Client Secret and Auhority URL which will pull the meta-data.

3. The Security encryption model need to be [RSA](https://docs.pingidentity.com/bundle/rsaik20_sm_rsaIntegrationKit/page/rsaik_c_RSASecurIDIntegrationKit.html)

4. Select Reply URL
   - **Name**: **XYZ**
   - **Reply URL**: `http://localhost:5000/signin-oidc`
  
5. PING accepts only single reply URL. I am not sure about this though.   


### Step 2: Run the sample

1. Build the solution and run it.

Make a request to the app. The app immediately attempts to authenticate you via PING. Sign in with the username and password of a user account that is in your PING SSO. 

## About The code

-- You can inject a ContextAccessor to access the httpContext.

```Dependency Injection

//Dependency Injection.
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>()

//Configure
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
//Update the claim to Name so that User.Identity.Name is updated in ASP CORE.
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

```

## Output : 

## REFERRENCE

- https://www.pingidentity.com/developer/en/index.html
- https://github.com/Azure-Samples/active-directory-dotnet-webapp-openidconnect-aspnetcore
