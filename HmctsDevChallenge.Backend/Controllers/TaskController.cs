using HmctsDevChallenge.Backend.Database;
using HmctsDevChallenge.Backend.Models.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task = HmctsDevChallenge.Backend.Models.Database.Task;

namespace HmctsDevChallenge.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController(HmctsDbContext dbContext) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<TaskDto.Read>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, "application/problem+json")]
    public async Task<Results<Ok<TaskDto.Read>, NotFound<ProblemDetails>, InternalServerError<ProblemDetails>>>
        CreateTask(
            [FromBody] TaskDto.Create body)
    {
        var task = new Task
        {
            Title = body.Title,
            Description = body.Description,
            Status = body.Status,
            Due = body.Due.ToUniversalTime()
        };
        dbContext.Tasks.Add(task);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return TypedResults.InternalServerError(new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "An error occurred while saving changes to the database!",
                Status = StatusCodes.Status500InternalServerError
            });
        }

        var dbTask = await dbContext.Tasks
            .AsNoTracking()
            .Select(t => new TaskDto.Read
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Due = t.Due
            })
            .FirstOrDefaultAsync(t => t.Id == task.Id);

        if (dbTask.Equals(default))
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = $"The Task with ID: [{task.Id}] could not be found!",
                Status = StatusCodes.Status404NotFound
            });

        // This would be TypedResults.Created(<url>) in a normal Create endpoint
        return TypedResults.Ok(dbTask);
    }
}