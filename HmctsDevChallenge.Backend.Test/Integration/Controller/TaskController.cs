using System.Net;
using HmctsDevChallenge.Backend.Database;
using HmctsDevChallenge.Backend.Models.Dto;
using HmctsDevChallenge.Backend.Test.Integration.Dependencies;
using Microsoft.AspNetCore.Mvc;

namespace HmctsDevChallenge.Backend.Test.Integration.Controller;

public class TaskController_CreateTaskShould(TestWebApplicationFactory webAppFactory)
    : IClassFixture<TestWebApplicationFactory>
{
    [Fact]
    public async Task CreateTask_WithValidData_ReturnsOkWithCreatedTask()
    {
        // ARRANGE
        var client = webAppFactory.CreateClient();

        var createDto = new TaskDto.Create
        {
            Title = "Test Task",
            Description = "This is a test task description",
            Status = "Pending",
            Due = DateTime.UtcNow.AddDays(7)
        };

        // ACT
        var response = await client.PostAsJsonAsync("/api/task", createDto);

        // ASSERT
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var createdTask = await response.Content.ReadFromJsonAsync<TaskDto.Read>();
        Assert.True(createdTask.Id > 0);
        Assert.Equal(createDto.Title, createdTask.Title);
        Assert.Equal(createDto.Description, createdTask.Description);
        Assert.Equal(createDto.Status, createdTask.Status);

        Assert.Equal(createDto.Due.ToUniversalTime(), createdTask.Due);

        using var scope = webAppFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<HmctsDbContext>();
        var taskInDb = await dbContext.Tasks.FindAsync(createdTask.Id);

        Assert.NotNull(taskInDb);
        Assert.Equal(createDto.Title, taskInDb.Title);
        Assert.Equal(createDto.Description, taskInDb.Description);
        Assert.Equal(createDto.Status, taskInDb.Status);
        Assert.Equal(createDto.Due.ToUniversalTime(), taskInDb.Due);
    }

    [Fact]
    public async Task CreateTask_WithNoDescription_ReturnsOkWithCreatedTask()
    {
        // ARRANGE
        var client = webAppFactory.CreateClient();

        var createDto = new TaskDto.Create
        {
            Title = "Test Task",
            Status = "Pending",
            Due = DateTime.UtcNow.AddDays(7)
        };

        // ACT
        var response = await client.PostAsJsonAsync("/api/task", createDto);

        // ASSERT
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var createdTask = await response.Content.ReadFromJsonAsync<TaskDto.Read>();
        Assert.True(createdTask.Id > 0);
        Assert.Equal(createDto.Title, createdTask.Title);
        Assert.Null(createdTask.Description);
        Assert.Equal(createDto.Status, createdTask.Status);

        Assert.Equal(createDto.Due.ToUniversalTime(), createdTask.Due);

        using var scope = webAppFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<HmctsDbContext>();
        var taskInDb = await dbContext.Tasks.FindAsync(createdTask.Id);

        Assert.NotNull(taskInDb);
        Assert.Equal(createDto.Title, taskInDb.Title);
        Assert.Null(taskInDb.Description);
        Assert.Equal(createDto.Status, taskInDb.Status);
        Assert.Equal(createDto.Due.ToUniversalTime(), taskInDb.Due);
    }

    [Fact]
    public async Task CreateTask_WithTitleOver255Chars_ReturnsBadRequestWithErrorMessage()
    {
        // ARRANGE
        var client = webAppFactory.CreateClient();

        var createDto = new TaskDto.Create
        {
            Title = new string('T', 256),
            Description = "This is a test task description",
            Status = "Pending",
            Due = DateTime.UtcNow.AddDays(7)
        };

        // ACT
        var response = await client.PostAsJsonAsync("/api/task", createDto);

        // ASSERT
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.True(problemDetails.Errors.Count > 0);
        Assert.True(problemDetails.Errors.TryGetValue("Title", out var title));
        Assert.True(title.Contains("Title cannot exceed 255 characters."));
    }

    [Fact]
    public async Task CreateTask_WithDescriptionOver2046Chars_ReturnsBadRequestWithErrorMessage()
    {
        // ARRANGE
        var client = webAppFactory.CreateClient();

        var createDto = new TaskDto.Create
        {
            Title = "Test Task",
            Description = new string('D', 2047),
            Status = "Pending",
            Due = DateTime.UtcNow.AddDays(7)
        };

        // ACT
        var response = await client.PostAsJsonAsync("/api/task", createDto);

        // ASSERT
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.True(problemDetails.Errors.Count > 0);
        Assert.True(problemDetails.Errors.TryGetValue("Description", out var description));
        Assert.True(description.Contains("Description cannot exceed 2046 characters."));
    }

    [Fact]
    public async Task CreateTask_WithStatusOver63Chars_ReturnsBadRequestWithErrorMessage()
    {
        // ARRANGE
        var client = webAppFactory.CreateClient();

        var createDto = new TaskDto.Create
        {
            Title = "Test Task",
            Description = "This is a test task description",
            Status = new string('S', 64),
            Due = DateTime.UtcNow.AddDays(7)
        };

        // ACT
        var response = await client.PostAsJsonAsync("/api/task", createDto);

        // ASSERT
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.True(problemDetails.Errors.Count > 0);
        Assert.True(problemDetails.Errors.TryGetValue("Status", out var status));
        Assert.True(status.Contains("Status cannot exceed 63 characters."));
    }

    [Fact]
    public async Task CreateTask_WithBadFormatDueDate_ReturnsBadRequestWithErrorMessage()
    {
        // ARRANGE
        var client = webAppFactory.CreateClient();

        var createDto = new
        {
            Title = "Test Task",
            Description = "This is a test task description",
            Status = "Pending",
            Due = "not-a-date"
        };

        // ACT
        var response = await client.PostAsJsonAsync("/api/task", createDto);

        // ASSERT
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.Contains("The JSON value could not be converted to System.DateTime", responseBody);
    }

    [Fact]
    public async Task CreateTask_WithPresentDueDate_ReturnsBadRequestWithErrorMessage()
    {
        // ARRANGE
        var client = webAppFactory.CreateClient();

        var createDto = new TaskDto.Create
        {
            Title = "Test Task",
            Description = "This is a test task description",
            Status = "Pending",
            Due = DateTime.UtcNow
        };

        // ACT
        var response = await client.PostAsJsonAsync("/api/task", createDto);

        // ASSERT
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.True(problemDetails.Errors.Count > 0);
        Assert.True(problemDetails.Errors.TryGetValue("Due", out var due));
        Assert.True(due.Contains("Due Date must be in the future."));
    }

    [Fact]
    public async Task CreateTask_WithPastDueDate_ReturnsBadRequestWithErrorMessage()
    {
        // ARRANGE
        var client = webAppFactory.CreateClient();

        var createDto = new TaskDto.Create
        {
            Title = "Test Task",
            Description = "This is a test task description",
            Status = "Pending",
            Due = DateTime.UtcNow.AddDays(-7)
        };

        // ACT
        var response = await client.PostAsJsonAsync("/api/task", createDto);

        // ASSERT
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.True(problemDetails.Errors.Count > 0);
        Assert.True(problemDetails.Errors.TryGetValue("Due", out var due));
        Assert.True(due.Contains("Due Date must be in the future."));
    }
}