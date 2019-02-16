using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using NSwag.Annotations.AzureFunctionsV2;
using NSwag.SwaggerGeneration.AzureFunctionsV2;

namespace ExampleFunctionApp
{
    public static class ExampleFunctions
    {
        public class MyGreeting
        {
            public string Message { get; set; }
        }

        [SwaggerResponse(200, typeof(MyGreeting))]
        [SwaggerQueryParameter("name", true, typeof(string), "The caller's name")]
        [FunctionName("hello")]
        public static async Task<IActionResult> Hello(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "hello")]
            HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult(new MyGreeting() {Message = req.Query["name"]});
        }

        [SwaggerIgnore]
        [FunctionName("swagger")]
        public static async Task<IActionResult> Swagger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger")]
            HttpRequest req,
            ILogger log)
        {
            var settings = new AzureFunctionsV2ToSwaggerGeneratorSettings()
            {
                Title = "My Function App Swagger",
                Description = "Here be dragons!",
                Version = "1.0"
            };
            var generator = new AzureFunctionsV2ToSwaggerGenerator(settings);
            var swaggerDoc = await generator.GenerateForAzureFunctionClassAsync(typeof(ExampleFunctions));
            var json = swaggerDoc.ToJson();
            return new OkObjectResult(json);
        }
    }
}
