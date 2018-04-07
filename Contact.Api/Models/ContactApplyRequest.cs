using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contact.Api.Models
{
    public class ContactApplyRequest : EntityBase<int>
    {
        public int ApplicantId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Title { get; set; }
        public string Avatar { get; set; }        
        /// <summary>
        /// 是否通过，0表示未通过，1表示通过
        /// </summary>
        public int Approved { get; set; }
        public DateTime HandledTime { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
