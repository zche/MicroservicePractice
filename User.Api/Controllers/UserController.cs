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
using Microsoft.AspNetCore.JsonPatch;
using User.Api.Models;
using User.Api.Dtos;
using DotNetCore.CAP;

namespace User.Api.Controllers
{
    [Route("api/users")]
    public class UserController : BaseController
    {
        private readonly UserContext _userContext;
        private readonly ILogger<UserController> _logger;
        private readonly ICapPublisher _capPublisher;

        private void RaiseUserProfileChangeEvent(AppUser user)
        {
            if (_userContext.Entry(user).Property(nameof(user.Name)).IsModified ||
                _userContext.Entry(user).Property(nameof(user.Title)).IsModified ||
                _userContext.Entry(user).Property(nameof(user.Company)).IsModified ||
                _userContext.Entry(user).Property(nameof(user.Avatar)).IsModified)
            {
                _capPublisher.Publish("userapi.user_profile_changed", new UserIdentity
                {
                    UserId = user.Id,
                    Name = user.Name,
                    Company = user.Company,
                    Avatar = user.Avatar
                });
            }
        }

        public UserController(UserContext userContext, ILogger<UserController> logger, ICapPublisher capPublisher)
        {
            _userContext = userContext;
            _logger = logger;
            _capPublisher = capPublisher;
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
            var originProperties = await _userContext.UserProperties.AsNoTracking().Where(u => u.AppUserId == UserIdentity.UserId).ToListAsync();
            var allProperties = originProperties.Union(user.Properties).Distinct();

            var removedProperties = originProperties.Except(user.Properties);
            var newProperties = allProperties.Except(originProperties);

            _userContext.RemoveRange(removedProperties);
            _userContext.AddRange(newProperties);

            using (var trans = _userContext.Database.BeginTransaction())
            {
                this.RaiseUserProfileChangeEvent(user);//发布用户属性变更的消息
                _userContext.SaveChanges();
                trans.Commit();
            }

            return Json(user);
        }

        /// <summary>
        /// 检查或者创建用户（当用户手机号不存在的时候创建用户）
        /// </summary>
        /// <param name="tel"></param>
        /// <returns></returns>
        [HttpPost("check-or-create")]
        public async Task<IActionResult> CheckOrCreate([FromBody]RequestCheckUserDto dto)
        {
            var user = await _userContext.Users.FirstOrDefaultAsync(u => u.Tel == dto.Tel);
            if (user == null)
            {
                var newUser = new AppUser { Tel = dto.Tel };
                _userContext.Users.Add(newUser);
                _userContext.SaveChanges();
                return Ok(newUser.Id);
            }
            return Ok(new
            {
                user.Id,
                user.Name,
                user.Company,
                user.Title,
                user.Avatar
            });
        }

        [HttpGet("userInfo/{userId}")]
        public async Task<IActionResult> GetBaseUserInfo(int userId)
        {
            var user = await _userContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();
            return Json(new
            {
                UserId = user.Id,
                user.Name,
                user.Company,
                user.Title,
                user.Avatar
            });
        }

        [HttpGet("tags")]
        public async Task<IActionResult> GetUserTags()
        {
            return Json(await _userContext.UserTags.ToListAsync());
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search(string tel)
        {
            return Json(await _userContext.Users.FirstOrDefaultAsync(u => u.Tel == tel));
        }

        [HttpPut("tags")]
        public async Task<IActionResult> UpdateTags([FromBody]List<string> tags)
        {
            var originTags = await _userContext.UserTags.Where(u => u.UserId == this.UserIdentity.UserId).ToListAsync();
            var newTags = tags.Except(originTags.Select(t => t.Tag));
            if (newTags != null && newTags.Count() > 0)
            {
                _userContext.UserTags.AddRange(newTags.Select(t => new UserTag
                {
                    UserId = this.UserIdentity.UserId,
                    CreatedTime = DateTime.Now,
                    Tag = t
                }));
                _userContext.SaveChanges();
            }
            return Ok();
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
