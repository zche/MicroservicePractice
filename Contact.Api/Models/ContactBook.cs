using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contact.Api.Models
{
    public class ContactBook : EntityBase<int>
    {
        public int UserId { get; set; }
        public List<Contact> Contacts { get; set; } = new List<Contact>();
    }
}
