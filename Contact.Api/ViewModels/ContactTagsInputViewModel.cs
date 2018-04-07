using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contact.Api.ViewModels
{
    public class ContactTagsInputViewModel
    {
        public int ContatId { get; set; }
        public List<string> Tags { get; set; }
    }
}
