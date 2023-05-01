using Backend.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationsController : ControllerBase
    {
        private readonly DataContext _context;
        
        public ApplicationsController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
public ActionResult<IEnumerable<Application>> GetApplications()
{
    TimeZoneInfo gmtPlus3 = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow"); // set the time zone to GMT+3
    var applications = _context.Applications.AsEnumerable()
        .Where(a => !a.hasBeenCompleted)
        .OrderByDescending(a => a.solveDate)
        .Select(a => new Application
        {
            id = a.id,
            description = a.description,
            entryDate = TimeZoneInfo.ConvertTimeFromUtc(a.entryDate, gmtPlus3), // convert entryDate to GMT+3
            solveDate = TimeZoneInfo.ConvertTimeFromUtc(a.solveDate, gmtPlus3), // convert solveDate to GMT+3
            hasBeenCompleted = a.hasBeenCompleted,
            isDueSoon = (TimeZoneInfo.ConvertTimeFromUtc(a.solveDate, gmtPlus3) - TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, gmtPlus3)).TotalHours <= 1 // Set isDueSoon to true if the solveDate is within the next hour in GMT+3
        })
        .ToList();
    
    return Ok(applications);
}

        [HttpPost]
        public IActionResult PostApplication ([FromBody] Application application) 
        {
            _context.Applications!.Add(application);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApplication(int id, [FromBody] Application application)
        {
            var existingApplication = await _context.Applications.FindAsync(id);

            if (existingApplication == null)
            {
                return NotFound();
            }

            existingApplication.hasBeenCompleted = application.hasBeenCompleted;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}