using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recommend.Api.Models
{
    public class ProjectReferenceUser
    {
        public int Id { get; set; }
        public int UseId { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string Company { get; set; }
    }
}
