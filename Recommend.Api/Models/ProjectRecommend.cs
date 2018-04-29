using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recommend.Api.Models
{
    public class ProjectRecommend
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FromUserId { get; set; }
        public string FromUserName { get; set; }
        public string FromUserAvatar { get; set; }
        /// <summary>
        /// 推荐类型 1好友推荐 2系统推荐 3 二度好友推荐
        /// </summary>
        public int RecommendType { get; set; }
        public int ProjectId { get; set; }

        public string ProjectAvatar { get; set; }
        public string Company { get; set; }
        public string Introduction { get; set; }
        public string Tags { get; set; }
        /// <summary>
        /// 融资阶段
        /// </summary>
        public string FinancingStage { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime RecommendedTime { get; set; }
        public List<ProjectReferenceUser> ReferenceUsers { get; set; } = new List<ProjectReferenceUser>();
    }
}
