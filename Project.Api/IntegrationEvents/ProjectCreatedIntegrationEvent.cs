﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Api.IntegrationEvents
{
    public class ProjectCreatedIntegrationEvent
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public string ProjectAvatar { get; set; }
        public string Company { get; set; }
        public string Introduction { get; set; }
        public string Tags { get; set; }
        /// <summary>
        /// 融资阶段
        /// </summary>
        public int FinancingStage { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
