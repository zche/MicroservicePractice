using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Contact.Api.Data;
using Contact.Api.Models;
using Contact.Api.Services;
using Contact.Api.ViewModels;

namespace Contact.Api.Controllers
{
    [Route("api/[controller]")]
    public class ContactController : BaseController
    {
        private readonly IContactApplyRequestRepository _contactApplyRequestRepository;
        private readonly IContactBookRepository _contactBookRepository;
        private readonly IUserService _userService;

        public ContactController(IContactApplyRequestRepository contactApplyRequestRepository, IUserService userService, IContactBookRepository contactBookRepository)
        {
            _contactApplyRequestRepository = contactApplyRequestRepository;
            _userService = userService;
            _contactBookRepository = contactBookRepository;
        }

        /// <summary>
        /// 获取当前用户的好友列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            var request = await _contactBookRepository.GetContactsAsync(this.UserIdentity.UserId);
            return Json(request);
        }

        /// <summary>
        /// 获取当前用户的好友申请列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("apply-request")]
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
        [HttpPost("apply-request/{userId}")]
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
                    ApplicantId = this.UserIdentity.UserId
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
        [HttpPut("apply-request/{applicantId}")]
        public async Task<IActionResult> ApproveApplyRequest(int applicantId)
        {
            var result = await _contactApplyRequestRepository.ApproveAsync(UserIdentity.UserId, applicantId);
            if (!result) return BadRequest();

            var applicant = await _userService.GetBaseUserInfoAsync(applicantId);
            var currentUser = await _userService.GetBaseUserInfoAsync(UserIdentity.UserId);
            await _contactBookRepository.AddContactAysnc(applicantId, currentUser);
            await _contactBookRepository.AddContactAysnc(UserIdentity.UserId, applicant);
            return Ok();
        }

        /// <summary>
        /// 更新好友标签
        /// </summary>
        /// <param name="applicantId"></param>
        /// <returns></returns>
        [HttpPut("tags")]
        public async Task<IActionResult> UpdateContactTags([FromBody]ContactTagsInputViewModel tagsViewModel)
        {
            var result = await _contactBookRepository.UpdateContactTagsAsync(UserIdentity.UserId, tagsViewModel.ContatId, tagsViewModel.Tags);
            return Json(result);
        }

    }
}
