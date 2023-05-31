using System.Diagnostics;
using System.Net.Http;

namespace MovieSystemTests;
internal record User(int Id, string Name, string Email);
internal record UserGenre(int UserId, int GenreId);

public class MovieSystemTest
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("http://localhost:5169")
    };

    [Fact]
    public async Task GivenARequest_WhenCallingGetUser_ThenTheAPIReturnsExpectedResponse()
    {
        // Arrange.
        var expectedStatusCode = System.Net.HttpStatusCode.OK;
        var expectedContent = new[]
        {
            new User(1, "Franke", "k@k.se"),
            new User(2, "Bengan", "b@b.se")
        };
        var stopwatch = Stopwatch.StartNew();

        // Act.
        var response = await _httpClient.GetAsync("/get/user");

        // Assert.
        await TestHelpers.AssertResponseWithContentAsync(stopwatch, response, expectedStatusCode, expectedContent);
    }

    [Fact]
    public async Task GivenARequest_WhenCallingAddGenre_ThenTheAPIReturnsExpectedResponse()
    {
        // Arrange.
        var expectedStatusCode = System.Net.HttpStatusCode.OK;
        var expectedContent = new UserGenre(1, 53);

        var stopwatch = Stopwatch.StartNew();

        // Act.
        var response = await _httpClient.PostAsync("/post/addgenre?userId=1&genreId=53", null);

        // Assert.
        await TestHelpers.AssertResponseWithContentAsync(stopwatch, response, expectedStatusCode, expectedContent);
    }

    [Fact]
    public async Task GivenARequestWithWrongMethod_WhenCallingAddGenre_ThenTheAPIReturnsExpectedErrorCode()
    {
        // Arrange.
        var expectedStatusCode = System.Net.HttpStatusCode.MethodNotAllowed;
        var expectedContent = new UserGenre(1, 53);

        var stopwatch = Stopwatch.StartNew();

        // Act.
        var response = await _httpClient.GetAsync("/post/addgenre?userId=1&genreId=53");

        // Assert.
        TestHelpers.AssertCommonResponseParts(stopwatch, response, expectedStatusCode);
    }

}
