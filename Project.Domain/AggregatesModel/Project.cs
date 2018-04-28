using Project.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Project.Domain.Events;

namespace Project.Domain.AggregatesModel
{
    public class Project : Entity, IAggregateRoot
    {
        #region 属性
        public int UserId { get; set; }

        public string UserName { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 原BP文件地址
        /// </summary>
        public string OriginBPFile { get; set; }

        /// <summary>
        /// 转换后的BP文件地址
        /// </summary>
        public string FormatBPFile { get; set; }

        /// <summary>
        /// 是否显示敏感信息
        /// </summary>
        public bool IsShowedSecurityInfo { get; set; }

        public int ProvinceId { get; set; }
        public string Province { get; set; }
        public int CityId { get; set; }
        public string City { get; set; }

        public int AreaId { get; set; }
        public string AreaName { get; set; }

        public DateTime RegisterTime { get; set; }

        public string Introduction { get; set; }

        /// <summary>
        /// 股票比例
        /// </summary>
        public int StockPercentage { get; set; }
        /// <summary>
        /// 融资阶段
        /// </summary>
        public int FinancingStage { get; set; }
        /// <summary>
        /// 融资金额（万）
        /// </summary>
        public int FinancingMoney { get; set; }
        /// <summary>
        /// 收入单位（万）
        /// </summary>
        public int Income { get; set; }
        /// <summary>
        /// 利润单位（万）
        /// </summary>
        public int Revenue { get; set; }
        /// <summary>
        /// 估值
        /// </summary>
        public int Valuation { get; set; }

        /// <summary>
        /// 佣金分配方式
        /// </summary>
        public int BrokerageMode { get; set; }

        /// <summary>
        /// 是否委托给平台
        /// </summary>
        public bool IsOnPlatform { get; set; }

        /// <summary>
        /// 可见范围设置
        /// </summary>
        public ProjectVisibleRule VisibleRule { get; set; }

        /// <summary>
        /// 根引用项目id
        /// </summary>
        public int SourceId { get; set; }

        /// <summary>
        /// 上级引用项目id
        /// </summary>
        public int ReferenceId { get; set; }

        /// <summary>
        /// 项目标签
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 项目属性:行业领域、融资币种
        /// </summary>
        public List<ProjectProperty> Properties { get; set; }

        /// <summary>
        /// 贡献者列表
        /// </summary>
        public List<ProjectContributor> Contributors { get; set; }
        /// <summary>
        /// 查看者
        /// </summary>
        public List<ProjectViewer> Viewers { get; set; }

        public DateTime UpdateTime { get; set; }
        public DateTime CreatedTime { get; set; }

        #endregion

        public Project()
        {
            this.Viewers = new List<ProjectViewer>();
            this.Contributors = new List<ProjectContributor>();
            this.AddDomainEvent(new ProjectCreatedEvent { Project = this });
        }
        private Project CloneProject(Project source = null)
        {
            if (source == null) source = this;
            var newProject = new Project
            {
                AreaId = source.AreaId,
                AreaName = source.AreaName,
                Avatar = source.Avatar,
                BrokerageMode = source.BrokerageMode,
                City = source.City,
                CityId = source.CityId,
                Company = source.Company,
                Contributors = new List<ProjectContributor>(),
                CreatedTime = DateTime.Now,
                FinancingMoney = source.FinancingMoney,
                FinancingStage = source.FinancingStage,
                FormatBPFile = source.FormatBPFile,
                Income = source.Income,
                Introduction = source.Introduction,
                IsOnPlatform = source.IsOnPlatform,
                IsShowedSecurityInfo = source.IsShowedSecurityInfo,
                OriginBPFile = source.OriginBPFile,
                Properties = new List<ProjectProperty>(),
                Province = source.Province,
                ProvinceId = source.ProvinceId,
                RegisterTime = source.RegisterTime,
                Revenue = source.Revenue,
                StockPercentage = source.StockPercentage,
                Tags = source.Tags,
                UpdateTime = DateTime.Now,
                Valuation = source.Valuation,
                Viewers = new List<ProjectViewer>(),
                VisibleRule = source.VisibleRule == null ? null : new ProjectVisibleRule
                { Visible = source.VisibleRule.Visible, Tags = source.VisibleRule.Tags },

            };
            source.Properties.ForEach(p =>
            {
                newProject.Properties.Add(new ProjectProperty
                {
                    Key = p.Key,
                    Text = p.Text,
                    Value = p.Value
                });
            });
            return newProject;
        }

        public Project ContributorFork(int contributorId, Project source = null)
        {
            if (source == null) source = this;
            var newProject = this.CloneProject(source);
            newProject.UserId = contributorId;
            newProject.SourceId = source.SourceId == 0 ? source.Id : source.SourceId;
            newProject.ReferenceId = source.ReferenceId == 0 ? source.Id : source.SourceId;
            return newProject;
        }

        public void AddViewer(ProjectViewer projectViewer)
        {
            if (!this.Viewers.Any(v => v.UserId == projectViewer.UserId))
            {
                this.Viewers.Add(projectViewer);
                this.AddDomainEvent(new ProjectViewedEvent { ProjectViewer = projectViewer });
            }
        }
        public void AddContributor(ProjectContributor projectContributor)
        {
            if (!this.Contributors.Any(v => v.UserId == projectContributor.UserId))
            {
                this.Contributors.Add(projectContributor);
                this.AddDomainEvent(new ProjectJoinedEvent { ProjectContributor = projectContributor });
            }
        }
    }
}
