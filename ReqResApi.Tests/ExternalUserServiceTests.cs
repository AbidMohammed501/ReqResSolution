using Moq;
using Moq.Protected;
using ReqResApiIntegration.Models;
using ReqResApiIntegration.Services;
using System.Net;
using System.Text.Json;

namespace ReqResApi.Tests
{
    public class ExternalUserServiceTests
    {
        private IExternalUserService CreateService(HttpResponseMessage mockResponse)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse);

            var client = new HttpClient(handlerMock.Object);

            // Create fake options
            var options = Microsoft.Extensions.Options.Options.Create(new ReqResApiIntegration.Config.ReqResOptions
            {
                BaseUrl = "https://reqres.in/api",
                ApiKey = "test-api-key",
                CacheExpirationSeconds = 60
            });

            // Use a real memory cache (safe for unit testing)
            var memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(
                new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());

            return new ExternalUserService(client, options, memoryCache);
        }


        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenSuccess()
        {
            var userJson = JsonSerializer.Serialize(new SingleUserResponse
            {
                Data = new User { Id = 1, Email = "test@reqres.in", FirstName = "Test", LastName = "User" }
            });

            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(userJson)
            };

            var service = CreateService(mockResponse);
            var user = await service.GetUserByIdAsync(1);

            Assert.NotNull(user);
            Assert.Equal(1, user?.Id);
            Assert.Equal("test@reqres.in", user?.Email);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsList_WhenSuccess()
        {
            var userJson = JsonSerializer.Serialize(new UserDataResponse
            {
                Page = 1,
                TotalPages = 1,
                Data = new List<User>
                {
                    new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@reqres.in" }
                }
            });

            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(userJson)
            };

            var service = CreateService(mockResponse);
            var users = await service.GetAllUsersAsync();

            Assert.NotEmpty(users);
        }
    }
}