﻿using System.Collections.Generic;
using NSwag;
using NSwag.SwaggerGeneration.AzureFunctionsV2;
using NSwag.SwaggerGeneration.AzureFunctionsV2.Processors;
using NSwag.SwaggerGeneration.Processors.Security;

namespace HttpExtensionsExamples.Startup
{
    public static class SwaggerConfiguration
    {
        public static AzureFunctionsV2ToSwaggerGeneratorSettings SwaggerGeneratorSettings { get; set; }

        /// <summary>
        /// Initialize SwaggerGenerator configuration.
        /// Add OperationSecurityProcessors and SecurityDefinitionAppenders to the settings.
        /// </summary>
        static SwaggerConfiguration()
        {
            var settings = new AzureFunctionsV2ToSwaggerGeneratorSettings();
            SwaggerGeneratorSettings = settings;

            settings.Title = "Azure Functions Swagger example";
            settings.Description =
                "This is an example generated Swagger JSON using NSwag.SwaggerGeneration.AzureFunctionsV2 and AzureFunctionsV2.HttpExtensions to " +
                "generate Swagger output directly from the assembly. <br/><br/>Credentials for testing:<br/><br/><b>OAuth2:</b> " +
                "\"testuser@testcorp.eu\" : \"foobar123---\", use client_id: \"XLjNBiBCx3_CZUAK3gagLSC_PPQjBDzB\"" +
                "<br/><b>Basic auth:</b> \"user\" : \"pass\" <br/> " +
                "<b>ApiKey:</b> \"key\".";

            settings.OperationProcessors.Add(new OperationSecurityProcessor("Basic",
                SwaggerSecuritySchemeType.Basic));
            settings.DocumentProcessors.Add(new SecurityDefinitionAppender("Basic", new SwaggerSecurityScheme()
            {
                Type = SwaggerSecuritySchemeType.Basic,
                Scheme = "Basic",
                Description = "Basic auth"
            }));

            settings.OperationProcessors.Add(new OperationSecurityProcessor("ApiKey",
                SwaggerSecuritySchemeType.ApiKey, SwaggerSecurityApiKeyLocation.Header));
            settings.DocumentProcessors.Add(new SecurityDefinitionAppender("ApiKey", new SwaggerSecurityScheme()
            {
                Type = SwaggerSecuritySchemeType.ApiKey,
                Name = "x-apikey",
                In = SwaggerSecurityApiKeyLocation.Header
            }));

            settings.OperationProcessors.Add(new OperationSecurityProcessor("Bearer",
                SwaggerSecuritySchemeType.OpenIdConnect));
            settings.DocumentProcessors.Add(new SecurityDefinitionAppender("Bearer", new SwaggerSecurityScheme()
            {
                Type = SwaggerSecuritySchemeType.OAuth2,
                Flow = SwaggerOAuth2Flow.Implicit,
                Flows = new OpenApiOAuthFlows()
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = "https://jusas-tests.eu.auth0.com/authorize",
                        Scopes = new Dictionary<string, string>()
                            {{"openid", "openid"}, {"profile", "profile"}, {"name", "name"}},
                        TokenUrl = "https://jusas-tests.eu.auth0.com/oauth/token"
                    }
                },
                Description = "Token"
            }));
        }

    }
}
