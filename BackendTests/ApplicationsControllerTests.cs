using Backend.Model;
using Backend.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BackendTests;

public class ApplicationsControllerTests
{    
    private DataContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            return new DataContext(options);
    }

    [Fact]
    public async Task GetApplicationsNoApplicationsTest()
    {
        // Arrange
        var dbContext = GetDbContext();
        // Arrange
        var controller = new ApplicationsController(dbContext);
    
        // Act
        var result = await controller.GetApplications();
        var okResult = result.Result as OkObjectResult;
        var applications = okResult.Value as List<Application>;
    
        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsType<List<Application>>(applications);
        Assert.Empty(applications);
    }
    
    [Fact]
    public async Task GetApplicationsWhenOneApplicationIsNotDueSoonTest()
    {
        // Arrange
        var dbContext = GetDbContext();
        var controller = new ApplicationsController(dbContext);
        var application = new Application()
        {
            description = "Test application",
            entryDate = DateTime.UtcNow,
            solveDate = DateTime.UtcNow.AddHours(2)
        };
        await controller.CreateApplication(application);
    
        // Act
        var result = await controller.GetApplications();
        var okResult = result.Result as OkObjectResult;
        var applications = okResult.Value as List<Application>;
    
        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsType<List<Application>>(applications);
        Assert.Single(applications);
        Assert.Equal(application.description, applications[0].description);
        Assert.Equal(TimeZoneInfo.ConvertTimeFromUtc(application.entryDate, TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow")), applications[0].entryDate);
        Assert.Equal(TimeZoneInfo.ConvertTimeFromUtc(application.solveDate, TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow")), applications[0].solveDate);
        Assert.False(applications[0].hasBeenCompleted);
        Assert.False(applications[0].isDueSoon);
    }
    [Fact]
    public async Task GetApplicationsWhenOneApplicationIsDueSoonTest()
    {    
        // Arrange
        var dbContext = GetDbContext();
        var controller = new ApplicationsController(dbContext);
        var application = new Application()
        {
            description = "Test application",
            entryDate = DateTime.UtcNow,
            solveDate = DateTime.UtcNow.AddHours(1)
        };
        await controller.CreateApplication(application);
    
        // Act
        var result = await controller.GetApplications();
        var okResult = result.Result as OkObjectResult;
        var applications = okResult.Value as List<Application>;
    
        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsType<List<Application>>(applications);
        Assert.Single(applications);
        Assert.Equal(application.description, applications[0].description);
        Assert.Equal(TimeZoneInfo.ConvertTimeFromUtc(application.entryDate, TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow")), applications[0].entryDate);
        Assert.Equal(TimeZoneInfo.ConvertTimeFromUtc(application.solveDate, TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow")), applications[0].solveDate);
        Assert.False(applications[0].hasBeenCompleted);
        Assert.True(applications[0].isDueSoon);
    }
    [Fact]
    public async Task UpdateApplicationTest()
    {
        // Arrange
        var dbContext = GetDbContext();
        var controller = new ApplicationsController(dbContext);

        // Create a new application
        var application = new Application()
        {
            description = "Test application",
            entryDate = DateTime.UtcNow,
            solveDate = DateTime.UtcNow.AddHours(1)
        };
        await controller.CreateApplication(application);

        // Update the application's `hasBeenCompleted` property to `true`
        var updateApplication = new Application()
        {
            hasBeenCompleted = true
        };
        var updateResult = await controller.UpdateApplication(application.id, updateApplication);

        // Assert that the update was successful
        Assert.IsType<NoContentResult>(updateResult);

        // Get the list of applications
        var getResult = await controller.GetApplications();
        var okResult = getResult.Result as OkObjectResult;
        var applications = okResult.Value as List<Application>;

        // Assert that the list is empty, since the only application has been completed
        Assert.IsType<OkObjectResult>(getResult.Result);
        Assert.Empty(applications);
    }
}