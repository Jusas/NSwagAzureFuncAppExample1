using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzureFunctionsV2.HttpExtensions.Annotations;
using AzureFunctionsV2.HttpExtensions.Authorization;
using AzureFunctionsV2.HttpExtensions.Infrastructure;
using HttpExtensionsExamples.Startup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSwag.Annotations;
using NSwag.SwaggerGeneration.AzureFunctionsV2;

namespace HttpExtensionsExamples
{
    public static class HttpExtensionFunctions
    {
        public class MyResponse
        {
            public string Message { get; set; }
        }

        public class MyUser
        {
            public string Id { get; set; }
            public string NickName { get; set; }
        }

        [SwaggerResponse(200, typeof(MyResponse))]
        [FunctionName("hello")]
        public static async Task<IActionResult> Hello(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "hello")] HttpRequest req,
            [HttpQuery(Required = true)]HttpParam<string> name,
            [HttpQuery(Required = true)]HttpParam<int> age,
            ILogger log)
        {
            return new OkObjectResult(new MyResponse() {Message = $"Hello {name}, {age} is a nice age!"});
        }

        [HttpAuthorize(Scheme.Basic)]
        [SwaggerResponse(200, typeof(MyResponse))]
        [SwaggerResponse(401, typeof(object))]
        [FunctionName("secrets")]
        public static async Task<IActionResult> GetSecrets(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "secrets")] HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult(new MyResponse() { Message = $"Psssht! This is a secret: two months from now, you're gonna get promoted!" });
        }

        [HttpAuthorize(Scheme.HeaderApiKey)]
        [SwaggerResponse(200, typeof(MyResponse))]
        [SwaggerResponse(401, typeof(object))]
        [FunctionName("webhook")]
        public static async Task<IActionResult> MyWebhook(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "webhook")] HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult(new MyResponse() { Message = $"Webhook call ok" });
        }

        [HttpAuthorize(Scheme.Jwt)]
        [SwaggerResponse(200, typeof(MyUser))]
        [SwaggerResponse(401, typeof(object))]
        [FunctionName("userdata")]
        public static async Task<IActionResult> UserData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "userdata")] HttpRequest req,
            [HttpToken]HttpUser user,
            ILogger log)
        {
            return new OkObjectResult(new MyUser() {Id = user.ClaimsPrincipal.Identity.Name, NickName = user.ClaimsPrincipal.Claims.First(x => x.Type == "nickname").Value});
        }

        [SwaggerIgnore]
        [FunctionName("swagger")]
        public static async Task<IActionResult> Swagger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger")]
            HttpRequest req,
            ILogger log)
        {
            var generator = new AzureFunctionsV2ToSwaggerGenerator(SwaggerConfiguration.SwaggerGeneratorSettings);
            var swaggerDoc = await generator.GenerateForAzureFunctionClassAsync(typeof(HttpExtensionFunctions));
            var json = swaggerDoc.ToJson();
            return new OkObjectResult(json);
        }
    }
}
