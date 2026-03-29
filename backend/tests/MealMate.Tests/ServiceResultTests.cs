using MealMate.Application.Common;
using Xunit;

namespace MealMate.Tests;

public class ServiceResultTests
{
    [Fact]
    public void Ok_ShouldReturnSuccessWithData()
    {
        var result = ServiceResult<string>.Ok("test");

        Assert.True(result.Success);
        Assert.Equal("test", result.Data);
        Assert.Equal(200, result.StatusCode);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Created_ShouldReturn201StatusCode()
    {
        var result = ServiceResult<int>.Created(42);

        Assert.True(result.Success);
        Assert.Equal(42, result.Data);
        Assert.Equal(201, result.StatusCode);
    }

    [Fact]
    public void Fail_ShouldReturnFailureWithError()
    {
        var result = ServiceResult<string>.Fail("Something went wrong");

        Assert.False(result.Success);
        Assert.Equal("Something went wrong", result.Error);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public void Fail_WithCustomStatusCode_ShouldUseProvidedStatusCode()
    {
        var result = ServiceResult<string>.Fail("Error", 422);

        Assert.False(result.Success);
        Assert.Equal(422, result.StatusCode);
    }

    [Fact]
    public void NotFound_ShouldReturn404StatusCode()
    {
        var result = ServiceResult<string>.NotFound("Item not found");

        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("Item not found", result.Error);
    }

    [Fact]
    public void Unauthorized_ShouldReturn401StatusCode()
    {
        var result = ServiceResult<string>.Unauthorized();

        Assert.False(result.Success);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public void Forbidden_ShouldReturn403StatusCode()
    {
        var result = ServiceResult<string>.Forbidden();

        Assert.False(result.Success);
        Assert.Equal(403, result.StatusCode);
    }
}
