using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Api.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Title { get; set; }
        /// <summary>
        /// 头像地址
        /// </summary>
        public string Avatar { get; set; }
        public byte Gender { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public string Province { get; set; }
        public int ProvinceID { get; set; }
        public string City { get; set; }
        public int CityID { get; set; }
        public string NameCard { get; set; }
        List<UserProperty> Properties { get; set; }
    }
}
