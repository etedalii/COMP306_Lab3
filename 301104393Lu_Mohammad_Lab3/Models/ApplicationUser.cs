using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace _301104393Lu_Mohammad_Lab3.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() : base()
        {
                
        }

        [DisplayName("First Name")]
        public string Name { get; set; }

        [DisplayName("Last Name")]
        public string Family { get; set; }
    }
}
