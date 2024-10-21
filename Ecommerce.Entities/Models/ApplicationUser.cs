using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public int? Age { get; set; }
        public string? City { get; set; }
        public string Adress { get; set; }

    }
}
