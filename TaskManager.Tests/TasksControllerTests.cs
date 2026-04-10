using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.API.Controllers;
using TaskManager.API.Data;
using TaskManager.API.Models;
using Xunit;

namespace TaskManager.Tests;

public class TasksControllerTests
{
    // Helper method to create a fresh In-Memory Database for every test
    private AppDbContext GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name ensures tests don't interfere
            .Options;
        
        var databaseContext = new AppDbContext(options);
        databaseContext.Database.EnsureCreated();
        return databaseContext;
    }

    [Fact]
    public async Task GetAll_ReturnsAllTasksFromDatabase()
    {
        // Arrange (Setup)
        var context = GetDatabaseContext();
        context.Tasks.Add(new TaskItem { Title = "Task 1", IsCompleted = false });
        context.Tasks.Add(new TaskItem { Title = "Task 2", IsCompleted = true });
        await context.SaveChangesAsync();
        
        var controller = new TasksController(context);

        // Act (Execute)
        var result = await controller.GetAll();

        // Assert (Verify)
        var actionResult = Assert.IsType<ActionResult<IEnumerable<TaskItem>>>(result);
        var tasks = Assert.IsAssignableFrom<IEnumerable<TaskItem>>(actionResult.Value);
        Assert.Equal(2, tasks.Count());
    }

    [Fact]
    public async Task Create_SavesNewTaskSuccessfully()
    {
        // Arrange
        var context = GetDatabaseContext();
        var controller = new TasksController(context);
        var newTask = new TaskItem { Title = "DevOps Project", Description = "Build CI/CD" };

        // Act
        var result = await controller.Create(newTask);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedTask = Assert.IsType<TaskItem>(createdAtActionResult.Value);
        Assert.Equal("DevOps Project", returnedTask.Title);
        Assert.Equal(1, await context.Tasks.CountAsync());
    }

    [Fact]
    public async Task Delete_RemovesTaskAndReturnsNoContent()
    {
        // Arrange
        var context = GetDatabaseContext();
        var task = new TaskItem { Id = 99, Title = "Delete Me" };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();
        
        var controller = new TasksController(context);

        // Act
        var result = await controller.Delete(99);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Equal(0, await context.Tasks.CountAsync());
    }
}