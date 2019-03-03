using System;
using System.Collections.Generic;
using System.Security.Claims;
using AzureFunctionsV2.HttpExtensions.Authorization;
using HttpExtensionsExamples.Startup;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(Startup), "MyStartup")]

namespace HttpExtensionsExamples.Startup
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            // Configuring HttpExtensions authentication methods.

            builder.Services.Configure<HttpAuthenticationOptions>(options =>
            {
                // API key auth.
                options.ApiKeyAuthentication = new ApiKeyAuthenticationParameters()
                {
                    ApiKeyVerifier = async (s, request) => s == "key" ? true : false,
                    HeaderName = "x-apikey"
                };
                // Basic auth
                options.BasicAuthentication = new BasicAuthenticationParameters()
                {
                    ValidCredentials = new Dictionary<string, string>() { { "user", "pass" } }
                };
                // JWT based auth, OpenIdConnect
                options.JwtAuthentication = new JwtAuthenticationParameters()
                {
                    TokenValidationParameters = new OpenIdConnectJwtValidationParameters()
                    {
                        OpenIdConnectConfigurationUrl =
                            "https://jusas-tests.eu.auth0.com/.well-known/openid-configuration",
                        ValidAudiences = new List<string>()
                            {"XLjNBiBCx3_CZUAK3gagLSC_PPQjBDzB"},
                        ValidateIssuerSigningKey = true,
                        NameClaimType = ClaimTypes.NameIdentifier
                    },
                    // Dummy authorization filter, ie. doesn't really do anything, everything goes.
                    CustomAuthorizationFilter = async (principal, token, attributes) => { }
                };
            });
        }
    }
}
