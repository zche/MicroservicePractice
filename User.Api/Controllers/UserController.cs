﻿using System;
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
using Microsoft.AspNetCore.JsonPatch;
using User.Api.Models;

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
        public async Task<IActionResult> Patch([FromBody]JsonPatchDocument<AppUser> patch)
        {
            var user = await _userContext.Users
                //.Include(u => u.Properties)
                .FirstOrDefaultAsync(u => u.Id == UserIdentity.UserId);

            //var originProperties = user.Properties;

            patch.ApplyTo(user);
            foreach (var property in user.Properties)
            {
                _userContext.Entry(property).State = EntityState.Detached;
            }
            var originProperties = await _userContext.UserProperties.AsNoTracking().Where(u=>u.AppUserId==UserIdentity.UserId).ToListAsync();
            var allProperties = originProperties.Union(user.Properties).Distinct();

            var removedProperties = originProperties.Except(user.Properties);
            var newProperties = allProperties.Except(originProperties);

            _userContext.RemoveRange(removedProperties);
            _userContext.AddRange(newProperties);
            _userContext.SaveChanges();
            return Json(user);
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