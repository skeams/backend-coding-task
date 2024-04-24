using Claims.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Claims.Tests
{
    public class ClaimsControllerTests
    {
        [Fact]
        public async Task Get_Claims()
        {
            var application = new WebApplicationFactory<Program>().WithWebHostBuilder(_ => {});
            var client = application.CreateClient();

            var response = await client.GetAsync("/Claims");

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            try
            {
                var claims = JsonConvert.DeserializeObject<IEnumerable<Claim>>(result);
                Assert.NotNull(claims);
            }
            catch
            {
                Assert.Fail("Should not fail deserialization");
            }
        }

    }
}
