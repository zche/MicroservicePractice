using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using User.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace User.Api.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly UserContext _userContext;

        public ValuesController(UserContext userContext)
        {
            _userContext = userContext;
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Json(await _userContext.Users.SingleOrDefaultAsync(u=>u.Name=="check"));
        }

      
    }
}
