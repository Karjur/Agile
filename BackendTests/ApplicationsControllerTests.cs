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
public async Task GetApplications_ReturnsEmptyList_WhenNoApplications()
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
public async Task GetApplications_ReturnsList_WhenOneApplicationIsNotDueSoon()
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
public async Task GetApplications_ReturnsList_WhenOneApplicationIsDueSoon()
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


}