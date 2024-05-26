using System; 
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Company.Function
{
    public static class GetResumeCounter
    {
        private static CosmosClient cosmosClient;
        private static Container container;

        static GetResumeCounter()
        {
            // Load configuration from local.settings.json.
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

           string connectionString = config["AzureResumeConnectionString"];
            if (connectionString == null)
            {
                throw new Exception("Azure Resume Connection String is not configured.");
            }
            cosmosClient = new CosmosClient(connectionString);
            container = cosmosClient.GetContainer("AzureResume", "Counter");
        }

[Function("GetResumeCounter")]
public static async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestData req,
    FunctionContext executionContext)
{
    var logger = executionContext.GetLogger("GetResumeCounter");
    logger.LogInformation("C# HTTP trigger function processed a request.");

    try
    {
        // Read the item !
        ItemResponse<Counter> response = await container.ReadItemAsync<Counter>("1", new PartitionKey("1"));
        Counter counter = response.Resource;

        // Update the item ! 
        counter.Count += 1;

        // Replace the item in the database.
        await container.ReplaceItemAsync(counter, "1", new PartitionKey("1"));

        var jsonToReturn = JsonConvert.SerializeObject(counter);

        var responseMessage = req.CreateResponse(System.Net.HttpStatusCode.OK);
        responseMessage.Headers.Add("Content-Type", "application/json");

        // Write to the response body
        await responseMessage.WriteStringAsync(jsonToReturn, Encoding.UTF8);

        return responseMessage;
    }
    catch (Exception ex)
    {
        logger.LogError($"Error: {ex.Message}");
        var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
        await errorResponse.WriteStringAsync("Internal server error", Encoding.UTF8);
        return errorResponse;
    }
}

    }

    public class Counter
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
