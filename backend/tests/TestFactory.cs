using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using Company.Function;

namespace tests
{
    public class TestCounter
    {
        private readonly ILogger logger = new ListLogger();

        [Fact]
        public async Task TestRun()
        {
            // Arrange
            var functionContext = new Mock<FunctionContext>().Object;
            var request = TestFactory.CreateHttpRequestData(functionContext, string.Empty);

            // Act
            var response = await GetResumeCounter.Run(request, functionContext);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }

    // Example implementation of ListLogger (make sure this matches your actual implementation)
    public class ListLogger : ILogger
    {
        // Implement ILogger methods...
    }
}
