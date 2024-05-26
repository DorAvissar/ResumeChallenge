using System;
using System.IO;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace tests
{
    public static class TestFactory
    {
        public static HttpRequestData CreateHttpRequestData(FunctionContext context, string body)
        {
            var request = new Mock<HttpRequestData>(context);
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(body);
            writer.Flush();
            memoryStream.Position = 0;

            request.Setup(r => r.Body).Returns(memoryStream);
            request.Setup(r => r.Method).Returns("GET");
            request.Setup(r => r.Url).Returns(new Uri("https://localhost"));

            return request.Object;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;

            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }

            return logger;
        }
    }
}
