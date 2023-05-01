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
}