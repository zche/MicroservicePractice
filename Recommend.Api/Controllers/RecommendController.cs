using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Recommend.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Recommend.Api.Controllers
{
    [Route("api/[controller]")]
    public class RecommendController : BaseController
    {
        private readonly RecommendContext _context;
        public RecommendController(RecommendContext context)
        {
            _context = context;
        }
        // GET api/values
        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            var currentId = this.UserIdentity.UserId;
            var result = await _context.ProjectRecommends.AsNoTracking()
                .Where(p => p.UserId == currentId).ToListAsync();
            return Ok(result);
        }
    }
}
