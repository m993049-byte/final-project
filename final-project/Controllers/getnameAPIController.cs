using final_project.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace final_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
     public class getnameAPIController : ControllerBase
    {

        private readonly final_projectContext _context;

        public getnameAPIController(final_projectContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return Ok(new List<string>());

            role = role.ToLower();

            var names = await _context.usersaccounts
                .Where(u => u.role.ToLower() == role)
                .Select(u => u.name)         
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            return Ok(names);
        }


    }
}
