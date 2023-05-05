using Backend.Model;
using Backend.ModelView;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<IEnumerable<ApplicationView>>> GetApplications()
        {
            var applications = await _context.Applications
                .OrderByDescending(a => a.solveDate)
                .Select(a => new ApplicationView
                {
                    id = a.id,
                    description = a.description,
                    entryDate = a.entryDate,// convert entryDate to GMT+3
                    solveDate = a.solveDate, // convert solveDate to GMT+3
                    hasBeenCompleted = a.hasBeenCompleted,
                    isDueSoon = (a.solveDate - DateTime.UtcNow).TotalHours <= 1 // Set isDueSoon to true if the solveDate is within the next hour in GMT+3
                }).ToListAsync();

            return Ok(applications);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateApplication([FromBody] CreateApplication createApplication)
        {
            var application = new Application
            {
                description = createApplication.description,
                solveDate = createApplication.solveDate,
                entryDate = DateTime.Now,
                hasBeenCompleted = false
            };
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
        
            return Ok(application);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApplication(int id, bool hasBeenCompleted)
        {
            var existingApplication = await _context.Applications.FindAsync(id);

            if (existingApplication == null)
            {
                return NotFound();
            }

            existingApplication.hasBeenCompleted = hasBeenCompleted;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // [HttpPut("{id}")]
        // public async Task<IActionResult> UpdateApplication(int id, [FromBody] Application application)
        // {
        //     var existingApplication = await _context.Applications.FindAsync(id);

        //     if (existingApplication == null)
        //     {
        //         return NotFound();
        //     }

        //     existingApplication.hasBeenCompleted = application.hasBeenCompleted;

        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }


        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<Application>>> GetApplications()
        // {
        //     TimeZoneInfo gmtPlus3 = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow"); // set the time zone to GMT+3
        //     var applications = await _context.Applications
        //         .Where(a => !a.hasBeenCompleted)
        //         .OrderByDescending(a => a.solveDate)
        //         .Select(a => new Application
        //         {
        //             id = a.id,
        //             description = a.description,
        //             entryDate = TimeZoneInfo.ConvertTimeFromUtc(a.entryDate, gmtPlus3), // convert entryDate to GMT+3
        //             solveDate = TimeZoneInfo.ConvertTimeFromUtc(a.solveDate, gmtPlus3), // convert solveDate to GMT+3
        //             hasBeenCompleted = a.hasBeenCompleted,
        //             isDueSoon = (TimeZoneInfo.ConvertTimeFromUtc(a.solveDate, gmtPlus3) - TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, gmtPlus3)).TotalHours <= 1 // Set isDueSoon to true if the solveDate is within the next hour in GMT+3
        //         })
        //         .ToListAsync();
        
        //     return Ok(applications);
        // }

        // [HttpPost]
        // public async IActionResult CreateApplication([FromBody] ApplicationView applicationView)
        // {
        //     var application = new Application()
        //     {
        //         description = applicationView.description,
        //         entryDate = DateTime.Now,
        //         solveDate = applicationView.solveDate
        //     };
        //     _context.Applications.Add(application);
        //     await _context.SaveChangesAsync();

        //     return Ok(application);
        // }

    }
}