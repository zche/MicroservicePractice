using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using User.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;

namespace User.Api.Controllers
{
    [Route("api/users")]
    public class UserController : BaseController
    {
        private readonly UserContext _userContext;
        private readonly ILogger<UserController> _logger;

        public UserController(UserContext userContext, ILogger<UserController> logger)
        {
            _userContext = userContext;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            var user = await _userContext.Users
                .AsNoTracking()
                .Include(u => u.Properties)
                .FirstOrDefaultAsync(u => u.Id == UserIdentity.UserId);
            if (user == null)
                throw new UserOperationException($"错误的用户上下文id{UserIdentity.UserId}");
            return Json(user);
        }

        [HttpPatch("")]
        public async Task<IActionResult> Patch()
        {
            return Json(new
            {
                message = "welcome!",
                user = await _userContext.Users.FirstOrDefaultAsync(u => u.Name == "check")
            });
        }


        [HttpGet("Exception")]
        public IActionResult Exception()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var request = HttpContext.Features.Get<IHttpRequestFeature>();

            if (exception != null && request != null)
            {
                var message = $@"RequestUrl: ${request.Path}异常信息: ${exception.Error}";
                _logger.LogError(message);
            }
            if (exception.Error.GetType() == typeof(UserOperationException))
            {
                var json = new JsonErrorResponse { Message = exception.Error.Message };
                return new BadRequestObjectResult(json);
            }
            else
                return Json(new { StatusCode = StatusCodes.Status500InternalServerError, Message = "发生了未知的内部错误" });
        }
    }
}
