using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Contact.Api.Data;
using Contact.Api.Models;
using Contact.Api.Services;

namespace Contact.Api.Controllers
{
    [Route("api/[controller]")]
    public class ContactController : BaseController
    {
        private readonly IContactApplyRequestRepository _contactApplyRequestRepository;
        private readonly IUserService _userService;

        public ContactController(IContactApplyRequestRepository contactApplyRequestRepository, IUserService userService)
        {
            _contactApplyRequestRepository = contactApplyRequestRepository;
            _userService = userService;
        }

        /// <summary>
        /// 获取当前用户的好友申请列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("apply-list")]
        public async Task<IActionResult> GetApplyRequest()
        {
            var request = await _contactApplyRequestRepository.GetRequestListAsync(this.UserIdentity.UserId);
            return Json(request);
        }

        /// <summary>
        /// 添加用户的好友申请
        /// </summary>
        /// <param name="applicantId"></param>
        /// <returns></returns>
        [HttpPost("add-apply")]
        public async Task<IActionResult> AddApplyRequest(int userId)
        {
            var baseUserInfo = await _userService.GetBaseUserInfoAsync(userId);
            if (baseUserInfo == null)
                throw new Exception("用户不存在");
            var result = await _contactApplyRequestRepository.AddRequestAsync(
                new ContactApplyRequest
                {
                    UserId = baseUserInfo.UserId,
                    Name = baseUserInfo.Name,
                    Company = baseUserInfo.Company,
                    Avatar = baseUserInfo.Avatar,
                    Title = baseUserInfo.Title,
                    CreatedTime = DateTime.Now,
                    ApplyId = this.UserIdentity.UserId
                }
                );
            if (!result) return BadRequest();
            return Ok();
        }

        /// <summary>
        /// 通过好友申请
        /// </summary>
        /// <param name="applicantId"></param>
        /// <returns></returns>
        [HttpPut("approve-apply")]
        public async Task<IActionResult> ApproveApplyRequest(int applicantId)
        {
            var result = await _contactApplyRequestRepository.ApproveAsync(applicantId);
            if (!result) return BadRequest();
            return Ok();
        }
    }
}
