using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contact.Api.Models
{
    public class ContactBook
    {
        public int UserId { get; set; }
        List<Contact> Contacts { get; set; }
    }
}
